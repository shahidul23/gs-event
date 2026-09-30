# FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
# WORKDIR /src
# COPY . .
# RUN dotnet restore "GSEvent/GSEvent.csproj"
# RUN dotnet publish \
#     "GSEvent/GSEvent.csproj" \
#     -c Release \
#     -o /app/publish \
#     --no-restore

# FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
# WORKDIR /app
# ENV ASPNETCORE_URLS=http://+:8080
# EXPOSE 8080
# COPY --from=build /app/publish .
# ENTRYPOINT ["dotnet", "GSEvent.dll"]

# Stage 1: Build stage with .NET 10 SDK
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY GSEvent/GSEvent.csproj GSEvent/
RUN dotnet restore "GSEvent/GSEvent.csproj"

# Install global tools (e.g., dotnet-ef) inside the SDK container
RUN dotnet tool install --global dotnet-ef
ENV PATH="${PATH}:/root/.dotnet/tools"

COPY . .
RUN dotnet publish "GSEvent/GSEvent.csproj" -c Release -o /app/publish --no-restore

# Stage 2: Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GSEvent.dll"]