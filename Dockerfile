FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS base
ARG BUILD_NUMBER=0.0.0.1
ARG PAT
WORKDIR /multi-carrier
COPY "MultiCarrier.sln" "MultiCarrier.sln"
COPY "src/MultiCarrier.Api/MultiCarrier.Api.csproj" "src/MultiCarrier.Api/"
COPY "src/MultiCarrier.Database/MultiCarrier.Database.csproj" "src/MultiCarrier.Database/"
COPY "src/MultiCarrier.Database.Migrator/MultiCarrier.Database.Migrator.csproj" "src/MultiCarrier.Database.Migrator/"
COPY "build/nuget.config" "nuget.config"
RUN dotnet restore MultiCarrier.sln 
COPY . .
RUN dotnet build -c Release /p:Version=$BUILD_NUMBER /p:AssemblyVersion=$BUILD_NUMBER /p:FileVersion=$BUILD_NUMBER

FROM base AS publish-api
RUN dotnet publish "src/MultiCarrier.Api/MultiCarrier.Api.csproj" -c Release -o /app/publish --no-build

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS api
WORKDIR /app
COPY --from=publish-api /app/publish .
ENTRYPOINT ["dotnet", "MultiCarrier.Api.dll"]


FROM base AS publish-migrator
RUN dotnet publish "src/MultiCarrier.Database.Migrator/MultiCarrier.Database.Migrator.csproj" -c Release -o /app/publish --no-build 

FROM amazon/aws-lambda-dotnet:6 AS migrator
WORKDIR /var/task/
COPY --from=publish-migrator /app/publish .
CMD ["MultiCarrier.Database.Migrator::MultiCarrier.Database.Migrator.Program::Handler"]

FROM amazon/aws-lambda-dotnet:6.0 AS migrator-local
WORKDIR /var/task/
COPY --from=publish-migrator /app/publish .
ENTRYPOINT ["dotnet", "MultiCarrier.Database.Migrator.dll"]

