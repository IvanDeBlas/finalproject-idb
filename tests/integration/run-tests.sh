#!/bin/bash

# WePlay Rises - Newman Integration Tests Runner
# Script para ejecutar pruebas de integración de Campanias API

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Default values
FOLDER=""
REPORTER="cli,htmlextra"
EXPORT_ENV=""
DELAY=100

# Function to print messages
print_info() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

print_warn() {
    echo -e "${YELLOW}[WARN]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Function to print usage
print_usage() {
    echo "Usage: ./run-tests.sh [OPTIONS]"
    echo ""
    echo "Options:"
    echo "  -f, --folder FOLDER      Run only tests in specified folder"
    echo "                          Examples: '_Setup', 'Campanias/201 CREATED', 'Campanias/E2E Happy Path'"
    echo "  -r, --reporters REPORTERS Reporters to use (default: cli,htmlextra)"
    echo "                          Options: cli, json, junitxml, htmlextra"
    echo "  -e, --export FILE        Export final environment variables to file"
    echo "  -d, --delay MS           Delay between requests in milliseconds (default: 100)"
    echo "  -h, --help               Show this help message"
    echo ""
    echo "Examples:"
    echo "  # Run all tests with default reporters"
    echo "  ./run-tests.sh"
    echo ""
    echo "  # Run only setup"
    echo "  ./run-tests.sh -f '_Setup'"
    echo ""
    echo "  # Run only E2E happy path"
    echo "  ./run-tests.sh -f 'Campanias/E2E Happy Path'"
    echo ""
    echo "  # Run CREATE tests and export environment"
    echo "  ./run-tests.sh -f 'Campanias/201 CREATED' -e final-env.json"
    echo ""
    echo "  # Run with JUnit reporter for CI/CD"
    echo "  ./run-tests.sh -r 'cli,junitxml' --delay 200"
}

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        -f|--folder)
            FOLDER="$2"
            shift 2
            ;;
        -r|--reporters)
            REPORTER="$2"
            shift 2
            ;;
        -e|--export)
            EXPORT_ENV="$2"
            shift 2
            ;;
        -d|--delay)
            DELAY="$2"
            shift 2
            ;;
        -h|--help)
            print_usage
            exit 0
            ;;
        *)
            print_error "Unknown option: $1"
            print_usage
            exit 1
            ;;
    esac
done

# Check if newman is installed
if ! command -v newman &> /dev/null; then
    print_error "Newman is not installed"
    echo "Install it with: npm install -g newman newman-reporter-htmlextra"
    exit 1
fi

# Create test-results directory if it doesn't exist
mkdir -p test-results

# Build newman command
NEWMAN_CMD="newman run WePlay.Campanias.IntegrationTests.postman_collection.json \
    -e environments/development.postman_environment.json \
    -r $REPORTER \
    --delay-request $DELAY"

# Add folder filter if specified
if [ -n "$FOLDER" ]; then
    NEWMAN_CMD="$NEWMAN_CMD --folder '$FOLDER'"
    print_info "Running tests in folder: $FOLDER"
fi

# Add HTML extra reporter options if htmlextra is in reporters
if [[ "$REPORTER" == *"htmlextra"* ]]; then
    NEWMAN_CMD="$NEWMAN_CMD --reporter-htmlextra-export test-results/report.html"
fi

# Add JUnit reporter options if junitxml is in reporters
if [[ "$REPORTER" == *"junitxml"* ]]; then
    NEWMAN_CMD="$NEWMAN_CMD --reporter-junitxml-export test-results/results.xml"
fi

# Add environment export if specified
if [ -n "$EXPORT_ENV" ]; then
    NEWMAN_CMD="$NEWMAN_CMD --export-environment '$EXPORT_ENV'"
    print_info "Final environment will be exported to: $EXPORT_ENV"
fi

# Print execution info
print_info "Newman Integration Tests Runner"
print_info "================================"
echo ""
print_info "Collection: WePlay.Campanias.IntegrationTests"
print_info "Environment: development"
print_info "Reporters: $REPORTER"
print_info "Request Delay: ${DELAY}ms"
echo ""

# Execute newman
print_info "Starting test execution..."
echo ""

eval $NEWMAN_CMD

# Check exit code
if [ $? -eq 0 ]; then
    echo ""
    print_info "Tests completed successfully!"
    if [[ "$REPORTER" == *"htmlextra"* ]]; then
        print_info "HTML Report: test-results/report.html"
    fi
    if [[ "$REPORTER" == *"junitxml"* ]]; then
        print_info "JUnit Report: test-results/results.xml"
    fi
else
    echo ""
    print_error "Tests failed!"
    exit 1
fi
