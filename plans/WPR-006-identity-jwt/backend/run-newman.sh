#!/bin/bash

# Newman Test Runner para WPR-006 Identity JWT Tests
# Script para ejecutar la coleccion Postman con Newman

set -e

SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
COLLECTION_FILE="${SCRIPT_DIR}/postman-collection.json"
ENVIRONMENT_FILE="${SCRIPT_DIR}/environment-development.json"
REPORTS_DIR="${SCRIPT_DIR}/../../reports"
TIMESTAMP=$(date +%Y%m%d-%H%M%S)

# Crear directorio de reportes
mkdir -p "${REPORTS_DIR}"

echo "======================================================"
echo "WePlay WPR006 - Identity JWT Integration Tests"
echo "======================================================"
echo ""
echo "Collection:  ${COLLECTION_FILE}"
echo "Environment: ${ENVIRONMENT_FILE}"
echo "Reports:     ${REPORTS_DIR}"
echo ""

# Verificar que newman esta instalado
if ! command -v newman &> /dev/null; then
    echo "ERROR: Newman no esta instalado"
    echo "Instalar con: npm install -g newman newman-reporter-htmlextra"
    exit 1
fi

echo "Starting tests..."
echo ""

# Ejecutar tests
newman run "${COLLECTION_FILE}" \
    -e "${ENVIRONMENT_FILE}" \
    --reporters cli,htmlextra,junitxml \
    --reporter-htmlextra-export "${REPORTS_DIR}/wpr006-identity-tests-${TIMESTAMP}.html" \
    --reporter-junitxml-export "${REPORTS_DIR}/wpr006-identity-tests-${TIMESTAMP}.xml" \
    --reporter-cli \
    --timeout 10000 \
    --timeout-request 5000 \
    --delay 100 \
    --insecure

TEST_RESULT=$?

echo ""
echo "======================================================"
if [ $TEST_RESULT -eq 0 ]; then
    echo "Test Suite: PASSED"
else
    echo "Test Suite: FAILED"
fi
echo "======================================================"
echo ""
echo "Reports generated:"
echo "  - HTML:  ${REPORTS_DIR}/wpr006-identity-tests-${TIMESTAMP}.html"
echo "  - JUnit: ${REPORTS_DIR}/wpr006-identity-tests-${TIMESTAMP}.xml"
echo ""

exit $TEST_RESULT
