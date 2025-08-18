# Stage 1: Use the .NET 9 SDK
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source
COPY . .
RUN dotnet publish "QuizGame.sln" --configuration Release --output /app/publish

# Stage 2: Use the .NET 9 Preview Runtime
FROM mcr.microsoft.com/dotnet/runtime:9.0-preview AS final
WORKDIR /app
COPY --from=build /app/publish .

# Set the environment variable for the web server to listen on port 8080
ENV ASPNETCORE_URLS=http://+:8080

# Expose port 8080 to the Docker host
EXPOSE 8080

ENTRYPOINT ["dotnet", "QuizGame.dll"]