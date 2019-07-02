FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ./FourTwenty.Core/FourTwenty.Core.csproj ./FourTwenty.Core/
RUN dotnet restore

# Copy everything else and build
COPY ./FourTwenty.Core/ ./FourTwenty.Core/
RUN dotnet build -c Release -o out/core
RUN dotnet publish -c Release -o out/core

# Copy csproj and restore as distinct layers
COPY ./FourTwenty.Dashboard/FourTwenty.Dashboard.csproj ./FourTwenty.Dashboard/
RUN dotnet restore

# Copy everything else and build
COPY ./FourTwenty.Dashboard/ ./FourTwenty.Dashboard/
RUN dotnet build -c Release -o out/dashboard
RUN dotnet publish -c Release -o out/dashboard

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
WORKDIR /app
COPY --from=build-env /app/out/core ./core
ENTRYPOINT ["dotnet", "core/FourTwenty.Core.dll"]

COPY --from=build-env /app/out/dashboard ./dashboard
ENTRYPOINT ["dotnet", "dashboard/CompanyWeb.Dashboard.dll"]

