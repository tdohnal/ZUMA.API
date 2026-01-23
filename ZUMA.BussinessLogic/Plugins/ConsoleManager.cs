using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

namespace ZUMA.BussinessLogic.Plugins;

[SuppressUnmanagedCodeSecurity]
public static class ConsoleManager
{
    private const string Kernel32_DllName = "kernel32.dll";

    public static bool HasConsole => GetConsoleWindow() != IntPtr.Zero;

    [DllImport(Kernel32_DllName)]
    private static extern bool AllocConsole();

    [DllImport(Kernel32_DllName)]
    private static extern bool FreeConsole();

    [DllImport(Kernel32_DllName)]
    private static extern IntPtr GetConsoleWindow();

    [DllImport(Kernel32_DllName)]
    private static extern int GetConsoleOutputCP();

    public static void Show()
    {
        if (!HasConsole)
        {
            AllocConsole();
            InvalidateOutAndError();
        }
    }

    public static void Hide()
    {
        //#if DEBUG
        if (HasConsole)
        {
            SetOutAndErrorNull();
            FreeConsole();
        }
        //#endif
    }

    public static void Toggle()
    {
        if (HasConsole)
            Hide();
        else
            Show();
    }

    private static void InvalidateOutAndError()
    {
        var type = typeof(System.Console);

        var sOut = type.GetField("s_out", BindingFlags.Static | BindingFlags.NonPublic);

        var sError = type.GetField("s_error", BindingFlags.Static | BindingFlags.NonPublic);

        Debug.Assert(sOut != null);
        Debug.Assert(sError != null);

        sOut.SetValue(null, null);
        sError.SetValue(null, null);
    }

    private static void SetOutAndErrorNull()
    {
        System.Console.SetOut(TextWriter.Null);
        System.Console.SetError(TextWriter.Null);
    }
}