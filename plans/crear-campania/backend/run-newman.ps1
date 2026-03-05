<#
.SYNOPSIS
    Newman Test Runner - WePlay.CrearCampania.IntegrationTests
    Script para ejecutar tests de integracion usando Newman CLI

.DESCRIPTION
    Ejecuta la coleccion Postman con Newman, generando reportes en
    formatos: CLI, HTML, JSON, JUnit, HTMLExtra

.PARAMETER Environment
    Ambiente donde ejecutar los tests (default: development)
    Valores: development, staging, production

.PARAMETER Folder
    Ejecutar solo un folder especifico de la coleccion

.PARAMETER DryRun
    Realizar dry run sin ejecutar requests

.PARAMETER Bail
    Detener en primer fallo

.PARAMETER Verbose
    Output verbose

.EXAMPLE
    # Ejecucion basica
    .\run-newman.ps1

    # Ejecucion en staging
    .\run-newman.ps1 -Environment staging

    # Ejecutar solo un folder
    .\run-newman.ps1 -Folder "Campanias - Create"

    # Con opciones adicionales
    .\run-newman.ps1 -Environment development -Bail -Verbose

.NOTES
    Requisitos:
    - Node.js 16+
    - Newman: npm install -g newman
    - Archivos: postman-collection.json, environment-{env}.json
#>

param(
    [Parameter(Position = 0)]
    [ValidateSet('development', 'staging', 'production')]
    [string]$Environment = 'development',

    [Parameter()]
    [string]$Folder,

    [Parameter()]
    [switch]$DryRun,

    [Parameter()]
    [switch]$Bail,

    [Parameter()]
    [switch]$Verbose,

    [Parameter()]
    [switch]$ShowHelp
)

# Funciones
function Write-Header {
    param([string]$Message)
    Write-Host ""
    Write-Host "═══════════════════════════════════════════════════════════════" -ForegroundColor Cyan
    Write-Host "  $Message" -ForegroundColor Cyan
    Write-Host "═══════════════════════════════════════════════════════════════" -ForegroundColor Cyan
}

function Write-Success {
    param([string]$Message)
    Write-Host "✓ $Message" -ForegroundColor Green
}

function Write-Error-Custom {
    param([string]$Message)
    Write-Host "✗ $Message" -ForegroundColor Red
}

function Write-Warning-Custom {
    param([string]$Message)
    Write-Host "⚠ $Message" -ForegroundColor Yellow
}

function Write-Info {
    param([string]$Message)
    Write-Host "ℹ $Message" -ForegroundColor Cyan
}

function Show-Help {
    $help = @"
Newman Test Runner - WePlay.CrearCampania.IntegrationTests

SINTAXIS:
    .\run-newman.ps1 [[-Environment] <string>] [-Folder <string>] [-DryRun] [-Bail] [-Verbose] [-ShowHelp]

PARAMETROS:
    -Environment <string>
        Ambiente donde ejecutar (default: development)
        Valores: development, staging, production

    -Folder <string>
        Ejecutar solo un folder especifico

    -DryRun
        Dry run sin ejecutar requests

    -Bail
        Detener en primer fallo

    -Verbose
        Output verbose

    -ShowHelp
        Mostrar esta ayuda

EJEMPLOS:
    # Ejecucion basica
    .\run-newman.ps1

    # Ejecucion en staging
    .\run-newman.ps1 -Environment staging

    # Ejecutar solo crear campanias
    .\run-newman.ps1 -Folder "Campanias - Create"

    # Con opciones
    .\run-newman.ps1 -Environment development -Bail -Verbose

    # Mostrar ayuda
    .\run-newman.ps1 -ShowHelp
"@
    Write-Host $help
}

