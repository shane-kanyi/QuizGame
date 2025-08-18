# Stage 1: Use the .NET 9 Preview SDK to build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

# Copy all files
COPY . .

# --- THIS IS THE KEY CHANGE ---
# Publish a self-contained application for Linux.
# This packages the .NET runtime WITH the app.
RUN dotnet publish "QuizGame.sln" --configuration Release --output /app/publish --runtime linux-x64 --self-contained true

# Stage 2: Use the ASP.NET base image for the final stage.
# It's leaner than the SDK but contains all necessary OS dependencies.
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copy the self-contained application output from the build stage
COPY --from=build /app/publish .

# Set the environment variable for the web server
ENV ASPNETCORE_URLS=http://+:8080

# Expose port 8080
EXPOSE 8080

# --- THE ENTRYPOINT IS NOW THE APP'S OWN EXECUTABLE ---
# We no longer need to call "dotnet" because the runtime is included.
ENTRYPOINT ["./QuizGame"]