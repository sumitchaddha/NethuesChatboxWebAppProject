# 🔐Chatbox Web App

A modern, secure, and lightweight ASP.NET Core 8 MVC application built with explicit control over authentication, data access, and user experience.
---

## 🚀 Features

- **User Authentication**: Login, Register, and JWT-based session management
- **Secure Password Handling**: Passwords hashed using BCrypt with built-in salt
- **Dashboard**: Interactive arithmetic chatbox with expression evaluation
- **User History**: View past calculations using the history button
- **Account Management**: Change password functionality with validation
- **Home/Index Page**: Clean landing page for unauthenticated users

---

## 🛡️ Security Highlights

- **JWT Authentication & Authorization**  
  Stateless session management using access and refresh tokens

- **BCrypt Password Hashing**  
  Adaptive hashing with automatic salt handling — no manual salt storage required. Preferred Over PBKDF2 because of no salt  and iteration management

- **Exception Handling Middleware**  
  Centralized error handling for clean API responses and logging

- **Raw ADO.NET + SQLite**  
  Direct SQL access for full control over queries and performance tuning

---

## 🧪 Testing Strategy

- **Unit Tests with xUnit**  
  Only JWT service logic are covered with isolated unit tests

- **Mocking with Moq**  
  Dependencies like repositories and JWT services are mocked for pure unit testing

- **In-Memory SQLite**  
  Used for testing DB operations without touching production data

---

## 📦 Tech Stack

| Layer       | Technology                     |
|------------|---------------------------------|
| Backend     | ASP.NET Core 8 (MVC)           |
| Auth        | JWT (Access + Refresh Tokens)  |
| DB Access   | Raw ADO.NET with SQLite        |
| Passwords   | BCrypt.Net                     |
| Testing     | xUnit + Moq                    |
| UI          | Razor Views + Bootstrap        |

---

## 📁 Project Structure
NethuesChatboxWebApp/ ├── Controllers/ ├── Models/ ├── Services/ ├── Views/ ├── Middleware/ └── Tests/ (xUnit-based)

---

## 🛠️ Getting Started

1. Clone the repository  
   `git clone https://github.com/your-username/SecureMvcJwt.git`

2. Update your `appsettings.json` with a secure JWT key and SQLite path

3. Run the application  
   `dotnet run`

4. Run unit tests  
   `dotnet test`
   
---
