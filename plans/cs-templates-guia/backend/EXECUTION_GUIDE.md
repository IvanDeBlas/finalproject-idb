# Quick Execution Guide - cs-templates-guia Postman Collection

## ⚡ 30-Second Setup

### Prerequisites
```bash
# 1. Backend running
cd src/api
dotnet run --launch-profile https

# 2. Database seeded (Docker)
docker compose up -d

# 3. Newman installed
npm install -g newman
```

### Run Tests
```bash
newman run plans/cs-templates-guia/backend/postman-collection.json \
  --reporters cli,htmlextra
```

## 📊 What Gets Tested

| Category | Requests | Validates |
|----------|----------|-----------|
| **Setup** | 1 | Login & token acquisition |
| **Happy Path** | 4 | List templates, detail, roles, categories |
| **Validation** | 3 | Empty needs, budget range, negative values |
| **Auth** | 2 | Missing token, invalid token |
| **Not Found** | 2 | Invalid IDs, missing templates |
| **Total** | **12** | **~60+ assertions** |

## 🎯 Test Execution Flow

```
1. _Setup
   └─ Login as usuario1@mail.com → Save JWT token

2. Templates - CRUD Lifecycle (Read-only endpoints)
   ├─ GET /api/crowdsourcing/templates
   ├─ GET /api/crowdsourcing/templates/{id}
   ├─ GET /api/crowdsourcing/maestras/roles-profesionales
   └─ GET /api/crowdsourcing/maestras/categorias-rol

3. Templates - Validation Errors (POST with bad data)
   ├─ Empty necesidadesSeleccionadas → 400
   ├─ Presupuesto max < min → 400
   └─ Presupuesto min < 0 → 400

4. Templates - Auth Errors
   ├─ Missing Authorization header → 401
   └─ Invalid token → 401

5. Templates - Not Found
   ├─ Invalid template ID → 404
   └─ Template not found in POST → 404
```

## 🔍 Key Test Data

### Test User (Pre-seeded in Docker DB)
- Email: `usuario1@mail.com`
- Password: `123456`
- Role: Fan (with Artista profile)

### Test Endpoints
| Method | Path | Auth | Status |
|--------|------|------|--------|
| GET | /api/crowdsourcing/templates | Bearer JWT | 200 |
| GET | /api/crowdsourcing/templates/{id} | Bearer JWT | 200 |
| GET | /api/crowdsourcing/maestras/roles-profesionales | Bearer JWT | 200 |
| GET | /api/crowdsourcing/maestras/categorias-rol | Bearer JWT | 200 |
| POST | /api/crowdsourcing/templates/{id}/generar | Bearer JWT | 201/400/404 |

## 📈 Expected Results

### ✅ All Tests Pass
```
┌─────────────────────────────────────────┐
│  WePlay.cs-templates-guia.IntegrationTests  │
├─────────────────────────────────────────┤
│ ✓ 12 requests run                      │
│ ✓ 0 failed                             │
│ ✓ ~60 assertions passed                │
│ ✓ Runtime: ~3 seconds                  │
└─────────────────────────────────────────┘
```

### ❌ If Tests Fail

1. **401 Unauthorized** → User/token issue
   - Check: `docker exec mssql-server sqlcmd -S localhost -U sa -P WePlayRises2024! -Q "SELECT COUNT(*) FROM AspNetUsers WHERE Email='usuario1@mail.com'"`

2. **404 Not Found** → Seed data missing
   - Check: `docker exec mssql-server sqlcmd -S localhost -U sa -P WePlayRises2024! -Q "SELECT COUNT(*) FROM PlantillaProyecto"`

3. **500 Internal Error** → API logs
   - Check: `dotnet run` console output

## 🚀 Advanced Options

### With HTML Report
```bash
newman run plans/cs-templates-guia/backend/postman-collection.json \
  --reporters cli,htmlextra \
  --reporter-htmlextra-export test-report.html
```
Then open `test-report.html` in browser.

### With Custom Base URL
```bash
newman run plans/cs-templates-guia/backend/postman-collection.json \
  --global-var "baseUrl=http://your-api.com"
```

### In Postman GUI
1. Import: `plans/cs-templates-guia/backend/postman-collection.json`
2. Run: Click `_Setup` folder → **Run Folder**
3. View: Each request shows assertion results

### For CI/CD Pipeline
```bash
newman run plans/cs-templates-guia/backend/postman-collection.json \
  --reporters json \
  --reporter-json-export results.json \
  --exit-code
```

## 🔐 Collection Variables

Automatically populated during execution:

| Variable | Set By | Used For |
|----------|--------|----------|
| `baseUrl` | Static (http://localhost:5001) | All requests |
| `accessToken` | _Setup Login | All authenticated requests |
| `templateId` | Request 01 (GET /templates) | Request 02, validation tests |
| `necesidadId` | Request 02 (GET /templates/{id}) | Validation tests |

**Manual Override:**
```json
// Create env-file.json
{
  "baseUrl": "http://localhost:5001",
  "accessToken": "your-token-here"
}

// Use it:
// newman run ... --environment env-file.json
```

## 🛠️ Troubleshooting Checklist

- [ ] API is running on http://localhost:5001
- [ ] Database is seeded with test user
- [ ] Newman is installed globally: `newman --version`
- [ ] Collection file exists: `plans/cs-templates-guia/backend/postman-collection.json`
- [ ] No firewall blocking localhost:5001
- [ ] Test user exists in database
- [ ] JWT secret key matches API configuration

## 📞 Support

1. **API won't start?**
   - Check .NET version: `dotnet --version` (should be 8.x)
   - Check ports: `netstat -ano | findstr :5001`

2. **Database errors?**
   - Check SQL Server: `docker ps | grep mssql`
   - Check migrations: `dotnet ef database update --project src/api`

3. **Test failures?**
   - Enable Postman console: Settings → Console → Show Newman Console
   - Check API logs in terminal
   - Verify seed data: SQL Server Management Studio

## 📚 Documentation Files

- **Main README:** `POSTMAN_COLLECTION_README.md` - Comprehensive guide
- **This File:** `EXECUTION_GUIDE.md` - Quick reference
- **Collection JSON:** `postman-collection.json` - Importable collection
- **Contracts:** `../../docs/user-stories/cs-templates-guia/contracts.md` - API specs
- **Feature Spec:** `../../docs/user-stories/cs-templates-guia/feature-spec.md` - Feature details

## ⏱️ Timeline

```
Estimated Execution Time by Environment:

Local Development:    ~3-5 seconds
CI/CD Pipeline:      ~10-15 seconds (with setup)
Cloud Environment:   ~15-30 seconds (network latency)
```

## 🎓 Learning Resources

- **Postman Scripting:** https://learning.postman.com/docs/writing-scripts/test-scripts/
- **Newman CLI:** https://github.com/postmanlabs/newman
- **HTTP Status Codes:** https://httpwg.org/specs/rfc9110.html
- **ServiceResponse Pattern:** `.claude/rules/backend/cqrs.rule.md`

---

**Ready to test?** Run this:
```bash
newman run plans/cs-templates-guia/backend/postman-collection.json
```

Good luck! 🚀
