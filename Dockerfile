# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY *.vbproj ./
COPY Sauberfix.Data/*.csproj ./Sauberfix.Data/

# Restore dependencies
RUN dotnet restore

# Copy source code
COPY . .

# Build and publish
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Install timezone data (required for TimeZoneInfo.FindSystemTimeZoneById)
RUN apt-get update && \
    apt-get install -y tzdata && \
    rm -rf /var/lib/apt/lists/*


# Copy published files
COPY --from=build /app/publish .

# Expose port
EXPOSE 5000

# Set environment
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "sauberfix.dll"]
