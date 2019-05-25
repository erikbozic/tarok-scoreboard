FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY src/TarokScoreBoard.Api/*.csproj ./src/TarokScoreBoard.Api/
COPY src/TarokScoreBoard.Core/*.csproj ./src/TarokScoreBoard.Core/
COPY src/TarokScoreBoard.Shared/*.csproj ./src/TarokScoreBoard.Shared/
COPY src/TarokScoreBoard.Infrastructure/*.csproj ./src/TarokScoreBoard.Infrastructure/
COPY src/TarokScoreBoard.Tests/*.csproj ./src/TarokScoreBoard.Tests/
RUN dotnet restore

# copy everything else and build app
COPY  . ./
RUN dotnet publish -c Release -o out


# Build runtime image
FROM microsoft/dotnet:2.2.0-aspnetcore-runtime-alpine
WORKDIR /
COPY --from=build /app/src/TarokScoreBoard.Api/out .
ENV ASPNETCORE_URLS http://+:5000
# Use tiered compilation for better compilation and throughput performance
ENV COMPlus_TieredCompilation 1
ENTRYPOINT ["dotnet", "TarokScoreBoard.Api.dll"]
