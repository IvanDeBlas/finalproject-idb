#!/usr/bin/env python3
"""Newman Runner for WePlay.CsMensajeria.IntegrationTests

Handles SQL setup that cannot be done via API:
  - Creates Artista record for the first user
  - Creates a NecesidadCrowdsourcing via SQL

Prerequisites:
  - Docker SQL Server running on localhost:1433
  - API running on localhost:5001
  - Newman: npx newman
"""

import json
import subprocess
import sys
import time
import uuid
import urllib.request

BASE_URL = "http://localhost:5001"
COLLECTION = "tests/integration/WePlay.CsMensajeria.IntegrationTests.postman_collection.json"


def api_post(path, data, token=None):
    body = json.dumps(data).encode("utf-8")
    headers = {"Content-Type": "application/json"}
    if token:
        headers["Authorization"] = f"Bearer {token}"
    req = urllib.request.Request(f"{BASE_URL}{path}", data=body, headers=headers, method="POST")
    try:
        with urllib.request.urlopen(req) as resp:
            return json.loads(resp.read().decode("utf-8"))
    except urllib.error.HTTPError as e:
        return json.loads(e.read().decode("utf-8"))


def run_sql(query):
    cmd = [
        "docker", "exec",
        "-e", "SQLCMDPASSWORD=WePlayRises2024!",
        "weplay_rises-sqlserver-1",
        "/opt/mssql-tools18/bin/sqlcmd",
        "-S", "localhost", "-U", "sa", "-d", "WePlayRises", "-C",
        "-Q", query, "-h", "-1"
    ]
    result = subprocess.run(cmd, capture_output=True, text=True)
    if result.returncode != 0:
        print(f"  SQL Error: {result.stderr.strip()}")
        return False
    return True


def main():
    print("=== CS Mensajeria Integration Tests ===\n")

    # Step 1: Register Artista User
    print("[1/5] Registering Artista user...")
    ts = int(time.time())
    artista_email = f"cs-msg-artista-{ts}@weplay.com"
    resp = api_post("/api/Auth/register", {
        "email": artista_email,
        "password": "TestPassword123",
        "confirmPassword": "TestPassword123",
        "nombre": "Artista Mensajeria"
    })
    artista_token = resp["data"]["token"]
    artista_user_id = resp["data"]["userId"]
    print(f"  Artista email: {artista_email}")
    print(f"  Artista userId: {artista_user_id}")

    # Step 2: Register Fan User
    print("\n[2/5] Registering Fan user...")
    fan_email = f"cs-msg-fan-{ts}@weplay.com"
    resp = api_post("/api/Auth/register", {
        "email": fan_email,
        "password": "TestPassword123",
        "confirmPassword": "TestPassword123",
        "nombre": "Fan Mensajeria"
    })
    fan_token = resp["data"]["token"]
    fan_user_id = resp["data"]["userId"]
    print(f"  Fan email: {fan_email}")
    print(f"  Fan userId: {fan_user_id}")

    # Step 3: Create Artista record via SQL
    print("\n[3/5] Creating Artista record via SQL...")
    artista_id = str(uuid.uuid4()).upper()
    ok = run_sql(
        f"INSERT INTO Artista (Id, UserId, NombreArtistico, Genero, Pais, Descripcion, ImagenPerfilUrl, FechaCreacion) "
        f"VALUES ('{artista_id}', '{artista_user_id}', 'Artista Newman Msg', 'Rock', 'ES', 'Test artist for messaging', NULL, GETUTCDATE())"
    )
    if ok:
        print(f"  Artista created: {artista_id}")
    else:
        print("  ERROR: Failed to create Artista")
        sys.exit(1)

    # Step 4: Create Necesidad via SQL
    print("\n[4/5] Creating Necesidad via SQL...")
    proyecto_id = str(uuid.uuid4()).upper()
    necesidad_id = str(uuid.uuid4()).upper()

    ok = run_sql(
        f"INSERT INTO ProyectoArtistico (Id, ArtistaId, Nombre, Descripcion, FechaCreacion) "
        f"VALUES ('{proyecto_id}', '{artista_id}', 'Proyecto Test Msg', 'Test project for messaging', GETUTCDATE())"
    )
    if ok:
        print(f"  ProyectoArtistico created: {proyecto_id}")
    else:
        print("  ERROR: Failed to create ProyectoArtistico")
        sys.exit(1)

    ok = run_sql(
        f"INSERT INTO NecesidadCrowdsourcing (Id, ProyectoArtistico_Id, Artista_Id, Titulo, Descripcion, "
        f"TipoNecesidad_Id, EstadoNecesidad_Id, ModalidadTrabajo_Id, PresupuestoMin, PresupuestoMax, Moneda_Id, FechaCreacion) "
        f"VALUES ('{necesidad_id}', '{proyecto_id}', '{artista_id}', 'Necesidad Test Mensajeria', "
        f"'Necesidad para tests de mensajeria', 1, 1, 3, 100.00, 500.00, 1, GETUTCDATE())"
    )
    if ok:
        print(f"  Necesidad created: {necesidad_id}")
    else:
        print("  ERROR: Failed to create Necesidad")
        sys.exit(1)

    # Step 5: Run Newman Collection
    print("\n[5/5] Running Newman collection...\n")

    newman_cmd = (
        f'npx newman run "{COLLECTION}"'
        f' --env-var "artistaToken={artista_token}"'
        f' --env-var "artistaUserId={artista_user_id}"'
        f' --env-var "fanToken={fan_token}"'
        f' --env-var "fanUserId={fan_user_id}"'
        f' --env-var "necesidadId={necesidad_id}"'
        f' --folder "Conversaciones - CRUD Lifecycle"'
        f' --folder "Mensajes - CRUD Lifecycle"'
        f' --folder "Conversaciones - Validation Errors"'
        f' --folder "Auth Errors"'
        f' --folder "Not Found"'
        f' --folder "Business Rules"'
        f' --folder "Pagination Tests"'
        f' --folder "Filters Tests"'
        f' --timeout-request 10000'
        f' --reporters cli'
    )

    result = subprocess.run(newman_cmd, shell=True)
    sys.exit(result.returncode)


if __name__ == "__main__":
    main()
