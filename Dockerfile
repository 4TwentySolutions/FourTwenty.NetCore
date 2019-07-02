FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
WORKDIR /app

# Build and Publish FourTwenty.Core library
# Copy csproj and restore as distinct layers
COPY ./FourTwenty.Core/FourTwenty.Core.csproj ./core/
WORKDIR /app/core
RUN dotnet restore

# Copy everything else and build
COPY ./FourTwenty.Core/ ./core/
RUN dotnet build -c Release -o out
RUN dotnet publish -c Release -o out

# Build and Publish FourTwenty.Dashboard RCL library
WORKDIR /app
COPY ./FourTwenty.Dashboard/FourTwenty.Dashboard.csproj ./dashboard/
WORKDIR /app/dashboard
RUN dotnet restore

# Copy everything else and build
COPY ./FourTwenty.Dashboard/ ./dashboard/
RUN dotnet build -c Release -o out
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
WORKDIR /app/core
COPY --from=build-env /app/core/out .
ENTRYPOINT ["dotnet", "FourTwenty.Core.dll"]

WORKDIR /app/dashboard
COPY --from=build-env /app/dashboard/out .
ENTRYPOINT ["dotnet", "FourTwenty.Dashboard.dll"]





