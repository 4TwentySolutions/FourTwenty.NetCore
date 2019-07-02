FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY FourTwenty.sln ./
COPY FourTwenty.Core/*.csproj ./FourTwenty.Core/
COPY FourTwenty.Dashboard/*.csproj ./FourTwenty.Dashboard/
RUN dotnet restore

# Copy everything else and build
COPY . .
WORKDIR /FourTwenty.Core
RUN dotnet build -c Release -o out

WORKDIR /FourTwenty.Dashboard
RUN dotnet build -c Release -o out

# Publishing
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "FourTwenty.Dashboard.dll"]



