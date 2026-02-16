# StancaBlogApi

ASP.NET Core Web API for a blog community where users can register/login, create blog posts, categorize posts, and comment on other users' posts.

## Tech Stack
- .NET 8 Web API
- Entity Framework Core + SQL Server
- JWT authentication
- Swagger/OpenAPI
- BCrypt password hashing

## Project Structure
- `Application/DTOs` Request/response DTOs
- `Application/Mappings` Mapping helpers
- `Controllers` Thin controllers (HTTP only)
- `Core/Interfaces` Service contracts
- `Core/Services` Business logic
- `Data/Context` EF Core `AppDbContext`
- `Data/Entities` Domain entities (`User`, `BlogPost`, `Category`, `Comment`)
- `Data/Interfaces` Repository contracts
- `Data/Repos` Repository implementations
- `Infrastructure/Auth` JWT token generator
- `Infrastructure/Security` Claims helpers
- `Migrations` EF Core migrations

## Run Instructions
1. Create `appsettings.json` from `appsettings.json.example`.
2. Set a valid SQL Server connection string and JWT settings.
3. Run:
```bash
dotnet restore
dotnet build
dotnet run
```
4. Open Swagger at:
- `https://localhost:<port>/swagger`

## Postman and Swagger
- A Postman collection exists: `StancaBlogApi.postman_collection.json`
- All API methods are exposed through controllers and can be called from Postman.

## Requirement Checklist
Assessment based on the assignment requirements you shared and current code state.

### Fulfilled
- User registration with username, password, and email.
- User login works and returns both JWT token and `userId`.
- Authenticated user can update account password (`PUT /api/auth/me/password`).
- Authenticated user can create blog posts.
- Unauthenticated users can read posts and search/filter posts.
- Categories are stored in a separate table (`Categories`), seeded in database.
- Blog posts can be created, updated, and deleted.
- Only the post owner can update/delete their own post.
- Logged-in users can comment on other users' posts.
- Users cannot comment on their own posts.
- Search by title is implemented (`search` query parameter).
- Filtering by category is implemented (`categoryId` query parameter).
- Swagger documentation is enabled with JWT support.
- Project builds successfully (`dotnet build` passed with 0 errors).

### Partially Fulfilled
- "Suitable status codes and data" is improved and mostly aligned with REST conventions, but response contracts can still be standardized further across all endpoints.

### Not Fulfilled
- No major missing functional requirement identified.

## Main Endpoints
- Auth:
  - `POST /api/auth/register`
  - `POST /api/auth/login`
  - `PUT /api/auth/me` (JWT)
  - `PUT /api/auth/me/password` (JWT)
  - `DELETE /api/auth/me` (JWT)
- Blog posts:
  - `GET /api/blogposts` (supports `search`, `categoryId`, pagination)
  - `GET /api/blogposts/{id}`
  - `POST /api/blogposts` (JWT)
  - `PUT /api/blogposts/{id}` (JWT, owner only)
  - `DELETE /api/blogposts/{id}` (JWT, owner only)
- Comments:
  - `GET /api/blogposts/{postId}/comments`
  - `POST /api/blogposts/{postId}/comments` (JWT, cannot comment own post)
  - `PUT /api/comments/{id}` (JWT, owner only)
  - `DELETE /api/comments/{id}` (JWT, owner only)
- Categories:
  - `GET /api/categories`

## Exam Check (Postman)
Use `{{baseUrl}}` and run requests in this order.

1. Register account  
Request: `POST {{baseUrl}}/api/Auth/register`  
Body:
```json
{
  "name": "anna",
  "email": "anna@test.se",
  "password": "secret123"
}
```  
Expected: `201 Created`

2. Login  
Request: `POST {{baseUrl}}/api/Auth/login`  
Body:
```json
{
  "name": "anna",
  "password": "secret123"
}
```  
Expected: `200 OK` with `token` and `userId`

3. Update account  
Request: `PUT {{baseUrl}}/api/Auth/me` with `Authorization: Bearer {{token}}`  
Body:
```json
{
  "name": "anna2",
  "email": "anna2@test.se"
}
```  
Expected: `200 OK`

4. Change password  
Request: `PUT {{baseUrl}}/api/Auth/me/password` with `Authorization: Bearer {{token}}`  
Body:
```json
{
  "currentPassword": "secret123",
  "newPassword": "secret456"
}
```  
Expected: `204 No Content`

5. Get categories  
Request: `GET {{baseUrl}}/api/Categories`  
Expected: `200 OK` with category list from DB table

6. Create blog post (logged in)  
Request: `POST {{baseUrl}}/api/BlogPosts` with `Authorization: Bearer {{token}}`  
Body:
```json
{
  "title": "My first post",
  "content": "Hello community!",
  "categoryId": 1
}
```  
Expected: `201 Created`

7. Read and search blog posts (public)  
Request: `GET {{baseUrl}}/api/BlogPosts`  
Expected: `200 OK`  
Request: `GET {{baseUrl}}/api/BlogPosts?search=first`  
Expected: `200 OK`  
Request: `GET {{baseUrl}}/api/BlogPosts?categoryId=1`  
Expected: `200 OK`

8. Update own blog post  
Request: `PUT {{baseUrl}}/api/BlogPosts/{{postId}}` with `Authorization: Bearer {{token}}`  
Body:
```json
{
  "title": "Updated title",
  "content": "Updated text",
  "categoryId": 1
}
```  
Expected: `204 No Content`

9. Delete own blog post  
Request: `DELETE {{baseUrl}}/api/BlogPosts/{{postId}}` with `Authorization: Bearer {{token}}`  
Expected: `204 No Content`

10. Comment on another user's post  
Request: `POST {{baseUrl}}/api/blogposts/{{postId}}/comments` with `Authorization: Bearer {{token}}`  
Body:
```json
{
  "content": "Nice post!"
}
```  
Expected: `201 Created`  
Restriction check: if user comments own post, expected `400 Bad Request`

11. Update and delete own comment  
Request: `PUT {{baseUrl}}/api/comments/{{commentId}}` with `Authorization: Bearer {{token}}`  
Body:
```json
{
  "content": "Edited comment"
}
```  
Expected: `204 No Content`  
Request: `DELETE {{baseUrl}}/api/comments/{{commentId}}` with `Authorization: Bearer {{token}}`  
Expected: `204 No Content`

12. Delete account  
Request: `DELETE {{baseUrl}}/api/Auth/me` with `Authorization: Bearer {{token}}`  
Expected: `204 No Content`

## Live Demo Checklist (Short)
1. Start API and open Swagger (`/swagger`) - verify API is running.
2. Register user - `POST /api/Auth/register` -> `201`.
3. Login user - `POST /api/Auth/login` -> `200` (copy `token`, `userId`).
4. Get categories - `GET /api/Categories` -> `200`.
5. Create post - `POST /api/BlogPosts` (Bearer token) -> `201`.
6. Public read/search - `GET /api/BlogPosts`, then `?search=...`, then `?categoryId=...` -> `200`.
7. Update own post - `PUT /api/BlogPosts/{id}` (Bearer) -> `204`.
8. Comment on another user's post - `POST /api/blogposts/{postId}/comments` (Bearer) -> `201`.
9. Show rule: own-post comment blocked - same endpoint on own post -> `400`.
10. Delete own post - `DELETE /api/BlogPosts/{id}` (Bearer) -> `204`.
11. Delete account - `DELETE /api/Auth/me` (Bearer) -> `204`.
