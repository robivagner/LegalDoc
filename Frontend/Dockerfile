FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. Copiem și restaurăm pachetele
COPY ["LegalDoc.Web/LegalDoc.Web.csproj", "LegalDoc.Web/"]
RUN dotnet restore "LegalDoc.Web/LegalDoc.Web.csproj"

# 2. Copiem restul codului
COPY . .
WORKDIR "/src/LegalDoc.Web"

# 3. Publish cu DEZACTIVARE TOTALĂ a manifestelor (Nuking the PWA logic)
RUN dotnet publish "LegalDoc.Web.csproj" -c Release -o /app/publish \
    --no-restore \
    -p:BlazorEnableServiceWorkerAssetsManifest=false \
    -p:GenerateServiceWorkerAssetsManifest=false

# Stage 2: Runtime cu Nginx
FROM nginx:alpine AS runtime
RUN rm -rf /usr/share/nginx/html/*
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]