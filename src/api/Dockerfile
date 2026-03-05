FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY WePlayRises.sln .
COPY WebApi/WePlayRises.WebApi.csproj WebApi/
COPY BuildingBlocks/Kernel/Kernel.csproj BuildingBlocks/Kernel/
COPY BuildingBlocks/Abstractions/Abstractions.csproj BuildingBlocks/Abstractions/
COPY BuildingBlocks/EntityFramework/EntityFramework.csproj BuildingBlocks/EntityFramework/
COPY BuildingBlocks/Caching/Caching.csproj BuildingBlocks/Caching/
COPY Modules/UserAccess/WePlayRises.UserAccess.Domain/WePlayRises.UserAccess.Domain.csproj Modules/UserAccess/WePlayRises.UserAccess.Domain/
COPY Modules/UserAccess/WePlayRises.UserAccess.Application/WePlayRises.UserAccess.Application.csproj Modules/UserAccess/WePlayRises.UserAccess.Application/
COPY Modules/UserAccess/WePlayRises.UserAccess.Infra/WePlayRises.UserAccess.Infra.csproj Modules/UserAccess/WePlayRises.UserAccess.Infra/
COPY Modules/UserAccess/WePlayRises.UserAccess.WebApi/WePlayRises.UserAccess.WebApi.csproj Modules/UserAccess/WePlayRises.UserAccess.WebApi/
COPY Modules/Crowdfunding/WePlayRises.Crowdfunding.Domain/WePlayRises.Crowdfunding.Domain.csproj Modules/Crowdfunding/WePlayRises.Crowdfunding.Domain/
COPY Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/WePlayRises.Crowdfunding.Application.csproj Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/
COPY Modules/Crowdfunding/WePlayRises.Crowdfunding.Infra/WePlayRises.Crowdfunding.Infra.csproj Modules/Crowdfunding/WePlayRises.Crowdfunding.Infra/
COPY Modules/Crowdfunding/WePlayRises.Crowdfunding.WebApi/WePlayRises.Crowdfunding.WebApi.csproj Modules/Crowdfunding/WePlayRises.Crowdfunding.WebApi/
COPY Modules/Core/WePlayRises.Core.Domain/WePlayRises.Core.Domain.csproj Modules/Core/WePlayRises.Core.Domain/
COPY Modules/Core/WePlayRises.Core.Application/WePlayRises.Core.Application.csproj Modules/Core/WePlayRises.Core.Application/
COPY Modules/Core/WePlayRises.Core.Infra/WePlayRises.Core.Infra.csproj Modules/Core/WePlayRises.Core.Infra/
COPY Modules/Core/WePlayRises.Core.WebApi/WePlayRises.Core.WebApi.csproj Modules/Core/WePlayRises.Core.WebApi/
COPY Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Domain/WePlayRises.Crowdsourcing.Domain.csproj Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Domain/
COPY Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/WePlayRises.Crowdsourcing.Application.csproj Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/
COPY Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/WePlayRises.Crowdsourcing.Infra.csproj Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/
COPY Modules/Crowdsourcing/WePlayRises.Crowdsourcing.WebApi/WePlayRises.Crowdsourcing.WebApi.csproj Modules/Crowdsourcing/WePlayRises.Crowdsourcing.WebApi/
COPY Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/WePlayRises.Crowdpromotion.Domain.csproj Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/
COPY Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/WePlayRises.Crowdpromotion.Application.csproj Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/
COPY Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Infra/WePlayRises.Crowdpromotion.Infra.csproj Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Infra/
COPY Modules/Crowdpromotion/WePlayRises.Crowdpromotion.WebApi/WePlayRises.Crowdpromotion.WebApi.csproj Modules/Crowdpromotion/WePlayRises.Crowdpromotion.WebApi/

RUN dotnet restore WebApi/WePlayRises.WebApi.csproj

COPY . .
RUN dotnet publish WebApi/WePlayRises.WebApi.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WePlayRises.WebApi.dll"]
