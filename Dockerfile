# Use the .NET SDK image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory
WORKDIR /source

# Copy everything from the build context (your repo root) into the container
COPY . .

# Restore dependencies, build, and publish in a single command.
# This is a very robust way to build, as it handles the entire lifecycle.
# The command targets the solution file, which is best practice.
RUN dotnet publish "QuizGame.sln" --configuration Release --output /app/publish

# Start the final, lean image
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "QuizGame.dll"]