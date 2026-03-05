@echo off
REM Script para ejecutar tests de Dashboard Artista con Newman (Windows)
REM Uso: run-tests.bat [environment] [reporters]

setlocal enabledelayedexpansion

set "SCRIPT_DIR=%~dp0"
set "COLLECTION=%SCRIPT_DIR%postman-collection.json"
set "ENVIRONMENT=%1"
if "!ENVIRONMENT!"=="" (
    set "ENVIRONMENT=environment.json"
)
set "ENVIRONMENT_PATH=%SCRIPT_DIR%!ENVIRONMENT!"
set "REPORTERS=%2"
if "!REPORTERS!"=="" (
    set "REPORTERS=cli"
)

REM Crear directorio de reportes
set "REPORT_DIR=%SCRIPT_DIR%reports"
if not exist "!REPORT_DIR!" (
    mkdir "!REPORT_DIR!"
)

REM Obtener timestamp
for /f "tokens=2-4 delims=/ " %%a in ('date /t') do (set "mydate=%%c%%a%%b")
for /f "tokens=1-2 delims=/:" %%a in ('time /t') do (set "mytime=%%a%%b")
set "TIMESTAMP=!mydate!_!mytime!"

echo ==================================================
echo Dashboard Artista - Integration Tests
echo ==================================================
echo Ambiente: !ENVIRONMENT!
echo Reporteros: !REPORTERS!
echo Timestamp: !TIMESTAMP!
echo.

REM Validar que la coleccion existe
if not exist "!COLLECTION!" (
    echo Error: Coleccion no encontrada en !COLLECTION!
    exit /b 1
)

REM Validar que el environment existe
if not exist "!ENVIRONMENT_PATH!" (
    echo Error: Environment no encontrado en !ENVIRONMENT_PATH!
    echo Ambientes disponibles:
    dir /b "%SCRIPT_DIR%environment*.json"
    exit /b 1
)

REM Ejecutar tests
if "!REPORTERS!"=="html" (
    newman run "!COLLECTION!" ^
        --environment "!ENVIRONMENT_PATH!" ^
        --reporters cli,html ^
        --reporter-html-export "!REPORT_DIR!\report_!TIMESTAMP!.html" ^
        --bail on
) else (
    newman run "!COLLECTION!" ^
        --environment "!ENVIRONMENT_PATH!" ^
        --reporters !REPORTERS! ^
        --bail on
)

set "EXIT_CODE=%ERRORLEVEL%"

echo.
echo ==================================================
if %EXIT_CODE% equ 0 (
    echo Tests PASSED!
) else (
    echo Tests FAILED!
)
echo ==================================================

if %EXIT_CODE% equ 0 (
    if exist "!REPORT_DIR!" (
        echo Reportes generados en: !REPORT_DIR!
        dir /b "!REPORT_DIR!"
    )
)

exit /b %EXIT_CODE%
