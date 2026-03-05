#!/bin/bash
###############################################################################
# Newman Runner for WePlay.CsMensajeria.IntegrationTests
#
# This script handles SQL setup that cannot be done via API:
#   - Creates Artista record for the first user
#   - Creates a NecesidadCrowdsourcing via SQL
#
# Prerequisites:
#   - Docker SQL Server running on localhost:1433
#   - API running on localhost:5001
#   - Newman: npx newman
#
# Usage:
#   ./run-cs-mensajeria.sh
###############################################################################

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
COLLECTION="$SCRIPT_DIR/WePlay.CsMensajeria.IntegrationTests.postman_collection.json"
BASE_URL="http://localhost:5001"

# Use SQLCMDPASSWORD env var to avoid shell escaping issues with '!' in password
run_sql() {
    MSYS_NO_PATHCONV=1 docker exec -e SQLCMDPASSWORD='WePlayRises2024!' weplay_rises-sqlserver-1 \
        /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -d WePlayRises -C -Q "$1" -h -1
}

echo "=== CS Mensajeria Integration Tests ==="
echo ""

# Step 1: Register Artista User
echo "[1/5] Registering Artista user..."
ARTISTA_EMAIL="cs-msg-artista-$(date +%s)@weplay.com"
ARTISTA_RESPONSE=$(curl -s -X POST "$BASE_URL/api/Auth/register" \
  -H "Content-Type: application/json" \
  -d "{\"email\":\"$ARTISTA_EMAIL\",\"password\":\"TestPassword123\",\"confirmPassword\":\"TestPassword123\",\"nombre\":\"Artista Mensajeria\"}")

ARTISTA_TOKEN=$(echo "$ARTISTA_RESPONSE" | python3 -c "import json,sys; print(json.load(sys.stdin)['data']['token'])")
ARTISTA_USER_ID=$(echo "$ARTISTA_RESPONSE" | python3 -c "import json,sys; print(json.load(sys.stdin)['data']['userId'])")

echo "  Artista email: $ARTISTA_EMAIL"
echo "  Artista userId: $ARTISTA_USER_ID"

# Step 2: Register Fan User
echo ""
echo "[2/5] Registering Fan user..."
FAN_EMAIL="cs-msg-fan-$(date +%s)@weplay.com"
FAN_RESPONSE=$(curl -s -X POST "$BASE_URL/api/Auth/register" \
  -H "Content-Type: application/json" \
  -d "{\"email\":\"$FAN_EMAIL\",\"password\":\"TestPassword123\",\"confirmPassword\":\"TestPassword123\",\"nombre\":\"Fan Mensajeria\"}")

FAN_TOKEN=$(echo "$FAN_RESPONSE" | python3 -c "import json,sys; print(json.load(sys.stdin)['data']['token'])")
FAN_USER_ID=$(echo "$FAN_RESPONSE" | python3 -c "import json,sys; print(json.load(sys.stdin)['data']['userId'])")

echo "  Fan email: $FAN_EMAIL"
echo "  Fan userId: $FAN_USER_ID"

# Step 3: Create Artista record via SQL
echo ""
echo "[3/5] Creating Artista record via SQL..."
ARTISTA_ID=$(python3 -c "import uuid; print(str(uuid.uuid4()).upper())")
run_sql "INSERT INTO Artista (Id, UserId, NombreArtistico, Genero, Pais, Descripcion, ImagenPerfilUrl, FechaCreacion) VALUES ('$ARTISTA_ID', '$ARTISTA_USER_ID', 'Artista Newman Msg', 'Rock', 'ES', 'Test artist for messaging', NULL, GETUTCDATE())" > /dev/null 2>&1
echo "  Artista created: $ARTISTA_ID"

# Step 4: Create NecesidadCrowdsourcing via SQL
echo ""
echo "[4/5] Creating Necesidad via SQL..."
NECESIDAD_ID=$(python3 -c "import uuid; print(str(uuid.uuid4()).upper())")
PROYECTO_ID=$(python3 -c "import uuid; print(str(uuid.uuid4()).upper())")

# First create ProyectoArtistico
run_sql "INSERT INTO ProyectoArtistico (Id, ArtistaId, Nombre, Descripcion, FechaCreacion) VALUES ('$PROYECTO_ID', '$ARTISTA_ID', 'Proyecto Test Msg', 'Test project for messaging', GETUTCDATE())" > /dev/null 2>&1
echo "  ProyectoArtistico created: $PROYECTO_ID"

run_sql "INSERT INTO NecesidadCrowdsourcing (Id, ProyectoArtistico_Id, Artista_Id, Titulo, Descripcion, TipoNecesidad_Id, EstadoNecesidad_Id, ModalidadTrabajo_Id, PresupuestoMin, PresupuestoMax, Moneda_Id, FechaCreacion) VALUES ('$NECESIDAD_ID', '$PROYECTO_ID', '$ARTISTA_ID', 'Necesidad Test Mensajeria', 'Necesidad para tests de mensajeria', 1, 1, 3, 100.00, 500.00, 1, GETUTCDATE())" > /dev/null 2>&1
echo "  Necesidad created: $NECESIDAD_ID"

# Step 5: Run Newman Collection
echo ""
echo "[5/5] Running Newman collection..."
echo ""

npx newman run "$COLLECTION" \
  --env-var "artistaToken=$ARTISTA_TOKEN" \
  --env-var "artistaUserId=$ARTISTA_USER_ID" \
  --env-var "fanToken=$FAN_TOKEN" \
  --env-var "fanUserId=$FAN_USER_ID" \
  --env-var "necesidadId=$NECESIDAD_ID" \
  --folder "Conversaciones - CRUD Lifecycle" \
  --folder "Mensajes - CRUD Lifecycle" \
  --folder "Conversaciones - Validation Errors" \
  --folder "Auth Errors" \
  --folder "Not Found" \
  --folder "Business Rules" \
  --folder "Pagination Tests" \
  --folder "Filters Tests" \
  --timeout-request 10000 \
  --reporters cli \
  "$@"
