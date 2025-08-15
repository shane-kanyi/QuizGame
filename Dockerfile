# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# CHANGE 1: Specify the subdirectory for the .csproj file
# Copy the project file from 'QuizGame/QuizGame.csproj' into the container's '/app' directory
COPY QuizGame/QuizGame.csproj .

# This command can now find the project file and will succeed
RUN dotnet restore

# CHANGE 2: Copy all the source code from the 'QuizGame' subdirectory
# This copies Program.cs and other files into the container's '/app' directory
COPY QuizGame/. .

# This command remains the same
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Create the final runtime image
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# The entrypoint remains the same
ENTRYPOINT ["dotnet", "QuizGame.dll"]