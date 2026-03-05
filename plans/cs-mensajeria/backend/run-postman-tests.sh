#!/bin/bash

# WePlay.CsMensajeria Integration Tests Runner
# Ejecuta la colección Postman con Newman para la feature cs-mensajeria

set -e

# Colores
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Variables
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
COLLECTION_FILE="${SCRIPT_DIR}/postman-collection.json"
BASE_URL="${BASE_URL:-http://localhost:5001}"
TEST_PASSWORD="${TEST_PASSWORD:-TestPassword123!}"
REPORTERS="${REPORTERS:-cli,htmlextra}"
DELAY_REQUEST="${DELAY_REQUEST:-0}"
TIMEOUT_REQUEST="${TIMEOUT_REQUEST:-5000}"

# Flags
VERBOSE=false
HELP=false
CHECK_ONLY=false
FOLDER=""

# Función para mostrar ayuda
show_help() {
    cat << EOF
${BLUE}WePlay CsMensajeria Integration Tests Runner${NC}

USAGE:
    ./run-postman-tests.sh [OPTIONS]

OPTIONS:
    -h, --help          Mostrar esta ayuda
    -u, --url URL       URL del backend (default: http://localhost:5001)
    -p, --password PWD  Contraseña de prueba (default: TestPassword123!)
    -r, --reporters REP Reporters a usar (default: cli,htmlextra)
    -d, --delay MS      Delay entre requests en ms (default: 0)
    -t, --timeout MS    Timeout de request en ms (default: 5000)
    -f, --folder NAME   Ejecutar solo un folder específico
    -v, --verbose       Mostrar output detallado
    -c, --check         Solo verificar que Newman esté instalado (no ejecutar)

EJEMPLOS:
    # Ejecución completa
    ./run-postman-tests.sh

    # Con URL personalizada
    ./run-postman-tests.sh --url http://api.example.com:5001

    # Solo folder de validaciones
    ./run-postman-tests.sh --folder "Conversaciones - Validation Errors"

    # Con delay y verbose
    ./run-postman-tests.sh -d 500 -v

EOF
}

# Parser de argumentos
parse_args() {
    while [[ $# -gt 0 ]]; do
        case $1 in
            -h|--help)
                HELP=true
                shift
                ;;
            -u|--url)
                BASE_URL="$2"
                shift 2
                ;;
            -p|--password)
                TEST_PASSWORD="$2"
                shift 2
                ;;
            -r|--reporters)
                REPORTERS="$2"
                shift 2
                ;;
            -d|--delay)
                DELAY_REQUEST="$2"
                shift 2
                ;;
            -t|--timeout)
                TIMEOUT_REQUEST="$2"
                shift 2
                ;;
            -f|--folder)
                FOLDER="$2"
                shift 2
                ;;
            -v|--verbose)
                VERBOSE=true
                shift
                ;;
            -c|--check)
                CHECK_ONLY=true
                shift
                ;;
            *)
                echo -e "${RED}Error: opción desconocida '$1'${NC}"
                show_help
                exit 1
                ;;
        esac
    done
}

# Verificar que Newman esté instalado
check_newman() {
    if ! command -v newman &> /dev/null; then
        echo -e "${RED}Error: Newman no está instalado${NC}"
        echo -e "${YELLOW}Instálalo con:${NC}"
        echo "  npm install -g newman"
        echo "  npm install -g newman-reporter-htmlextra"
        return 1
    fi
    return 0
}

# Verificar conectividad al backend
check_backend() {
    echo -e "${BLUE}Verificando conectividad a ${BASE_URL}...${NC}"

    if ! command -v curl &> /dev/null; then
        echo -e "${YELLOW}curl no disponible, omitiendo verificación de conectividad${NC}"
        return 0
    fi

    if curl -s "${BASE_URL}/swagger/index.html" > /dev/null 2>&1; then
        echo -e "${GREEN}✓ Backend accesible en ${BASE_URL}${NC}"
        return 0
    else
        echo -e "${RED}✗ No se pudo conectar a ${BASE_URL}${NC}"
        echo -e "${YELLOW}Asegúrate de que el backend está ejecutándose:${NC}"
        echo "  - Backend API: ${BASE_URL}"
        echo "  - Swagger: ${BASE_URL}/swagger"
        return 1
    fi
}

