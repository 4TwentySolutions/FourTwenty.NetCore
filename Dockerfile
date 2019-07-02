FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
WORKDIR /src
COPY FourTwenty.sln ./
COPY FourTwenty.Core/*.csproj ./FourTwenty.Core/
COPY FourTwenty.Dashboard/*.csproj ./FourTwenty.Dashboard/
RUN dotnet restore

# Copy everything else and build
COPY . .
WORKDIR /src/FourTwenty.Core
RUN dotnet build -c Release -o /app

WORKDIR /src/FourTwenty.Dashboard
RUN dotnet build -c Release -o /app

WORKDIR /app
# Publishing
RUN dotnet publish -c Release -o /app

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "FourTwenty.Dashboard.dll"]



