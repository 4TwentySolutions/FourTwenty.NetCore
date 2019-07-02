FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
WORKDIR /core

# Copy csproj and restore as distinct layers
COPY ./FourTwenty.Core/FourTwenty.Core.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY ./FourTwenty.Core/ ./
RUN dotnet build -c Release -o out
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
WORKDIR /core
COPY --from=build-env /core/out .
ENTRYPOINT ["dotnet", "FourTwenty.Core.dll"]

WORKDIR /dashboard

# Copy csproj and restore as distinct layers
COPY ./FourTwenty.Dashboard/FourTwenty.Dashboard.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY ./FourTwenty.Dashboard/ ./
RUN dotnet build -c Release -o out
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
WORKDIR /dashboard
COPY --from=build-env /dashboard/out .
ENTRYPOINT ["dotnet", "FourTwenty.Dashboard.dll"]



