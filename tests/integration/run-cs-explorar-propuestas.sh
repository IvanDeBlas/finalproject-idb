#!/bin/bash
# Run cs-explorar-propuestas integration tests
# Prerequisites: Docker containers running, Newman installed

set -e

COLLECTION="tests/integration/WePlay.CrowdsourcingExplorarPropuestas.IntegrationTests.postman_collection.json"
BASE_URL="http://localhost:5001"
SQLCMD="docker exec weplay_rises-sqlserver-1 /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P WePlayRises2024! -d WePlayRises -C"

echo "=== CS Explorar Propuestas Integration Tests ==="
echo ""

# Step 1: Register professional user and create PerfilProfesional via SQL
echo "[1/3] Creating professional user and profile..."

# Register user via API
PRO_EMAIL="cs-pro-$(date +%s)@weplay.com"
REGISTER_RESPONSE=$(curl -s -X POST "$BASE_URL/api/Auth/register" \
  -H "Content-Type: application/json" \
  -d "{\"email\":\"$PRO_EMAIL\",\"password\":\"TestPassword123\",\"confirmPassword\":\"TestPassword123\",\"nombre\":\"CS Pro User\"}")

PRO_TOKEN=$(echo "$REGISTER_RESPONSE" | python3 -c "import json,sys; print(json.load(sys.stdin)['data']['token'])")
PRO_USER_ID=$(echo "$REGISTER_RESPONSE" | python3 -c "import json,sys; print(json.load(sys.stdin)['data']['userId'])")

echo "  Professional registered: $PRO_EMAIL (userId: $PRO_USER_ID)"

# Insert PerfilProfesional via SQL
PERFIL_ID=$(python3 -c "import uuid; print(str(uuid.uuid4()).upper())")
MSYS_NO_PATHCONV=1 $SQLCMD -Q "INSERT INTO PerfilProfesional (Id, UserId, Titulo, Descripcion, FechaCreacion) VALUES ('$PERFIL_ID', '$PRO_USER_ID', 'Test Professional', 'Integration test profile', GETUTCDATE())" > /dev/null 2>&1

echo "  PerfilProfesional created: $PERFIL_ID"

# Step 2: Run Newman with pre-set variables
echo ""
echo "[2/3] Running Newman collection..."
echo ""

npx newman run "$COLLECTION" \
  --timeout-request 10000 \
  --env-var "proToken=$PRO_TOKEN" \
  --env-var "proUserId=$PRO_USER_ID"

echo ""
echo "[3/3] Done!"
