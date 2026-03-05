@echo off
REM WePlay Rises - Newman Integration Tests Runner
REM Script para ejecutar pruebas de integración de Campanias API

setlocal enabledelayedexpansion

REM Default values
set "FOLDER="
set "REPORTER=cli,htmlextra"
set "EXPORT_ENV="
set "DELAY=100"

REM Parse command line arguments
:parse_args
if "%1"=="" goto run_tests
if "%1"=="-f" (
    set "FOLDER=%2"
    shift
    shift
    goto parse_args
)
if "%1"=="--folder" (
    set "FOLDER=%2"
    shift
    shift
    goto parse_args
)
if "%1"=="-r" (
    set "REPORTER=%2"
    shift
    shift
    goto parse_args
)
if "%1"=="--reporters" (
    set "REPORTER=%2"
    shift
    shift
    goto parse_args
)
if "%1"=="-e" (
    set "EXPORT_ENV=%2"
    shift
    shift
    goto parse_args
)
if "%1"=="--export" (
    set "EXPORT_ENV=%2"
    shift
    shift
    goto parse_args
)
if "%1"=="-d" (
    set "DELAY=%2"
    shift
    shift
    goto parse_args
)
if "%1"=="--delay" (
    set "DELAY=%2"
    shift
    shift
    goto parse_args
)
if "%1"=="-h" (
    goto show_help
)
if "%1"=="--help" (
    goto show_help
)
goto parse_args

:show_help
echo Usage: run-tests.bat [OPTIONS]
echo.
echo Options:
echo   -f, --folder FOLDER      Run only tests in specified folder
echo                           Examples: "_Setup", "Campanias/201 CREATED", "Campanias/E2E Happy Path"
echo   -r, --reporters REPORTERS Reporters to use (default: cli,htmlextra^)
echo                           Options: cli, json, junitxml, htmlextra
echo   -e, --export FILE        Export final environment variables to file
echo   -d, --delay MS           Delay between requests in milliseconds (default: 100^)
echo   -h, --help               Show this help message
echo.
echo Examples:
echo   run-tests.bat
echo   run-tests.bat -f "_Setup"
echo   run-tests.bat -f "Campanias/E2E Happy Path"
echo   run-tests.bat -r "cli,junitxml" --delay 200
goto end

:run_tests
REM Check if newman is installed
where newman >nul 2>nul
if %errorlevel% neq 0 (
    echo.
    echo [ERROR] Newman is not installed
    echo Install it with: npm install -g newman newman-reporter-htmlextra
    echo.
    exit /b 1
)

REM Create test-results directory if it doesn't exist
if not exist test-results mkdir test-results

echo.
echo [INFO] Newman Integration Tests Runner
echo [INFO] ================================
echo.
echo [INFO] Collection: WePlay.Campanias.IntegrationTests
echo [INFO] Environment: development
echo [INFO] Reporters: %REPORTER%
echo [INFO] Request Delay: %DELAY%ms

if not "!FOLDER!"=="" (
    echo [INFO] Running tests in folder: !FOLDER!
)

if not "!EXPORT_ENV!"=="" (
    echo [INFO] Final environment will be exported to: !EXPORT_ENV!
)

echo.
echo [INFO] Starting test execution...
echo.

REM Build newman command
set "CMD=newman run WePlay.Campanias.IntegrationTests.postman_collection.json -e environments\development.postman_environment.json -r %REPORTER% --delay-request %DELAY%"

REM Add folder filter if specified
if not "!FOLDER!"=="" (
    set "CMD=!CMD! --folder "!FOLDER!""
)

REM Add HTML extra reporter options if htmlextra is in reporters
echo %REPORTER% | find /i "htmlextra" >nul
if %errorlevel% equ 0 (
    set "CMD=!CMD! --reporter-htmlextra-export test-results\report.html"
)

REM Add JUnit reporter options if junitxml is in reporters
echo %REPORTER% | find /i "junitxml" >nul
if %errorlevel% equ 0 (
    set "CMD=!CMD! --reporter-junitxml-export test-results\results.xml"
)

REM Add environment export if specified
if not "!EXPORT_ENV!"=="" (
    set "CMD=!CMD! --export-environment "!EXPORT_ENV!""
)

REM Execute newman
%CMD%

if %errorlevel% neq 0 (
    echo.
    echo [ERROR] Tests failed!
    exit /b 1
)

echo.
echo [INFO] Tests completed successfully!

echo %REPORTER% | find /i "htmlextra" >nul
if %errorlevel% equ 0 (
    echo [INFO] HTML Report: test-results\report.html
)

echo %REPORTER% | find /i "junitxml" >nul
if %errorlevel% equ 0 (
    echo [INFO] JUnit Report: test-results\results.xml
)

echo.
exit /b 0

:end
endlocal
