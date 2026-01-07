# Contacts Manager

A full-featured web application built with ASP.NET Core 8 using Razor Views and a Clean Architecture approach, designed to manage and organize contact information with a modern, scalable, and maintainable structure.

---

## 🎯 Project Goals
This application demonstrates solid practical skills in:

- Clean Architecture applied in ASP.NET Core
- MVC & Razor Views development
- Reusable UI components using View Components and Tag Helpers
- Authentication & Authorization with ASP.NET Core Identity
- Validation, Filters, Logging, and Dependency Injection (DI)
- Automated testing using xUnit
- Best practices for maintainability and layering in .NET 8

---

## 🧪 Features

✔️ Full contact management (Create, Read, Update, Delete)  
✔️ Full ASP.NET Core Identity integration (login, registration, roles)  
✔️ Custom Tag Helpers and View Components for reusable UI  
✔️ Clean Architecture with separation of concerns  
✔️ Server-side validation and Filters (action, exception, resource filters)  
✔️ Repository pattern + EF Core  
✔️ Logging with built-in ASP.NET Core abstractions  
✔️ Unit testing using xUnit  
✔️ Responsive design with Tailwind CSS v4  
✔️ Secure form handling with antiforgery tokens  

---

## 🧰 Technologies Used

- **.NET 8** (ASP.NET Core MVC + Razor Views)  
- **Entity Framework Core**  
- **ASP.NET Core Identity**  
- **Razor Pages & Views**  
- **xUnit**  
- **Clean Architecture**  
- **Dependency Injection**  
- **Logging & Filters**  
- **Tailwind CSS v4**  
- **SQL Server**  

---

## 🛠️ Getting Started

### Prerequisites
- Visual Studio 2022
- .NET 8 SDK  
- SQL Server 

### Installation

1. Clone the repository:
```bash
git clone https://github.com/ejbpz/contacts-manager-app.git
```
2. Navigate to the project directory:
```bash
cd contacts-manager-app
```
3. Restore packages:
```bash
dotnet restore
```
4. Apply migrations and update the database:
```bash
dotnet ef database update
```
5. Run the application:
```bash
dotnet run
```

---

## 📜 License
This project is open-source. You are free to use, modify, and enhance it for personal or commercial projects.
