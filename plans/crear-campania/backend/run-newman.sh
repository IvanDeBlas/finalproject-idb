#!/bin/bash

#############################################################################
# Newman Test Runner - WePlay.CrearCampania.IntegrationTests
# Script para ejecutar tests de integracion usando Newman CLI
#
# Uso:
#   ./run-newman.sh                    # Ejecucion basica (development)
#   ./run-newman.sh staging            # Ejecucion en staging
#   ./run-newman.sh --help             # Ver opciones
#
# Requisitos:
#   - Node.js 16+
#   - Newman: npm install -g newman
#   - Archivo: postman-collection.json
#   - Archivo: environment-{env}.json
#############################################################################

set -e

# Colores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Configuracion
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
COLLECTION_FILE="${SCRIPT_DIR}/postman-collection.json"
ENVIRONMENT="${1:-development}"
ENV_FILE="${SCRIPT_DIR}/environment-${ENVIRONMENT}.json"
REPORT_DIR="${SCRIPT_DIR}/reports"
TIMESTAMP=$(date +%Y%m%d_%H%M%S)

# Funciones
print_header() {
    echo -e "${BLUE}═══════════════════════════════════════════════════════════════${NC}"
    echo -e "${BLUE}  $1${NC}"
    echo -e "${BLUE}═══════════════════════════════════════════════════════════════${NC}"
}

print_success() {
    echo -e "${GREEN}✓ $1${NC}"
}

print_error() {
    echo -e "${RED}✗ $1${NC}"
}

print_warning() {
    echo -e "${YELLOW}⚠ $1${NC}"
}

print_info() {
    echo -e "${BLUE}ℹ $1${NC}"
}

check_requirements() {
    print_header "Verificando requisitos"

    # Verificar Node.js
    if ! command -v node &> /dev/null; then
        print_error "Node.js no está instalado"
        exit 1
    fi
    print_success "Node.js $(node --version)"

    # Verificar Newman
    if ! command -v newman &> /dev/null; then
        print_warning "Newman no encontrado. Instalando globalmente..."
        npm install -g newman
    fi
    print_success "Newman $(newman --version)"

    # Verificar coleccion
    if [ ! -f "$COLLECTION_FILE" ]; then
        print_error "Archivo de coleccion no encontrado: $COLLECTION_FILE"
        exit 1
    fi
    print_success "Coleccion encontrada"

    # Verificar environment
    if [ ! -f "$ENV_FILE" ]; then
        print_error "Archivo de environment no encontrado: $ENV_FILE"
        exit 1
    fi
    print_success "Environment encontrado: $ENVIRONMENT"
}

show_help() {
    cat << EOF
${BLUE}Newman Test Runner - WePlay.CrearCampania.IntegrationTests${NC}

${YELLOW}Uso:${NC}
    $0 [ambiente] [opciones]

${YELLOW}Ambientes disponibles:${NC}
    development    Localhost (default)
    staging        Staging environment
    production     Production environment (cuidado!)

${YELLOW}Opciones:${NC}
    --folder       Ejecutar solo un folder especifico
    --dry-run      Dry run sin ejecutar requests
    --bail         Detener en primer fallo
    --verbose      Output verbose
    --help         Mostrar esta ayuda

${YELLOW}Ejemplos:${NC}
    # Ejecucion basica en development
    $0

    # Ejecucion en staging
    $0 staging

    # Ejecutar solo crear campanias
    $0 development --folder "Campanias - Create"

    # Ejecucion verbose con bail
    $0 development --bail --verbose

EOF
}

run_tests() {
    print_header "Ejecutando tests"

    # Crear directorio de reportes
    mkdir -p "$REPORT_DIR"

    # Construir comando
    local cmd="newman run \"$COLLECTION_FILE\" -e \"$ENV_FILE\""

    # Agregar reportes
    cmd="$cmd --reporters cli,html,json,junit,htmlextra"
    cmd="$cmd --reporter-html-export=\"${REPORT_DIR}/report_${TIMESTAMP}.html\""
    cmd="$cmd --reporter-json-export=\"${REPORT_DIR}/results_${TIMESTAMP}.json\""
    cmd="$cmd --reporter-junit-export=\"${REPORT_DIR}/results_${TIMESTAMP}.xml\""
    cmd="$cmd --reporter-htmlextra-title=\"WePlay Integration Tests - ${ENVIRONMENT} - ${TIMESTAMP}\""

    # Agregar timeout
    cmd="$cmd --timeout 30000"

    # Agregar opciones adicionales
    for arg in "${@:2}"; do
        case $arg in
            --folder)
                shift
                cmd="$cmd --folder \"$1\""
                ;;
            --dry-run)
                cmd="$cmd --dry-run"
                ;;
            --bail)
                cmd="$cmd --bail"
                ;;
            --verbose)
                cmd="$cmd --verbose"
                ;;
            *)
                ;;
        esac
    done

    print_info "Comando: $cmd"
    echo

    # Ejecutar
    eval "$cmd"
}

show_results() {
    print_header "Resultados"

    if [ -f "${REPORT_DIR}/results_${TIMESTAMP}.json" ]; then
        print_success "Reporte JSON: ${REPORT_DIR}/results_${TIMESTAMP}.json"
    fi

    if [ -f "${REPORT_DIR}/report_${TIMESTAMP}.html" ]; then
        print_success "Reporte HTML: ${REPORT_DIR}/report_${TIMESTAMP}.html"

        # Intentar abrir en navegador si es macOS o Linux
        if command -v open &> /dev/null; then
            open "${REPORT_DIR}/report_${TIMESTAMP}.html"
        elif command -v xdg-open &> /dev/null; then
            xdg-open "${REPORT_DIR}/report_${TIMESTAMP}.html"
        fi
    fi

    if [ -f "${REPORT_DIR}/results_${TIMESTAMP}.xml" ]; then
        print_success "Reporte JUnit: ${REPORT_DIR}/results_${TIMESTAMP}.xml"
    fi
}

# Main
main() {
    case "${1:-}" in
        --help|-h)
            show_help
            exit 0
            ;;
        "")
            ;;
        *)
            if [[ "$1" != --* ]]; then
                ENVIRONMENT="$1"
            fi
            ;;
    esac

    print_header "WePlay Integration Tests - Newman Runner"
    print_info "Ambiente: ${ENVIRONMENT}"
    print_info "Timestamp: ${TIMESTAMP}"
    echo

    check_requirements
    echo

    run_tests "$@"
    echo

    show_results

    print_success "Tests completados"
}

main "$@"
