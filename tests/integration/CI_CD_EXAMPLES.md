# CI/CD Integration Examples

Ejemplos de configuración para ejecutar Newman tests en pipelines de CI/CD.

## GitHub Actions

**Archivo:** `.github/workflows/integration-tests-campanias.yml`

```yaml
name: Integration Tests - Campanias API

on:
  push:
    branches: [master, develop]
    paths:
      - 'src/api/**'
      - 'tests/integration/**'
  pull_request:
    branches: [master, develop]
    paths:
      - 'src/api/**'
      - 'tests/integration/**'

jobs:
  integration-tests:
    runs-on: ubuntu-latest
    timeout-minutes: 10

    services:
      sqlserver:
        image: mcr.microsoft.com/mssql/server:2022-latest
        env:
          SA_PASSWORD: TestPassword123!@
          ACCEPT_EULA: Y
        options: >-
          --health-cmd "/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P TestPassword123!@ -Q 'select 1'"
          --health-interval 10s
          --health-timeout 5s
          --health-retries 5
        ports:
          - 1433:1433

    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'

      - name: Setup Node.js
        uses: actions/setup-node@v4
        with:
          node-version: '18'
          cache: 'npm'

      - name: Install Newman
        run: npm install -g newman newman-reporter-htmlextra

      - name: Restore NuGet packages
        run: dotnet restore src/api/WePlayRises.sln

      - name: Build Backend
        run: dotnet build src/api/WePlayRises.sln --configuration Release --no-restore

      - name: Apply Migrations
        working-directory: src/api/WebApi
        env:
          ASPNETCORE_ENVIRONMENT: Testing
          ConnectionStrings__DefaultConnection: "Server=localhost;Database=WePlayRises_Test;User Id=sa;Password=TestPassword123!@;Encrypt=false;"
        run: |
          dotnet ef database update -p ../WePlayRises.Crowdfunding.Infra/WePlayRises.Crowdfunding.Infra.csproj \
            --startup-project . --context WePlayRisesDbContext

      - name: Start API Server
        working-directory: src/api/WebApi
        env:
          ASPNETCORE_ENVIRONMENT: Testing
          ASPNETCORE_URLS: http://localhost:5000
          ConnectionStrings__DefaultConnection: "Server=localhost;Database=WePlayRises_Test;User Id=sa;Password=TestPassword123!@;Encrypt=false;"
        run: |
          dotnet run --configuration Release --no-build &
          sleep 15

      - name: Run Integration Tests
        working-directory: tests/integration
        run: |
          newman run WePlay.Campanias.IntegrationTests.postman_collection.json \
            -e environments/development.postman_environment.json \
            -r cli,junitxml,htmlextra \
            --reporter-junitxml-export ../test-results/campanias-results.xml \
            --reporter-htmlextra-export ../test-results/campanias-report.html \
            --delay-request 100

      - name: Upload Test Results
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: integration-test-results
          path: tests/test-results/
          retention-days: 30

      - name: Publish Test Results
        if: always()
        uses: dorny/test-reporter@v1
        with:
          name: Integration Tests Report
          path: 'tests/test-results/campanias-results.xml'
          reporter: 'java-junit'
          fail-on-error: true

      - name: Comment PR with Results
        if: always() && github.event_name == 'pull_request'
        uses: actions/github-script@v7
        with:
          script: |
            const fs = require('fs');
            const results = JSON.parse(fs.readFileSync('tests/test-results/campanias-results.json', 'utf8'));
            const stats = results.run.stats;

            const comment = `## Integration Tests Results

            | Metric | Value |
            |--------|-------|
            | Total Requests | ${stats.requests.total} |
            | Passed | ${stats.requests.total - stats.requests.failed} |
            | Failed | ${stats.requests.failed} |
            | Assertions | ${stats.assertions.total} |
            | Duration | ${(results.run.timings.completed / 1000).toFixed(2)}s |

            [View Full Report](https://github.com/${{ github.repository }}/actions/runs/${{ github.run_id }})`;

            github.rest.issues.createComment({
              issue_number: context.issue.number,
              owner: context.repo.owner,
              repo: context.repo.repo,
              body: comment
            });
```

## Azure DevOps

**Archivo:** `azure-pipelines.yml`

```yaml
trigger:
  branches:
    include:
      - master
      - develop
  paths:
    include:
      - src/api/**
      - tests/integration/**

pr:
  branches:
    include:
      - master
      - develop
  paths:
    include:
      - src/api/**
      - tests/integration/**

pool:
  vmImage: 'ubuntu-latest'

variables:
  buildConfiguration: 'Release'
  dotnetVersion: '8.0.x'
  nodeVersion: '18.x'

stages:
  - stage: Build
    displayName: 'Build and Test'
    jobs:
      - job: IntegrationTests
        displayName: 'Run Integration Tests'
        timeoutInMinutes: 15

        services:
          sqlserver:
            image: mcr.microsoft.com/mssql/server:2022-latest
            options: >-
              -e SA_PASSWORD=TestPassword123!@
              -e ACCEPT_EULA=Y
              -p 1433:1433
            env:
              SA_PASSWORD: TestPassword123!@
              ACCEPT_EULA: Y

        steps:
          - checkout: self
            fetchDepth: 0

          - task: UseDotNet@2
            displayName: 'Install .NET SDK'
            inputs:
              version: $(dotnetVersion)
              packageType: 'sdk'

          - task: UseNode@1
            displayName: 'Setup Node.js'
            inputs:
              version: $(nodeVersion)

          - task: Npm@1
            displayName: 'Install Newman'
            inputs:
              command: 'custom'
              customCommand: 'install -g newman newman-reporter-htmlextra'

          - task: DotNetCoreCLI@2
            displayName: 'Restore NuGet Packages'
            inputs:
              command: 'restore'
              projects: 'src/api/WePlayRises.sln'

          - task: DotNetCoreCLI@2
            displayName: 'Build Solution'
            inputs:
              command: 'build'
              projects: 'src/api/WePlayRises.sln'
              arguments: '--configuration $(buildConfiguration) --no-restore'

          - task: DotNetCoreCLI@2
            displayName: 'Apply Database Migrations'
            inputs:
              command: 'custom'
              custom: 'ef'
              arguments: 'database update --startup-project src/api/WebApi --project src/api/WePlayRises.Crowdfunding.Infra --context WePlayRisesDbContext'
            env:
              ASPNETCORE_ENVIRONMENT: Testing
              ConnectionStrings__DefaultConnection: 'Server=localhost;Database=WePlayRises_Test;User Id=sa;Password=TestPassword123!@;Encrypt=false;'

          - task: DotNetCoreCLI@2
            displayName: 'Start API Server'
            inputs:
              command: 'run'
              projects: 'src/api/WebApi/WePlayRises.WebApi.csproj'
              arguments: '--configuration $(buildConfiguration) --no-build'
            env:
              ASPNETCORE_ENVIRONMENT: Testing
              ASPNETCORE_URLS: http://localhost:5000
              ConnectionStrings__DefaultConnection: 'Server=localhost;Database=WePlayRises_Test;User Id=sa;Password=TestPassword123!@;Encrypt=false;'
            continueOnError: false

          - bash: 'sleep 15'
            displayName: 'Wait for API to start'

          - bash: |
              cd tests/integration
              newman run WePlay.Campanias.IntegrationTests.postman_collection.json \
                -e environments/development.postman_environment.json \
                -r cli,junitxml,htmlextra \
                --reporter-junitxml-export ../test-results/campanias-results.xml \
                --reporter-htmlextra-export ../test-results/campanias-report.html \
                --delay-request 100
            displayName: 'Run Newman Integration Tests'
            continueOnError: true

          - task: PublishTestResults@2
            displayName: 'Publish Test Results'
            condition: always()
            inputs:
              testResultsFormat: 'JUnit'
              testResultsFiles: 'tests/test-results/campanias-results.xml'
              mergeTestResults: true
              failTaskOnFailedTests: true

          - task: PublishBuildArtifacts@1
            displayName: 'Publish Artifacts'
            condition: always()
            inputs:
              pathToPublish: 'tests/test-results'
              artifactName: 'integration-test-results'
              publishLocation: 'Container'
```

## GitLab CI/CD

**Archivo:** `.gitlab-ci.yml`

```yaml
stages:
  - build
  - test
  - report

variables:
  DOTNET_VERSION: '8.0'
  NODE_VERSION: '18'
  DOCKER_DRIVER: overlay2

integration-tests:
  stage: test
  image: mcr.microsoft.com/dotnet/sdk:8.0
  services:
    - name: mcr.microsoft.com/mssql/server:2022-latest
      alias: mssql
      variables:
        SA_PASSWORD: TestPassword123!@
        ACCEPT_EULA: 'Y'
  variables:
    ASPNETCORE_ENVIRONMENT: Testing
    ASPNETCORE_URLS: http://localhost:5000
    ConnectionStrings__DefaultConnection: 'Server=mssql;Database=WePlayRises_Test;User Id=sa;Password=TestPassword123!@;Encrypt=false;'
  before_script:
    - curl -fsSL https://deb.nodesource.com/setup_18.x | bash -
    - apt-get update && apt-get install -y nodejs
    - npm install -g newman newman-reporter-htmlextra
  script:
    - dotnet restore src/api/WePlayRises.sln
    - dotnet build src/api/WePlayRises.sln --configuration Release --no-restore
    - cd src/api/WebApi && dotnet ef database update --startup-project . && cd ../../..
    - dotnet run --project src/api/WebApi/WePlayRises.WebApi.csproj --configuration Release --no-build &
    - sleep 15
    - cd tests/integration
    - newman run WePlay.Campanias.IntegrationTests.postman_collection.json
        -e environments/development.postman_environment.json
        -r cli,junitxml,htmlextra
        --reporter-junitxml-export ../test-results/results.xml
        --reporter-htmlextra-export ../test-results/report.html
        --delay-request 100
  artifacts:
    when: always
    paths:
      - tests/test-results/
    reports:
      junit: tests/test-results/results.xml
  retry:
    max: 2
    when:
      - api_failure
      - runner_system_failure
```

## Jenkins

**Archivo:** `Jenkinsfile`

```groovy
pipeline {
    agent any

    triggers {
        pollSCM('H H * * *')
    }

    environment {
        DOTNET_SKIP_FIRST_TIME_EXPERIENCE = '1'
        DOTNET_CLI_TELEMETRY_OPTOUT = '1'
        ASPNETCORE_ENVIRONMENT = 'Testing'
        ASPNETCORE_URLS = 'http://localhost:5000'
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Setup') {
            steps {
                script {
                    sh '''
                        echo "Installing .NET SDK..."
                        # Assuming .NET is already installed on Jenkins agent

                        echo "Installing Node.js dependencies..."
                        npm install -g newman newman-reporter-htmlextra

                        echo "Environment setup complete"
                    '''
                }
            }
        }

        stage('Build') {
            steps {
                script {
                    sh '''
                        echo "Restoring NuGet packages..."
                        dotnet restore src/api/WePlayRises.sln

                        echo "Building solution..."
                        dotnet build src/api/WePlayRises.sln --configuration Release --no-restore
                    '''
                }
            }
        }

        stage('Database Migration') {
            steps {
                script {
                    sh '''
                        echo "Applying database migrations..."
                        cd src/api/WebApi
                        dotnet ef database update --startup-project . \\
                            --project ../WePlayRises.Crowdfunding.Infra \\
                            --context WePlayRisesDbContext
                        cd ../../..
                    '''
                }
            }
        }

        stage('Start API') {
            steps {
                script {
                    sh '''
                        echo "Starting API server..."
                        dotnet run --project src/api/WebApi/WePlayRises.WebApi.csproj \\
                            --configuration Release --no-build &

                        echo "Waiting for API to start..."
                        sleep 15
                    '''
                }
            }
        }

        stage('Integration Tests') {
            steps {
                script {
                    sh '''
                        cd tests/integration

                        echo "Running Newman integration tests..."
                        newman run WePlay.Campanias.IntegrationTests.postman_collection.json \\
                            -e environments/development.postman_environment.json \\
                            -r cli,junitxml,htmlextra \\
                            --reporter-junitxml-export ../test-results/results.xml \\
                            --reporter-htmlextra-export ../test-results/report.html \\
                            --delay-request 100
                    '''
                }
            }
        }
    }

    post {
        always {
            junit 'tests/test-results/results.xml'

            publishHTML([
                allowMissing: false,
                alwaysLinkToLastBuild: true,
                keepAll: true,
                reportDir: 'tests/test-results',
                reportFiles: 'report.html',
                reportName: 'Newman Integration Test Report'
            ])

            archiveArtifacts artifacts: 'tests/test-results/**', allowEmptyArchive: true
        }

        success {
            echo 'Integration tests passed!'
        }

        failure {
            echo 'Integration tests failed!'
            currentBuild.result = 'FAILURE'
        }
    }
}
```

## Docker Compose (Local Development)

**Archivo:** `docker-compose.test.yml`

```yaml
version: '3.8'

services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      SA_PASSWORD: TestPassword123!@
      ACCEPT_EULA: 'Y'
    ports:
      - "1433:1433"
    healthcheck:
      test: /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P TestPassword123!@ -Q "select 1"
      interval: 10s
      timeout: 5s
      retries: 5

  api:
    build:
      context: .
      dockerfile: src/api/Dockerfile
    environment:
      ASPNETCORE_ENVIRONMENT: Testing
      ASPNETCORE_URLS: http://0.0.0.0:5000
      ConnectionStrings__DefaultConnection: "Server=sqlserver;Database=WePlayRises_Test;User Id=sa;Password=TestPassword123!@;Encrypt=false;"
    ports:
      - "5000:5000"
    depends_on:
      sqlserver:
        condition: service_healthy
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:5000/api/health"]
      interval: 10s
      timeout: 5s
      retries: 5

  newman:
    image: postman/newman:latest
    working_dir: /test
    volumes:
      - ./tests/integration:/test
    command: >
      run WePlay.Campanias.IntegrationTests.postman_collection.json
      -e environments/development.postman_environment.json
      -r cli,htmlextra
      --reporter-htmlextra-export test-results/report.html
      --delay-request 100
    depends_on:
      api:
        condition: service_healthy
    ports:
      - "8080:8080"
```

## Makefile (Local Development)

**Archivo:** `Makefile`

```makefile
.PHONY: test-install test-setup test-run test-e2e test-all clean

# Install test dependencies
test-install:
	npm install -g newman newman-reporter-htmlextra

# Setup test environment
test-setup:
	mkdir -p tests/test-results

# Run all integration tests
test-run: test-setup
	cd tests/integration && \
	./run-tests.sh

# Run E2E happy path
test-e2e: test-setup
	cd tests/integration && \
	./run-tests.sh -f "Campanias/E2E Happy Path"

# Run setup only
test-setup-only:
	cd tests/integration && \
	./run-tests.sh -f "_Setup"

# Run validation tests
test-validation:
	cd tests/integration && \
	./run-tests.sh -f "Campanias/400 BAD REQUEST"

# Run with JUnit reporter for CI
test-junit: test-setup
	cd tests/integration && \
	./run-tests.sh -r "cli,junitxml"

# Run all tests
test-all: test-install test-run

# Clean test artifacts
clean:
	rm -rf tests/test-results/
	rm -f final-environment.json

help:
	@echo "Available targets:"
	@echo "  make test-install    - Install Newman globally"
	@echo "  make test-setup      - Create test directories"
	@echo "  make test-run        - Run all integration tests"
	@echo "  make test-e2e        - Run E2E happy path"
	@echo "  make test-validation - Run validation tests"
	@echo "  make test-junit      - Run with JUnit reporter"
	@echo "  make test-all        - Install and run all tests"
	@echo "  make clean           - Remove test artifacts"
```

---

**Versión:** 1.0
**Fecha:** 2026-02-12
