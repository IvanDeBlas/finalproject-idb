#!/bin/bash

# Script para ejecutar tests de Dashboard Artista con Newman
# Uso: ./run-tests.sh [environment] [reporters]

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
COLLECTION="$SCRIPT_DIR/postman-collection.json"
ENVIRONMENT="${1:-environment.json}"
ENVIRONMENT_PATH="$SCRIPT_DIR/$ENVIRONMENT"
REPORTERS="${2:-cli}"
TIMESTAMP=$(date +%Y%m%d_%H%M%S)
REPORT_DIR="$SCRIPT_DIR/reports"

# Validar que la coleccion existe
if [ ! -f "$COLLECTION" ]; then
    echo "Error: Coleccion no encontrada en $COLLECTION"
    exit 1
fi

# Validar que el environment existe
if [ ! -f "$ENVIRONMENT_PATH" ]; then
    echo "Error: Environment no encontrado en $ENVIRONMENT_PATH"
    echo "Ambientes disponibles:"
    ls -1 "$SCRIPT_DIR"/environment*.json
    exit 1
fi

# Crear directorio de reportes si no existe
mkdir -p "$REPORT_DIR"

echo "=================================================="
echo "Dashboard Artista - Integration Tests"
echo "=================================================="
echo "Ambiente: $ENVIRONMENT"
echo "Reporteros: $REPORTERS"
echo "Timestamp: $TIMESTAMP"
echo ""

# Ejecutar tests
if [[ "$REPORTERS" == *"html"* ]]; then
    newman run "$COLLECTION" \
        --environment "$ENVIRONMENT_PATH" \
        --reporters cli,html \
        --reporter-html-export "$REPORT_DIR/report_$TIMESTAMP.html" \
        --reporter-html-template "$SCRIPT_DIR/html-template.hbs" \
        --bail on
else
    newman run "$COLLECTION" \
        --environment "$ENVIRONMENT_PATH" \
        --reporters "$REPORTERS" \
        --bail on
fi

EXIT_CODE=$?

echo ""
echo "=================================================="
if [ $EXIT_CODE -eq 0 ]; then
    echo "Tests PASSED!"
else
    echo "Tests FAILED!"
fi
echo "=================================================="

if [ -d "$REPORT_DIR" ] && [ $EXIT_CODE -eq 0 ]; then
    echo "Reportes generados en: $REPORT_DIR"
    ls -lh "$REPORT_DIR"
fi

exit $EXIT_CODE
