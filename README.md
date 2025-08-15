# C# Console Quiz Game with a Full DevOps Pipeline

This project contains a text-based quiz game on the topic of Geography, developed as a C# Console Application. It is accompanied by a complete DevOps toolchain for automated building, testing, security scanning, and deployment to an Azure Virtual Machine.

## Project Structure

- **/QuizGame**: Contains the C# source code for the console application.
- **Dockerfile**: A multi-stage Dockerfile to build an optimized, production-ready container image of the application.
- **docker-compose.yml**: For running the application locally using Docker.
- **terraform/**: Contains Terraform scripts to provision all necessary Azure infrastructure, including a VM with Docker pre-installed.
- **.github/workflows/**: Contains the GitHub Actions CI/CD pipeline definition (`ci-cd.yml`).

## Features

### Application Features
- **Two Modes**: Create Mode (add/edit/delete questions) and Play Mode.
- **Three Question Types**: Multiple Choice, Open-Ended, and True/False.
- **OOP Principles**: Utilizes abstraction, inheritance, and encapsulation.
- **Scoring & Timing**: Calculates the final score and the time taken to complete the quiz.

### DevOps Features
- **Source Control**: Git repository hosted on GitHub with a feature-branching workflow.
- **Infrastructure as Code (IaC)**: Terraform provisions a complete environment on Azure.
- **Containerization**: The application is containerized with Docker for portability and consistency.
- **Continuous Integration (CI)**: On every push to `main`, GitHub Actions automatically builds the application.
- **DevSecOps**: The Docker image is scanned for critical vulnerabilities using **Trivy**.
- **Continuous Deployment (CD)**: On a successful build and scan, the new image is pushed to Docker Hub and automatically deployed to the Azure VM.

## How to Run

### Locally (via .NET CLI)
1. Navigate to the `QuizGame` directory.
2. Run the command: `dotnet run`

### Locally (via Docker)
1. Make sure you have Docker installed.
2. From the `QuizGame` directory, run: `docker-compose up --build`
3. To interact with the game, attach to the container's terminal: `docker attach quiz-game-app` (Press `Ctrl+P` then `Ctrl+Q` to detach without stopping it).

### Deployed on Azure
The CI/CD pipeline handles the deployment automatically. To play the game on the deployed instance:
1. SSH into the Azure VM: `ssh <your_vm_username>@<your_vm_ip>`
2. Attach to the running Docker container: `docker attach quiz-game-app`

## CI/CD Pipeline

The pipeline is defined in `.github/workflows/ci-cd.yml` and performs the following steps on every push to the `main` branch:
1. **Build & Scan Job**:
    - Checks out the source code.
    - Builds the Docker image.
    - Scans the image with Trivy for HIGH and CRITICAL vulnerabilities. The job fails if any are found.
2. **Push & Deploy Job**:
    - Runs only if the `build-and-scan` job succeeds.
    - Pushes the scanned Docker image to Docker Hub.
    - SSHes into the Azure VM, pulls the new image, and restarts the Docker container with the updated version.