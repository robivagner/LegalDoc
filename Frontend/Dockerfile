FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["LegalDoc.Web/LegalDoc.Web.csproj", "LegalDoc.Web/"]
RUN dotnet restore "LegalDoc.Web/LegalDoc.Web.csproj"

COPY LegalDoc.Web/ LegalDoc.Web/

WORKDIR "/src/LegalDoc.Web"
RUN dotnet publish "LegalDoc.Web.csproj" -c Release -o /app/publish \
    --no-restore \
    -p:BlazorEnableServiceWorkerAssetsManifest=false \
    -p:GenerateServiceWorkerAssetsManifest=false

FROM nginx:alpine AS runtime
RUN rm -rf /usr/share/nginx/html/*
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]