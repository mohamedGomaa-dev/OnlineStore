# Store API

This is a backend API for an e-commerce platform built with C# and ASP.NET Core. The project follows an N-Tier architecture and implements strict business rules, data integrity, and security measures.

## Architecture

The solution is divided into four main layers to separate concerns:
* **Store.Models**: Domain entities, enums, and base interfaces.
* **Store.DataAccess**: Database context, EF Core configurations, Generic Repository, and Unit of Work implementation.
* **Store.Services**: Business logic, DTOs, AutoMapper profiles, and JWT generation.
* **Store.API**: Controllers, routing, and middleware configurations.

## Core Features

* **Authentication & Authorization**: JWT-based auth. Role-based access control (Admin vs. Customer) and strict resource ownership (users can only access their own orders and data). Passwords are encrypted using BCrypt.
* **Order Management**: Handles order creation, calculates totals, and manages order states.
* **Payments**: Validates payment amounts against order totals and prevents duplicate payments. Updates order status upon success.
* **Shipping Management**: Tracks shipments, carrier details, and dynamically updates actual delivery dates when status changes to delivered.
* **Verified Reviews**: Users can only leave a 1-5 star review if the database verifies they have a completed order containing that specific product.
* **Data Integrity**: Soft delete is implemented via global query filters across all entities to prevent accidental data loss.

## Tech Stack

* C# / .NET
* ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* BCrypt.Net-Next (Password Hashing)
* AutoMapper


## How to Run

1. Clone the repository.
2. Update the connection string in `appsettings.json` to point to your local SQL Server.
3. Configure your JWT secret key. You can add this to your local User Secrets to avoid exposing it:
   `dotnet user-secrets set "JWT_SECRET_KEY" "your_super_secret_key_here"`
4. Open the Package Manager Console, set `Store.DataAccess` as the default project, and run `Update-Database` to apply migrations.
5. Run the project. Swagger UI will open by default for endpoint testing.

## Notes

When testing endpoints in Swagger, use the `/api/auth/register` and `/api/auth/login` endpoints first. Copy the returned token, click the "Authorize" button at the top of Swagger, and enter `Bearer {your_token}` to access protected routes.
