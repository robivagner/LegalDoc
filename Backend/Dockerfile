FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiem doar .csproj-urile pentru a folosi cache-ul Docker la restore
COPY ["LegalDoc.API/LegalDoc.API.csproj", "LegalDoc.API/"]
COPY ["LegalDoc.Application/LegalDoc.Application.csproj", "LegalDoc.Application/"]
COPY ["LegalDoc.Domain/LegalDoc.Domain.csproj", "LegalDoc.Domain/"]
COPY ["LegalDoc.Infrastructure/LegalDoc.Infrastructure.csproj", "LegalDoc.Infrastructure/"]

RUN dotnet restore "LegalDoc.API/LegalDoc.API.csproj"

# Copiem restul codului
COPY . .
WORKDIR "/src/LegalDoc.API"

# Publicăm aplicația
RUN dotnet publish "LegalDoc.API.csproj" -c Release -o /app/publish --no-restore

# Stage de runtime (Debian - Stabil)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Dezactivăm Invariant Globalization (important pentru stabilitate)
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080
ENTRYPOINT ["dotnet", "LegalDoc.API.dll"]