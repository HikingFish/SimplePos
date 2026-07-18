# build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

# workdir to set working directory
WORKDIR /src

# restore
# Docker caches each instruction as a "layer". If a layer hasn't
# changed, Docker reuses the cached version. By copying ONLY
# the .csproj/.slnx files first and running `dotnet restore`,
# we cache the NuGet restore layer. This means if you only change
# a .cs file (not a package reference), Docker skips the slow
# restore step on subsequent builds.

COPY SimplePos.slnx ./
COPY src/SimplePos.Domain/SimplePos.Domain.csproj ./src/SimplePos.Domain/
COPY src/SimplePos.Application/SimplePos.Application.csproj ./src/SimplePos.Application/
COPY src/SimplePos.Infrastructure/SimplePos.Infrastructure.csproj ./src/SimplePos.Infrastructure/
COPY src/SimplePos.WebApi/SimplePos.WebApi.csproj ./src/SimplePos.WebApi/
COPY test/Tests/Tests.csproj ./test/Tests/

RUN dotnet restore SimplePos.slnx

# Build 
COPY . .

# Publish a release build into /app/publish.
# -c Release        → Release configuration (optimized, no debug symbols)
# -o /app/publish   → Output directory
# --no-restore      → Skip restore (we already did it above)
RUN dotnet publish src/SimplePos.WebApi/SimplePos.WebApi.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# Stage 2 Runtime
# This stage creates the final, lean image that actually runs.
# It uses the ASP.NET runtime image — much smaller than the SDK
# because it doesn't include compilers and build tools.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

# Copy ONLY the published output from the build stage.
# Everything else (source code, SDK, NuGet cache) is discarded.
# This is why it's called "multi-stage" — the final image is tiny.
COPY --from=build /app/publish .
# Tell ASP.NET Core to listen on port 8080.
# (Port 80 requires root; 8080 works with non-root users)
ENV ASPNETCORE_URLS=http://+:8080
# Expose port 8080 to the outside world.
# This is documentation — it tells other developers which port
# the container uses. It doesn't actually open the port.
EXPOSE 8080
# The command that runs when the container starts.
# This launches your Web API.
ENTRYPOINT ["dotnet", "SimplePos.WebApi.dll"]