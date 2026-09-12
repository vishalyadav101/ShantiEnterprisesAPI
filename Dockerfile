# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["ShantiEnterprises.API/ShantiEnterprises.API.csproj", "ShantiEnterprises.API/"]
RUN dotnet restore "ShantiEnterprises.API/ShantiEnterprises.API.csproj"

COPY . .
WORKDIR "/src/ShantiEnterprises.API"

RUN dotnet publish "ShantiEnterprises.API.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:10000

EXPOSE 10000

ENTRYPOINT ["dotnet", "ShantiEnterprises.API.dll"]