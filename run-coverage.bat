@echo off
setlocal enabledelayedexpansion

echo =========================================================
echo ZUMA.API - Transparent Turbo Coverage
echo =========================================================

if exist TestResults rmdir /s /q TestResults
set /a totalP=0 & set /a passP=0 & set /a failP=0 & set "summary="

:: 1. Inteligentní příprava s logováním
echo [PREPARING]: Verifying dependencies and links...
for /d %%G in ("Tests\*") do (
    set "projectName=%%~nxG"
    set "csprojPath=%%G\!projectName!.csproj"
    
    if exist "!csprojPath!" (
        echo  - Project: !projectName!
        
        :: Kontrola Coverletu
        findstr /I "coverlet.collector" "!csprojPath!" >nul
        if !errorlevel! neq 0 (
            echo     [+] Adding: coverlet.collector
            dotnet add "!csprojPath!" package coverlet.collector >nul 2>&1
        ) else (
            echo     [SKIP]: coverlet.collector already present
        )

        :: Kontrola Reference
        set "mainProjectName=!projectName:.Tests=!"
        set "mainPath=%%G\..\..\!mainProjectName!\!mainProjectName!.csproj"
        
        if exist "!mainPath!" (
            findstr /I "!mainProjectName!.csproj" "!csprojPath!" >nul
            if !errorlevel! neq 0 (
                echo     [+] Linking: to !mainProjectName!
                dotnet add "!csprojPath!" reference "!mainPath!" >nul 2>&1
            ) else (
                echo     [SKIP]: Reference to !mainProjectName! already exists
            )
        )
    )
)
echo.
echo [STATUS]: Verification complete.
echo ---------------------------------------------------------

:: 2. Spouštění testů + Měření času
echo [TESTING]: Executing project tests...
for /d %%G in ("Tests\*") do (
    set "projectName=%%~nxG"
    set /a totalP+=1
    echo. & echo Running: !projectName!
    set "startTime=!time!"
    
    dotnet test "%%G\!projectName!.csproj" --collect:"XPlat Code Coverage" --results-directory "./TestResults/!projectName!" --nologo
    
    if !errorlevel! equ 0 ( set /a passP+=1 & set "status=[OK  ]" ) else ( set /a failP+=1 & set "status=[FAIL]" )
    set "endTime=!time!"
    set "summary=!summary!!status! !projectName! (Started: !startTime:~0,8! - Ended: !endTime:~0,8!) & echo."
)

:: 3. Report
echo. & echo [REPORTING]: Merging XML files and cleaning up...
dotnet tool run reportgenerator -reports:"TestResults\**\*.xml" -targetdir:"TestResults/Report" -reporttypes:Html "-filefilters:-*.g.cs;-*.generated.cs" -verbosity:Error

:: Pokud by i tak něco prolezlo, tohle to umlčí úplně:
:: dotnet tool run reportgenerator ... >nul 2>&1

:: 4. Finální tabulka
echo.
echo =========================================================
echo FINAL OVERALL STATUS (ZUMA SOLUTION)
echo =========================================================
echo Total Projects: !totalP! ^| Succeeded: !passP! ^| Failed: !failP!
echo ---------------------------------------------------------
echo %summary%
echo =========================================================

if exist "TestResults\Report\index.html" start "" "TestResults\Report\index.html"
pause