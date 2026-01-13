# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY PaperlessMCP.csproj .
RUN dotnet restore

# Copy source code and build
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS runtime
WORKDIR /app

# Copy published application
COPY --from=build /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5000
ENV MCP_PORT=5000

# Expose port for HTTP transport
EXPOSE 5000

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:5000/mcp || exit 1

# Run the application (HTTP mode by default)
ENTRYPOINT ["dotnet", "PaperlessMCP.dll"]
