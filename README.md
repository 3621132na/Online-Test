Task Management Application
Overview
This is a full-stack Task Management application built with a backend in .NET Core (C#) and a frontend in React. The application allows users to register, log in, and manage tasks with features such as filtering, sorting, keyword search, and user role management (Admin/Regular).
Project Structure

TaskManagementBackend/: Contains the backend API written in C# using .NET Core.
TaskManagementFrontend/: Contains the frontend application built with React.
docker-compose.yml: Configuration file for Docker Compose to run the entire application.
TaskManagementAPI.postman_collection.json: Postman Collection for API documentation.

Prerequisites

Operating System: Windows, macOS, or Linux.
Tools:
.NET SDK 8.0
Node.js 18.x or later
Docker and Docker Compose (for containerization)
Git (optional, for cloning the repository)
PostgreSQL (for local database setup)



Setup and Run Instructions
1. Clone the Repository
If the project is in a Git repository, clone it:
git clone <repository-url>
cd TaskManagement

2. Backend Setup
Install Dependencies

Ensure .NET SDK is installed. Verify with:dotnet --version


Navigate to the backend directory:cd TaskManagementBackend



Configure Database

The application uses PostgreSQL. Install PostgreSQL and create a database named TaskManagementDb.
Update the connection string in appsettings.json:{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=TaskManagementDb;Username=your_username;Password=your_password"
  },
  "Jwt": {
    "Key": "your_secret_key",
    "Issuer": "your_issuer",
    "Audience": "your_audience"
  }
}


Run the initial migration to set up the database:dotnet ef migrations add InitialCreate
dotnet ef database update



Run Backend

Start the backend:dotnet run


The API will be available at http://localhost:5170.

3. Frontend Setup
Install Dependencies

Navigate to the frontend directory:cd TaskManagementFrontend


Install Node.js dependencies:npm install



Run Frontend

Start the development server:npm run dev


Open http://localhost:5173 in your browser.

Deployment with Docker Compose
1. Prepare the Environment

Ensure Docker and Docker Compose are installed.
Create a .env file in the root directory with the following content (adjust values as needed):POSTGRES_USER=your_username
POSTGRES_PASSWORD=your_password
POSTGRES_DB=TaskManagementDb
JWT_KEY=your_secret_key
JWT_ISSUER=your_issuer
JWT_AUDIENCE=your_audience



2. Docker Compose Configuration
Ensure docker-compose.yml is configured as follows:
version: '3.8'
services:
  backend:
    build: ./TaskManagementBackend
    ports:
      - "5170:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Host=db;Database=TaskManagementDb;Username=${POSTGRES_USER};Password=${POSTGRES_PASSWORD}
      - Jwt__Key=${JWT_KEY}
      - Jwt__Issuer=${JWT_ISSUER}
      - Jwt__Audience=${JWT_AUDIENCE}
    depends_on:
      - db
  frontend:
    build: ./TaskManagementFrontend
    ports:
      - "5173:3000"
    depends_on:
      - backend
  db:
    image: postgres:latest
    environment:
      - POSTGRES_USER=${POSTGRES_USER}
      - POSTGRES_PASSWORD=${POSTGRES_PASSWORD}
      - POSTGRES_DB=${POSTGRES_DB}
    volumes:
      - pgdata:/var/lib/postgresql/data
    ports:
      - "5432:5432"
volumes:
  pgdata:

3. Build and Run

Build and start the application:docker-compose up --build


The application will be available at:
Frontend: http://localhost:5173
Backend API: http://localhost:5170
Database: localhost:5432 (for local access if needed).



4. Stop the Application

Stop and remove containers:docker-compose down


To remove volumes (e.g., database data), add -v:docker-compose down -v



API Documentation

Swagger: Run the backend in Development mode (dotnet run --environment Development) and access Swagger UI at http://localhost:5170/.
Postman: Import the TaskManagementAPI.postman_collection.json file into Postman to test API endpoints. The collection includes:
POST /api/auth/register: Register a new user.
POST /api/auth/login: Login to get a JWT token.
POST /api/tasks: Create a new task.
GET /api/tasks: Get list of tasks (supports filtering, sorting, and keyword search).
PUT /api/tasks/{id}: Update a task.
DELETE /api/tasks/{id}: Delete a task.



Troubleshooting

Backend not starting: Check the connection string in appsettings.json and ensure PostgreSQL is running.
Frontend errors: Ensure all dependencies are installed (npm install) and check the console for detailed errors.
Connection refused (Docker): Ensure PostgreSQL is running and the connection string in .env is correct.
Build fails (Docker): Check Dockerfile in TaskManagementBackend and TaskManagementFrontend for syntax errors.
Port conflicts: Change port mappings in docker-compose.yml if needed.

Notes

Replace your_username, your_password, your_secret_key, your_issuer, and your_audience with appropriate values.
For production, add security measures (e.g., HTTPS, environment variable encryption).

