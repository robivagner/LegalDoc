FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["LegalDoc.API/LegalDoc.API.csproj", "LegalDoc.API/"]
COPY ["LegalDoc.Application/LegalDoc.Application.csproj", "LegalDoc.Application/"]
COPY ["LegalDoc.Domain/LegalDoc.Domain.csproj", "LegalDoc.Domain/"]
COPY ["LegalDoc.Infrastructure/LegalDoc.Infrastructure.csproj", "LegalDoc.Infrastructure/"]

RUN dotnet restore "LegalDoc.API/LegalDoc.API.csproj"

COPY LegalDoc.API/ LegalDoc.API/
COPY LegalDoc.Application/ LegalDoc.Application/
COPY LegalDoc.Domain/ LegalDoc.Domain/
COPY LegalDoc.Infrastructure/ LegalDoc.Infrastructure/

WORKDIR "/src/LegalDoc.API"
RUN dotnet publish "LegalDoc.API.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "LegalDoc.API.dll"]