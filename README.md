# Dynamic Quiz Builder

A full-stack, dynamic web application for creating, managing, and playing quizzes on any topic. Originally a C# console app, now a cloud-native web app with automated DevOps pipeline.

---

## 🚀 Live Demo
The app is deployed on a Microsoft Azure Virtual Machine:
```
http://<YOUR_AZURE_VM_IP>
```
Find the IP by running `terraform output public_ip_address` in the `terraform` directory or checking your GitHub secrets.

---

## ✨ Core Features
### Application
- **Dynamic Quiz Creation:** Create new quizzes from scratch.
- **Multiple Question Types:** Add multiple-choice, open-ended, and true/false questions.
- **Play Any Quiz:** Home screen lists all quizzes to play.
- **Interactive Gameplay:** SPA experience for answering questions and seeing results.
- **Responsive UI:** Works on desktop, laptop, and mobile.
- **In-Memory Storage:** Quizzes are stored in memory (data cleared on restart).

### DevOps & Engineering
- **Infrastructure as Code (IaC):** Azure cloud infrastructure managed with Terraform.
- **Containerized:** Packaged as a Docker image for portability.
- **Automated CI/CD:** GitHub Actions pipeline for build, scan, and deploy on every push to `main`.
- **DevSecOps Security Gate:** Trivy scans Docker images for HIGH/CRITICAL vulnerabilities; build fails if found (unless ignored in `.trivyignore`).

---

## 🛠️ Technology Stack
| Area      | Technologies Used                        |
|-----------|------------------------------------------|
| Backend   | C#, ASP.NET Core, Web API                |
| Frontend  | HTML5, CSS3, Vanilla JavaScript (SPA)    |
| Cloud     | Microsoft Azure (Virtual Machine)        |
| DevOps    | Docker, Terraform, GitHub Actions, Trivy |

---

## 🏗️ Project Structure
```
.
├── .github/workflows/      # GitHub Actions CI/CD pipeline
├── Controllers/            # ASP.NET Core API controllers
├── Models/                 # C# classes for game logic
├── terraform/              # Terraform scripts for Azure infra
├── wwwroot/                # Frontend assets (HTML, CSS, JS)
├── .dockerignore           # Docker ignore file
├── .gitignore              # Git ignore file
├── .trivyignore            # Trivy CVE ignore file
├── Dockerfile              # Docker build file
├── QuizGame.csproj         # C# project file
└── QuizGame.sln            # Solution file
```

---

## 💻 How to Run Locally
### Prerequisites
- .NET 9 SDK
- Docker Desktop

### Option 1: Using the .NET CLI
Run the web server directly:
```bash
# From the project root
dotnet run
```
App available at `http://localhost:5xxx` (see terminal output for port).

### Option 2: Using Docker (Recommended)
Build and run inside a container:
```bash
# From the project root
docker build -t quiz-game-local .
docker run -p 8080:8080 quiz-game-local
```
App available at `http://localhost:8080`.

---

## 🔄 CI/CD Pipeline Workflow
- **Trigger:** On every push to `main`.
- **Build:** Compile and publish C# app, build Docker image.
- **Scan:** Trivy scans Docker image for vulnerabilities. Build fails if HIGH/CRITICAL CVEs found (unless ignored).
- **Push:** Tag and push image to Docker Hub.
- **Deploy:** SSH into Azure VM, pull latest image, stop old container, start new one with correct port mapping.

---

## 🚀 Future Improvements
- **Persistent Storage:** Use a database (PostgreSQL/SQLite) for quizzes.
- **User Accounts:** Add authentication for private quizzes.
- **Custom Domain & HTTPS:** Set up domain and SSL (Nginx/Caddy) for secure access.