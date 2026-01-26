# CI/CD
## Configuration
Configuration is handled via environment variables.
Supported environment variables (in docker-compose):
```yaml
ConnectionStrings__DefaultConnection: "Server=db;Database=TemplateDb;User Id=sa;Password=YourStrong@Pass123"
Redis__ConnectionString: "redis:6379"
Redis__InstanceName: "prod:"
JWT__ValidAudience: "https://yourdomain.com"
JWT__ValidIssuer: "https://api.yourdomain.com"
JWT__Secret: "SUPER_SECRET_KEY"
JWT__TokenValidityInMinutes: "30"
JWT__RefreshTokenValidity: "7"
EmailConfiguration__UserName: "your@gmail.com"
EmailConfiguration__Password: "your-email-app-password"
```
These variables override settings in appsettings.json using ASP.NET Core’s configuration system.
## Run with Docker Compose
docker-compose.yml defines services:
```yaml
version: "3.9"
services:
  api:
    image: yourdockerhub/template-api:latest
    container_name: template-api
    ports:
      - "8080:8080"
    environment:
      ASPNETCORE_ENVIRONMENT: Production
      ConnectionStrings__DefaultConnection: "Server=db;Database=TemplateDb;User Id=sa;Password=YourStrong@Pass123"
      Redis__ConnectionString: "redis:6379"
      Redis__InstanceName: "prod:"
      JWT__ValidAudience: "https://yourdomain.com"
      JWT__ValidIssuer: "https://api.yourdomain.com"
      JWT__Secret: "SUPER_SECRET_KEY"
      JWT__TokenValidityInMinutes: "30"
      JWT__RefreshTokenValidity: "7"
      EmailConfiguration__SmtpServer: "smtp.gmail.com"
      EmailConfiguration__Port: "465"
      EmailConfiguration__UserName: "your@gmail.com"
      EmailConfiguration__Password: "your-email-app-password"
      EmailConfiguration__ApplicationName: "TEMPLATE"
    depends_on:
      - db
      - redis
  db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      SA_PASSWORD: "YourStrong@Pass123"
      ACCEPT_EULA: "Y"
    ports:
      - "1433:1433"
  redis:
    image: redis:7
    ports:
      - "6379:6379"
```

Run: docker compose up -d

Visit API at: http://localhost:8080

## Docker Build
You can also build the image manually:
docker build -t yourdockerhub/template-api:latest .
## Unit Tests
Run unit tests locally: dotnet test UnitTest
All business logic should be covered by unit tests.
## CI/CD with GitHub Actions
The project includes a CI/CD pipeline located at: .github/workflows/ci-cd.yml  
Pipeline stages:
1. Check out source
2. Restore dependencies
3. Build solution
4. Run unit tests
5. Build Docker image
6. Login to Docker Hub
7. Push the image
Required GitHub Secrets:
- DOCKER_USERNAME
- DOCKER_PASSWORD

Example workflow snippet:
```yaml
jobs:
  build-test:
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: 9.0.x
      - run: dotnet restore Template.sln
      - run: dotnet build Template.sln -c Release
      - run: dotnet test UnitTest -c Release

  docker:
    needs: build-test
    steps:
      - uses: actions/checkout@v4
      - uses: docker/login-action@v3
        with:
          username: ${{ secrets.DOCKER_USERNAME }}
          password: ${{ secrets.DOCKER_PASSWORD }}
      - run: |
          docker build -t ${{ secrets.DOCKER_USERNAME }}/template-api:latest .
          docker push ${{ secrets.DOCKER_USERNAME }}/template-api:latest
```