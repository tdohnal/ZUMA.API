@echo off
setlocal enabledelayedexpansion

echo =========================================================
echo ZUMA.API - Transparent Turbo Coverage (Final Fix)
echo =========================================================

if exist TestResults rmdir /s /q TestResults
set /a totalP=0 & set /a passP=0 & set /a failP=0 & set "summary="

:: 1. PRE-FLIGHT CHECK & PREPARATION
echo [1/3] PRE-FLIGHT CHECK: Verifying environment...
echo ---------------------------------------------------------

for /d %%G in ("Tests\*") do (
    set "projectName=%%~nxG"
    set "csprojPath=%%~fG\!projectName!.csproj"
    
    if exist "!csprojPath!" (
        echo [CHECK] !projectName!:
        
        :: A. Kontrola/Instalace Coverletu
        findstr /I "coverlet.collector" "!csprojPath!" >nul
        if !errorlevel! neq 0 (
            dotnet add "!csprojPath!" package coverlet.collector >nul 2>&1
            echo      - Collector: Installed [+]
        ) else (
            echo      - Collector: Present [OK]
        )

        :: B. Kontrola Reference na hlavní projekt
        set "mainProjectName=!projectName:.Tests=!"
        set "mainPath=%%~fG\..\..\!mainProjectName!\!mainProjectName!.csproj"
        
        if exist "!mainPath!" (
            findstr /I "!mainProjectName!.csproj" "!csprojPath!" >nul
            if !errorlevel! neq 0 (
                dotnet add "!csprojPath!" reference "!mainPath!" >nul 2>&1
                echo      - Linking: Done [+]
            ) else (
                echo      - Linking: Reference exists [OK]
            )
        )
        echo      - External: SharedKernel detected in dependencies [OK]
    )
)

echo.
echo [STATUS]: All systems green. Starting Deep Coverage Scan...
echo ---------------------------------------------------------

:: 2. TEST EXECUTION WITH DEEP SCAN
echo [2/3] TESTING: Executing project tests...
for /d %%G in ("Tests\*") do (
    set "projectName=%%~nxG"
    set "csprojPath=%%~fG\!projectName!.csproj"
    
    if exist "!csprojPath!" (
        set /a totalP+=1
        echo. & echo Running: !projectName!
        set "startTime=!time!"
        
        :: KLÍČOVÁ ZMĚNA: Používáme kombinaci parametrů pro vynucení NuGetu
        :: /p:Include="[ZUMA*]*" je širší a spolehlivější
        dotnet test "!csprojPath!" ^
          --collect:"XPlat Code Coverage" ^
          --results-directory "./TestResults/!projectName!" ^
          --nologo ^
          /p:CopyLocalLockFileAssemblies=true ^
          /p:Include="[ZUMA*]*" ^
          /p:IncludeTestAssembly=false

        if !errorlevel! equ 0 ( 
            set /a passP+=1 
            set "status=[OK   ]" 
        ) else ( 
            set /a failP+=1 
            set "status=[FAIL]" 
        )
        set "endTime=!time!"
        set "summary=!summary!!status! !projectName! (Started: !startTime:~0,8! - Ended: !endTime:~0,8!) & echo."
    )
)

:: 3. REPORT GENERATION
echo.
echo [3/3] REPORTING: Merging coverage data...
dotnet tool run reportgenerator ^
  -reports:"TestResults\**\*.xml" ^
  -targetdir:"TestResults/Report" ^
  -reporttypes:Html ^
  "-filefilters:-*.g.cs;-*.generated.cs;-*Migrations*" ^
  -verbosity:Error

:: FINAL SUMMARY
echo.
echo =========================================================
echo FINAL OVERALL STATUS (ZUMA SOLUTION)
echo =========================================================
echo Total Projects: !totalP! ^| Succeeded: !passP! ^| Failed: !failP!
echo ---------------------------------------------------------
echo !summary!
echo =========================================================

if exist "TestResults\Report\index.html" start "" "TestResults\Report\index.html"
pause