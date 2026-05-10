# 🏡 RoomDecor API

## 📖 Overview
**RoomDecor API** is a robust and intelligent backend service built with **ASP.NET Core**. It serves as the core infrastructure for a modern interior design and room decoration application. The API not only handles secure user authentication but also acts as a bridge to an AI-powered microservice, providing users with smart room recommendations and visual search capabilities. 

This project is perfectly tailored to be consumed by front-end clients (like a Flutter mobile application), ensuring seamless communication, fast responses, and a secure environment.

---

## ✨ Key Features

### 1. 🔐 Secure User Authentication & Management
- Built on top of **ASP.NET Core Identity** for reliable user management.
- Secure user registration and login endpoints.
- Implements **JWT (JSON Web Tokens)** for stateless, secure API authorization.

### 2. 🧠 AI-Powered Room Recommendations
- Integrates with an external AI Python server.
- Users can input their room dimensions (Length, Width, Area) and preferred design **Style**.
- The API processes these inputs and returns tailored room design recommendations, complete with confidence scores and direct image URLs.

### 3. 👁️ Visual Search & Similar Designs (Vision AI)
- Allows users to find similar room designs based on a selected image.
- Communicates with a Computer Vision model to analyze image features and return the top visually similar rooms.
- Supports handling complex image layers, such as separated foregrounds/transparent images.

---

## 🛠️ Technology Stack
- **Framework:** ASP.NET Core 8.0 (Web API)
- **Database:** Microsoft SQL Server
- **ORM:** Entity Framework Core (Code-First Approach)
- **Authentication:** JWT (JSON Web Tokens) & ASP.NET Core Identity
- **External Integrations:** RESTful communication via `HttpClient` to an external Python Flask/FastAPI AI server (`http://127.0.0.1:5000`).
- **Documentation:** Swagger / OpenAPI

---

## 🏗️ Architecture & Workflow
1. **Client Request:** A client application (e.g., Flutter app) sends a request (like searching for a room style or uploading an image).
2. **API Gateway:** The ASP.NET Core API intercepts the request, validates the JWT token, and checks the user's permissions.
3. **AI Delegation:** For intelligent tasks, the API formats the request and forwards it to the dedicated local AI server running on `http://127.0.0.1:5000`.
4. **Response Formatting:** Once the AI processes the data, the API receives the raw output, enriches it (e.g., constructing absolute Image URLs based on the host), and returns a clean, structured JSON response to the client application.

---

## 🚀 Getting Started

### Prerequisites
- [.NET SDK](https://dotnet.microsoft.com/download) installed.
- SQL Server installed and running.
- The external Python AI Server running on `http://127.0.0.1:5000`.

### Setup
1. Clone this repository.
2. Update the `DefaultConnection` string in `appsettings.json` to point to your SQL Server instance.
3. Open a terminal in the project directory and run the migrations to create the database:
   ```bash
   dotnet ef database update
   ```
4. Run the application:
   ```bash
   dotnet run
   ```
5. Navigate to `http://localhost:<port>/swagger` to view the API documentation and test the endpoints.
