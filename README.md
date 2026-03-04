# StancaBlogApi

ASP.NET Core Web API for a blog community project.

## Tech Stack
- .NET 8
- ASP.NET Core Web API
- Entity Framework Core + SQL Server
- JWT Authentication
- Swagger/OpenAPI

## Project Structure
- `Controllers` API endpoints
- `Core/Services` business logic
- `Data/Context` EF Core DbContext
- `Data/Entities` domain entities (`User`, `BlogPost`, `Category`, `Comment`)
- `Data/Repos` repositories
- `Application/DTOs` request/response DTOs
- `Infrastructure/Auth` JWT token generator
- `Migrations` EF Core migrations

## Requirements Covered
- Register, login, update account, delete account
- Login returns `userId` and JWT token
- Logged-in user can create blog posts
- Public users can read/search/filter posts
- Categories stored in separate table
- Owner-only update/delete for posts
- Logged-in users can comment on others' posts
- Users cannot comment on own posts
- Search by title and filter by category
- Swagger enabled for API documentation

## Main Endpoints
- Auth
  - `POST /api/auth/register`
  - `POST /api/auth/login`
  - `PUT /api/auth/me` (JWT)
  - `PUT /api/auth/me/password` (JWT)
  - `DELETE /api/auth/me` (JWT)
- Blog posts
  - `GET /api/blogposts`
  - `GET /api/blogposts/{id}`
  - `POST /api/blogposts` (JWT)
  - `PUT /api/blogposts/{id}` (JWT, owner only)
  - `DELETE /api/blogposts/{id}` (JWT, owner only)
- Comments
  - `GET /api/blogposts/{postId}/comments`
  - `POST /api/blogposts/{postId}/comments` (JWT)
  - `PUT /api/comments/{id}` (JWT, owner only)
  - `DELETE /api/comments/{id}` (JWT, owner only)
- Categories
  - `GET /api/categories`

## Run
1. Configure `appsettings.json` (SQL connection + JWT key).
2. Run:
```bash
dotnet restore
dotnet build
dotnet run
```
3. Open Swagger: `https://localhost:<port>/swagger`

## Postman
- Collection file: `StancaBlogApi.postman_collection.json`
