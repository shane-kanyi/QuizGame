# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the solution and project files
COPY ["QuizGame.sln", "."]
COPY ["QuizGame.csproj", "."]

# Restore dependencies for the solution
RUN dotnet restore "./QuizGame.sln"

# Copy the rest of the application's source code
COPY . .

# Publish the project
RUN dotnet publish "./QuizGame.csproj" -c Release -o /app/publish

# Stage 2: Create the final runtime image
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "QuizGame.dll"]