function Check-Requirements {
    Write-Header "Verificando requisitos"

    # Verificar Node.js
    $nodeCmd = Get-Command node -ErrorAction SilentlyContinue
    if (!$nodeCmd) {
        Write-Error-Custom "Node.js no está instalado"
        exit 1
    }
    $nodeVersion = & node --version
    Write-Success "Node.js $nodeVersion"

    # Verificar Newman
    $newmanCmd = Get-Command newman -ErrorAction SilentlyContinue
    if (!$newmanCmd) {
        Write-Warning-Custom "Newman no encontrado. Instalando globalmente..."
        & npm install -g newman
    }
    $newmanVersion = & newman --version
    Write-Success "Newman $newmanVersion"

    # Verificar coleccion
    $collectionPath = Join-Path $PSScriptRoot "postman-collection.json"
    if (!(Test-Path $collectionPath)) {
        Write-Error-Custom "Archivo de coleccion no encontrado: $collectionPath"
        exit 1
    }
    Write-Success "Coleccion encontrada"

    # Verificar environment
    $envPath = Join-Path $PSScriptRoot "environment-$Environment.json"
    if (!(Test-Path $envPath)) {
        Write-Error-Custom "Archivo de environment no encontrado: $envPath"
        exit 1
    }
    Write-Success "Environment encontrado: $Environment"

    return @{
        Collection = $collectionPath
        Environment = $envPath
    }
}

function Run-Tests {
    param(
        [hashtable]$Paths
    )

    Write-Header "Ejecutando tests"

    # Crear directorio de reportes
    $reportDir = Join-Path $PSScriptRoot "reports"
    if (!(Test-Path $reportDir)) {
        New-Item -ItemType Directory -Path $reportDir | Out-Null
    }

    $timestamp = Get-Date -Format "yyyyMMdd_HHmmss"

    # Construir argumentos
    $arguments = @(
        "run"
        $Paths.Collection
        "-e"
        $Paths.Environment
        "--reporters"
        "cli,html,json,junit,htmlextra"
        "--reporter-html-export=$(Join-Path $reportDir "report_$timestamp.html")"
        "--reporter-json-export=$(Join-Path $reportDir "results_$timestamp.json")"
        "--reporter-junit-export=$(Join-Path $reportDir "results_$timestamp.xml")"
        "--reporter-htmlextra-title=WePlay Integration Tests - $Environment - $timestamp"
        "--timeout"
        "30000"
    )

    # Agregar folder si se especifica
    if ($Folder) {
        $arguments += @("--folder", $Folder)
    }

    # Agregar opciones adicionales
    if ($DryRun) {
        $arguments += "--dry-run"
    }
    if ($Bail) {
        $arguments += "--bail"
    }
    if ($Verbose) {
        $arguments += "--verbose"
    }

    Write-Info "Comando: newman $(($arguments | ConvertTo-Json -Compress))"
    Write-Host ""

    # Ejecutar
    & newman @arguments
}

function Show-Results {
    param([string]$ReportDir, [string]$Timestamp)

    Write-Header "Resultados"

    $resultsJson = Join-Path $ReportDir "results_$Timestamp.json"
    $reportHtml = Join-Path $ReportDir "report_$Timestamp.html"
    $resultsXml = Join-Path $ReportDir "results_$Timestamp.xml"

    if (Test-Path $resultsJson) {
        Write-Success "Reporte JSON: $resultsJson"
    }

    if (Test-Path $reportHtml) {
        Write-Success "Reporte HTML: $reportHtml"

        # Intentar abrir en navegador
        try {
            Invoke-Item $reportHtml
        }
        catch {
            Write-Warning-Custom "No se pudo abrir el reporte en navegador automáticamente"
        }
    }

    if (Test-Path $resultsXml) {
        Write-Success "Reporte JUnit: $resultsXml"
    }
}

# Main
if ($ShowHelp) {
    Show-Help
    exit 0
}

Write-Header "WePlay Integration Tests - Newman Runner"
Write-Info "Ambiente: $Environment"
Write-Info "Timestamp: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')"
if ($Folder) {
    Write-Info "Folder: $Folder"
}
Write-Host ""

$paths = Check-Requirements
Write-Host ""

$reportDir = Join-Path $PSScriptRoot "reports"
$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"

Run-Tests -Paths $paths
Write-Host ""

Show-Results -ReportDir $reportDir -Timestamp $timestamp

Write-Success "Tests completados"
