# API Design & Documentation
## API Design
### RESTful Resource Naming
| Rule         | Example                 |
| ------------ | ----------------------- |
| plural nouns | `/users`                |
| no verbs     | x `/getUsers`           |
| hierarchy    | `/companies/{id}/users` |
| lowercase    | `/api/v1/users`         |
### HTTP Methods
| Method | Use            |
| ------ | -------------- |
| GET    | Query          |
| POST   | Create         |
| PUT    | Full update    |
| PATCH  | Partial update |
| DELETE | Remove         |
### Http status codes
| Code | Meaning          |
| ---- | ---------------- |
| 200  | OK               |
| 201  | Created          |
| 204  | No Content       |
| 400  | Validation error |
| 401  | Unauthorized     |
| 403  | Forbidden        |
| 404  | Not found        |
| 409  | Conflict         |
| 500  | Server error     |
## Response & error standard
### Standard API Response
- ApiResponse
```csharp
                ServiceResultType.Success => Ok(
                    new
                    {
                        Data = result.Data,
                        Message = result.Message
                    }
                ),
                ServiceResultType.Created => StatusCode(StatusCodes.Status201Created,
                    new
                    {
                        Data = result.Data,
                        Message = result.Message
                    }
                ),
                ServiceResultType.NoContent => StatusCode(StatusCodes.Status204NoContent,
                    new
                    {
                        Data = result.Data,
                        Message = result.Message
                    }
                ),
```
- Error Response
```csharp
        ServiceResultType.Conflict => Conflict(CreateError("CONFLICT", result.Message)),
        ServiceResultType.NotFound => NotFound(CreateError("NOT_FOUND", result.Message)),
        ServiceResultType.Error => BadRequest(CreateError("BAD_REQUEST", result.Message)),
        ServiceResultType.ValidationError => UnprocessableEntity(CreateError("VALIDATION_ERROR", result.Message)),
        ServiceResultType.ServiceUnavailable => StatusCode(StatusCodes.Status503ServiceUnavailable, CreateError("INTERNAL_ERROR", result.Message)),
        _ => StatusCode(500, CreateError("INTERNAL_ERROR", result.Message)),
        
        private object CreateError(string code, string? message)
        => new
        {
            Code = code,
            Message = message ?? "An error occurred",
            TraceId = HttpContext.TraceIdentifier
        };
```