# Función principal de ejecución
run_tests() {
    echo -e "${BLUE}========================================${NC}"
    echo -e "${BLUE}WePlay CsMensajeria Integration Tests${NC}"
    echo -e "${BLUE}========================================${NC}"
    echo ""

    # Mostrar configuración
    echo -e "${YELLOW}Configuración:${NC}"
    echo "  Base URL: ${BASE_URL}"
    echo "  Reporters: ${REPORTERS}"
    echo "  Delay: ${DELAY_REQUEST}ms"
    echo "  Timeout: ${TIMEOUT_REQUEST}ms"
    if [ -n "$FOLDER" ]; then
        echo "  Folder: ${FOLDER}"
    fi
    echo ""

    # Construcción del comando
    CMD="newman run ${COLLECTION_FILE}"
    CMD="${CMD} --var baseUrl=${BASE_URL}"
    CMD="${CMD} --var testPassword=${TEST_PASSWORD}"
    CMD="${CMD} --reporters ${REPORTERS}"
    CMD="${CMD} --timeout-request ${TIMEOUT_REQUEST}"

    if [ "$VERBOSE" = true ]; then
        CMD="${CMD} -v"
    fi

    if [ "$DELAY_REQUEST" -gt 0 ]; then
        CMD="${CMD} --delay-request ${DELAY_REQUEST}"
    fi

    if [ -n "$FOLDER" ]; then
        CMD="${CMD} --folder \"${FOLDER}\""
    fi

    # Exportar resultados a archivos
    RESULTS_DIR="${SCRIPT_DIR}/test-results"
    mkdir -p "${RESULTS_DIR}"

    TIMESTAMP=$(date +%Y%m%d_%H%M%S)
    JSON_EXPORT="${RESULTS_DIR}/results_${TIMESTAMP}.json"
    HTML_EXPORT="${RESULTS_DIR}/results_${TIMESTAMP}.html"

    CMD="${CMD} --reporter-json-export ${JSON_EXPORT}"
    CMD="${CMD} --reporter-htmlextra-export ${HTML_EXPORT}"

    echo -e "${BLUE}Ejecutando colección...${NC}"
    echo ""

    # Ejecutar comando
    eval "$CMD"
    EXIT_CODE=$?

    echo ""
    echo -e "${BLUE}========================================${NC}"

    if [ $EXIT_CODE -eq 0 ]; then
        echo -e "${GREEN}✓ TESTS EXITOSOS${NC}"
        echo -e "${YELLOW}Resultados guardados en:${NC}"
        echo "  JSON: ${JSON_EXPORT}"
        echo "  HTML: ${HTML_EXPORT}"
    else
        echo -e "${RED}✗ TESTS FALLARON${NC}"
        echo -e "${YELLOW}Revisar errores en:${NC}"
        echo "  JSON: ${JSON_EXPORT}"
        echo "  HTML: ${HTML_EXPORT}"
    fi

    echo -e "${BLUE}========================================${NC}"

    return $EXIT_CODE
}

# Main
main() {
    parse_args "$@"

    if [ "$HELP" = true ]; then
        show_help
        exit 0
    fi

    # Verificar Newman
    if ! check_newman; then
        exit 1
    fi

    echo -e "${GREEN}✓ Newman instalado${NC}"
    echo ""

    if [ "$CHECK_ONLY" = true ]; then
        echo -e "${GREEN}Verificación completada exitosamente${NC}"
        exit 0
    fi

    # Verificar backend
    if ! check_backend; then
        exit 1
    fi

    echo ""

    # Ejecutar tests
    run_tests
    exit $?
}

# Ejecutar main con todos los argumentos
main "$@"
