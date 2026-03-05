#!/bin/bash

# Script para ejecutar las pruebas Postman de valoraciones (US-CS-06)
# Uso: ./run-tests.sh [opciones]

set -e

# Variables
COLLECTION_PATH="plans/cs-valoraciones/backend/postman-collection.json"
ENVIRONMENT_PATH="tests/integration/environments/development.postman_environment.json"
REPORT_DIR="reports/valoraciones"
TIMESTAMP=$(date +%Y%m%d_%H%M%S)
REPORT_NAME="valoraciones_${TIMESTAMP}"

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Funciones helper
print_header() {
    echo ""
    echo "========================================"
    echo "$1"
    echo "========================================"
    echo ""
}

print_success() {
    echo -e "${GREEN}✓ $1${NC}"
}

print_error() {
    echo -e "${RED}✗ $1${NC}"
}

print_info() {
    echo -e "${YELLOW}ℹ $1${NC}"
}

# Verificar que Newman está instalado
if ! command -v newman &> /dev/null; then
    print_error "Newman no está instalado"
    echo "Instala con: npm install -g newman newman-reporter-htmlextra"
    exit 1
fi

# Crear directorio de reportes
mkdir -p "$REPORT_DIR"

print_header "WePlay Rises - Valoraciones Integration Tests (US-CS-06)"

# Parsear argumentos
case "${1:-all}" in
    "all")
        print_info "Ejecutando todas las pruebas..."
        FOLDERS=""
        ;;
    "setup")
        print_info "Ejecutando solo _Setup..."
        FOLDERS='--folder "_Setup"'
        ;;
    "happy")
        print_info "Ejecutando _Setup + Happy Path..."
        FOLDERS='--folder "_Setup,Valoraciones - Happy Path"'
        ;;
    "validation")
        print_info "Ejecutando solo Validation Errors..."
        FOLDERS='--folder "Valoraciones - Validation Errors"'
        ;;
    "business")
        print_info "Ejecutando solo Business Rules..."
        FOLDERS='--folder "Valoraciones - Business Rules"'
        ;;
    "auth")
        print_info "Ejecutando solo Auth Errors..."
        FOLDERS='--folder "Valoraciones - Auth Errors"'
        ;;
    "notfound")
        print_info "Ejecutando solo Not Found..."
        FOLDERS='--folder "Valoraciones - Not Found"'
        ;;
    "pagination")
        print_info "Ejecutando solo Pagination..."
        FOLDERS='--folder "Valoraciones - Pagination"'
        ;;
    "help"|"-h"|"--help")
        echo "Uso: $0 [opciones]"
        echo ""
        echo "Opciones:"
        echo "  all         Ejecutar todas las pruebas (default)"
        echo "  setup       Solo _Setup"
        echo "  happy       _Setup + Happy Path"
        echo "  validation  Solo Validation Errors"
        echo "  business    Solo Business Rules"
        echo "  auth        Solo Auth Errors"
        echo "  notfound    Solo Not Found"
        echo "  pagination  Solo Pagination"
        echo "  help        Mostrar esta ayuda"
        echo ""
        echo "Ejemplos:"
        echo "  ./run-tests.sh all"
        echo "  ./run-tests.sh happy"
        echo "  ./run-tests.sh validation"
        exit 0
        ;;
    *)
        print_error "Opción desconocida: $1"
        echo "Usa '$0 help' para ver las opciones disponibles"
        exit 1
        ;;
esac

# Verificar que archivos existen
if [ ! -f "$COLLECTION_PATH" ]; then
    print_error "Colección no encontrada: $COLLECTION_PATH"
    exit 1
fi

if [ ! -f "$ENVIRONMENT_PATH" ]; then
    print_info "Archivo de entorno no encontrado: $ENVIRONMENT_PATH"
    print_info "Continuando sin entorno (usando variables de la colección)"
fi

# Ejecutar Newman
print_info "Colección: $COLLECTION_PATH"
print_info "Reporte: $REPORT_DIR/$REPORT_NAME.html"
print_info "Iniciando pruebas..."
echo ""

if [ -f "$ENVIRONMENT_PATH" ]; then
    newman run "$COLLECTION_PATH" \
        --environment "$ENVIRONMENT_PATH" \
        $FOLDERS \
        --reporters cli,htmlextra,json \
        --reporter-htmlextra-export "$REPORT_DIR/${REPORT_NAME}.html" \
        --reporter-json-export "$REPORT_DIR/${REPORT_NAME}.json" \
        --timeout-request 10000 \
        --timeout-script 10000
else
    newman run "$COLLECTION_PATH" \
        $FOLDERS \
        --reporters cli,htmlextra,json \
        --reporter-htmlextra-export "$REPORT_DIR/${REPORT_NAME}.html" \
        --reporter-json-export "$REPORT_DIR/${REPORT_NAME}.json" \
        --timeout-request 10000 \
        --timeout-script 10000
fi

# Verificar resultado
if [ $? -eq 0 ]; then
    print_success "Todas las pruebas pasaron correctamente"
    print_info "Reporte HTML: $REPORT_DIR/${REPORT_NAME}.html"
    print_info "Reporte JSON: $REPORT_DIR/${REPORT_NAME}.json"
    exit 0
else
    print_error "Las pruebas tuvieron errores"
    print_info "Reporte HTML: $REPORT_DIR/${REPORT_NAME}.html"
    print_info "Reporte JSON: $REPORT_DIR/${REPORT_NAME}.json"
    exit 1
fi
