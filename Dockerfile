# Dockerfile w folderze głównym (obok HomeOrganizer.sln)

# ============================================
# Stage 1: Build
# ============================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Skopiuj solution file
COPY HomeOrganizer.sln ./

# Skopiuj wszystkie .csproj (ale bez testów - nie są potrzebne w produkcji)
COPY HomeOrganizer.Api/HomeOrganizer.Api.csproj HomeOrganizer.Api/
COPY HomeOrganizer.Application/HomeOrganizer.Application.csproj HomeOrganizer.Application/
COPY HomeOrganizer.Domain/HomeOrganizer.Domain.csproj HomeOrganizer.Domain/
COPY HomeOrganizer.Infrastructure/HomeOrganizer.Infrastructure.csproj HomeOrganizer.Infrastructure/

# Restore dependencies (dla całej solucji)
RUN dotnet restore HomeOrganizer.sln

# Skopiuj cały kod źródłowy (bez testów)
COPY HomeOrganizer.Api/ HomeOrganizer.Api/
COPY HomeOrganizer.Application/ HomeOrganizer.Application/
COPY HomeOrganizer.Domain/ HomeOrganizer.Domain/
COPY HomeOrganizer.Infrastructure/ HomeOrganizer.Infrastructure/

# Build projektu API (referencje do innych projektów są automatycznie budowane)
WORKDIR /src/HomeOrganizer.Api
RUN dotnet build HomeOrganizer.Api.csproj -c Release -o /app/build

# ============================================
# Stage 2: Publish
# ============================================
FROM build AS publish
RUN dotnet publish HomeOrganizer.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# ============================================
# Stage 3: Runtime
# ============================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Expose port (Railway używa zmiennej PORT)
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Skopiuj opublikowaną aplikację
COPY --from=publish /app/publish .

# Uruchom aplikację
ENTRYPOINT ["dotnet", "HomeOrganizer.Api.dll"]