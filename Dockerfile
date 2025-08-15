# Stage 1: Use the .NET 9 Preview SDK to match your project's target framework
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

# Copy all files from the build context into the container
COPY . .

# Publish the solution using the .NET 9 SDK. This will now succeed.
RUN dotnet publish "QuizGame.sln" --configuration Release --output /app/publish

# Stage 2: Use the .NET 9 Preview Runtime for the final, lean image
FROM mcr.microsoft.com/dotnet/runtime:9.0-preview AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "QuizGame.dll"]