# --- Stage 1: Base image ---
# This is the lightweight runtime image that will run our app.
# It only contains what's needed to RUN .NET apps, not build them.
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

# --- Stage 2: Build ---
# This uses the full SDK image which can COMPILE our code.
# We copy in project files, restore dependencies, then build.
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the .csproj file first and restore dependencies.
# This is done separately so Docker can cache this step —
# dependencies don't change often, so it skips re-downloading them every time.

COPY ["apps/messaging-microservice/src/MessagingService/MessagingService.csproj", "apps/messaging-microservice/src/MessagingService/"]
RUN dotnet restore "apps/messaging-microservice/src/MessagingService/MessagingService.csproj"

# Copy all the source code and build the project

COPY . .
WORKDIR "/src/apps/messaging-microservice/src/MessagingService"
RUN dotnet build "MessagingService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# --- Stage 3: Publish ---
# Takes the build output and optimizes it for deployment.
# /p:UseAppHost=false means we don't bundle the .NET runtime into the output —
# we already have it in the base image.

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MessagingService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# --- Stage 4: Final image ---
# Start from the lightweight runtime image again,
# copy in only the published output (no SDK, no source code).
# This keeps the final image small and clean.

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MessagingService.dll"]