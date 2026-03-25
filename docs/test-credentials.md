# Credenciales de Prueba - WePlay Rises

Usuarios creados por `scripts/seed-complete.sql`.

---

## Artistas

| Email | Password | Nombre Artistico | Roles | Notas |
|-------|----------|------------------|-------|-------|
| vetusta@weplay-test.com | WePlay2026! | Vetusta Morla | Admin, Fan, Artista | Tiene rol Admin |
| rosalia@weplay-test.com | WePlay2026! | Rosalia | Fan, Artista | |
| tangana@weplay-test.com | WePlay2026! | C. Tangana | Fan, Artista | |
| badbunny@weplay-test.com | WePlay2026! | Bad Bunny | Fan, Artista | |
| billie@weplay-test.com | WePlay2026! | Billie Eilish | Fan, Artista | |
| arctic@weplay-test.com | WePlay2026! | Arctic Monkeys | Fan, Artista | |
| muse@weplay-test.com | WePlay2026! | Muse | Fan, Artista | |
| arcade@weplay-test.com | WePlay2026! | Arcade Fire | Fan, Artista | |

## Fans

| Email | Password | Nombre | Perfil Profesional |
|-------|----------|--------|--------------------|
| maria.garcia@weplay-test.com | Test123! | Maria Garcia | No |
| carlos.lopez@weplay-test.com | Test123! | Carlos Lopez | Si (Productor musical) |
| ana.martinez@weplay-test.com | Test123! | Ana Martinez | Si (Disenadora grafica) |
| pablo.ruiz@weplay-test.com | Test123! | Pablo Ruiz | No |
| emma.wilson@weplay-test.com | Test123! | Emma Wilson | Si (Ingeniera de mezcla) |

---

## Seed Script

Para repoblar la base de datos (local o Azure):

```bash
# Docker local
cat scripts/seed-complete.sql | docker compose exec -T sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'WePlayRises2024!' -d WePlayRises -C

# Azure SQL
sqlcmd -S <server>.database.windows.net -U <user> -P <password> -d weplay-rises_db -i scripts/seed-complete.sql
```

## Acceso

| App | URL |
|-----|-----|
| Landing | http://localhost:3000 |
| Admin Dashboard | http://localhost:3001 |
| API Swagger | http://localhost:5001/swagger |
