FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Skopiuj solution file
COPY HomeOrganizer.sln ./

# Skopiuj wszystkie .csproj (ale bez testów)
COPY HomeOrganizer.Api/HomeOrganizer.Api.csproj HomeOrganizer.Api/
COPY HomeOrganizer.Application/HomeOrganizer.Application.csproj HomeOrganizer.Application/
COPY HomeOrganizer.Domain/HomeOrganizer.Domain.csproj HomeOrganizer.Domain/
COPY HomeOrganizer.Infrastructure/HomeOrganizer.Infrastructure.csproj HomeOrganizer.Infrastructure/

# ⬇️ ZMIENIONE: Restore tylko API project (automatycznie restore wszystkie zależności)
RUN dotnet restore HomeOrganizer.Api/HomeOrganizer.Api.csproj

# Skopiuj cały kod źródłowy (bez testów)
COPY HomeOrganizer.Api/ HomeOrganizer.Api/
COPY HomeOrganizer.Application/ HomeOrganizer.Application/
COPY HomeOrganizer.Domain/ HomeOrganizer.Domain/
COPY HomeOrganizer.Infrastructure/ HomeOrganizer.Infrastructure/

# Build projektu API
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

# Expose port
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Skopiuj opublikowaną aplikację
COPY --from=publish /app/publish .

# Uruchom aplikację
ENTRYPOINT ["dotnet", "HomeOrganizer.Api.dll"]