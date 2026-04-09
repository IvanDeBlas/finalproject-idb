#!/bin/bash
###############################################################################
# Newman Runner for WePlay.AcuerdosEntregables.IntegrationTests
#
# This script handles the PerfilProfesional SQL setup that cannot be done
# via API, then runs the Newman collection.
#
# Prerequisites:
#   - Docker SQL Server running on localhost:1433
#   - API running on localhost:5001
#   - sqlcmd available in PATH
#   - Newman: npx newman
#
# Usage:
#   ./run-acuerdos-newman.sh
###############################################################################

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
COLLECTION="$SCRIPT_DIR/WePlay.AcuerdosEntregables.IntegrationTests.postman_collection.json"
DB_SERVER="localhost,1433"
DB_USER="sa"
DB_PASS="WePlayRises2024!"
DB_NAME="WePlayRises"

echo "=== Step 1: Register Professional User ==="
PRO_EMAIL="ae-pro-$(date +%s)@weplay.com"
REGISTER_RESPONSE=$(curl -s -X POST "http://localhost:5001/api/Auth/register" \
  -H "Content-Type: application/json" \
  -d "{\"email\":\"$PRO_EMAIL\",\"password\":\"TestPassword123\",\"confirmPassword\":\"TestPassword123\",\"nombre\":\"AE Newman Pro\"}")

PRO_TOKEN=$(echo "$REGISTER_RESPONSE" | python3 -c "import sys,json; print(json.load(sys.stdin)['data']['token'])")
PRO_USER_ID=$(echo "$REGISTER_RESPONSE" | python3 -c "import sys,json; print(json.load(sys.stdin)['data']['userId'])")

echo "  Pro email: $PRO_EMAIL"
echo "  Pro userId: $PRO_USER_ID"

echo ""
echo "=== Step 2: Create PerfilProfesional via SQL ==="
PERFIL_ID=$(python3 -c "import uuid; print(str(uuid.uuid4()).upper())")
sqlcmd -S "$DB_SERVER" -U "$DB_USER" -P "$DB_PASS" -d "$DB_NAME" -Q "
INSERT INTO PerfilProfesional (Id, UserId, Titulo, Descripcion, FechaCreacion)
VALUES ('$PERFIL_ID', '$PRO_USER_ID', 'Newman Test Professional', 'Created by run-acuerdos-newman.sh', GETUTCDATE())
" -h -1
echo "  PerfilProfesional created: $PERFIL_ID"

echo ""
echo "=== Step 3: Run Newman Collection ==="
npx newman run "$COLLECTION" \
  --env-var "proToken=$PRO_TOKEN" \
  --env-var "proUserId=$PRO_USER_ID" \
  --reporters cli \
  "$@"
