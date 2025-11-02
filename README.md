# FindFi — Vertical Slice (DAL → BLL → Web/API)

Цей репозиторій містить приклад вертикального зрізу для домену «пошук та оренда квартир/будинків без рієлторів». Реалізовано тришарову архітектуру: Data Access Layer (ADO.NET + Dapper) → Business Logic Layer → Web/API з ProblemDetails та OpenAPI/Swagger.

## Зміст
- Огляд
- Архітектура/Технології
- Вимоги
- Швидкий старт
  - Варіант A: через .NET Aspire (рекомендовано)
  - Варіант B: локальний запуск API
- Конфігурація (ConnectionStrings, змінні середовища)
- База даних (мінімальна схема для прикладу)
- Перевірка роботи API (Swagger, curl приклади)
- Тести
- Усунення несправностей

## Огляд
Реалізовано CRUD для сутності Product як демонстраційний вертикальний зріз:
- DAL: репозиторій Product на Dapper з параметризацією, Unit of Work з транзакціями (MySQL).
- BLL: сервіси, DTO, AutoMapper, бізнес-валидація та доменні винятки.
- Web/API: тонкі контролери, атрибутна маршрутизація, коректні HTTP-статуси, глобальний middleware винятків (ProblemDetails), Swagger/OpenAPI.

## Архітектура/Технології
- .NET 9
- MySQL (через пакет MySqlConnector)
- Dapper (читання/операції), чистий ADO.NET для одного репозиторію
- AutoMapper для мапінгу DTO
- ProblemDetails (RFC7807) для помилок
- Swagger/OpenAPI для документації
- .NET Aspire AppHost для локальної оркестрації залежностей (MySQL + API)

## Вимоги
- .NET SDK 9.0+
- Docker (для запуску через Aspire)
- Доступ до MySQL (локально або в контейнері)

## Швидкий старт
### Варіант A: через .NET Aspire (Docker)
1. Переконайтесь, що Docker запущений.
2. Зробіть стартовим проектом FindFi.AppHost або виконайте:
   - Linux/macOS:
     ```bash
     dotnet run -p FindFi.AppHost/FindFi.AppHost.csproj
     ```
   - Windows PowerShell:
     ```powershell
     dotnet run -p .\FindFi.AppHost\FindFi.AppHost.csproj
     ```
3. AppHost підніме контейнер MySQL (mysql:8.4) і Web/API, інжектує у нього рядок підключення через змінну середовища `ConnectionStrings__Default`.
4. У консолі буде виведений URL API (або відкрийте Swagger за адресою нижче).

Примітки:
- У AppHost зафіксовано orchestrator Docker Compose і вимкнений вбудований Aspire Dashboard (щоб не вимагати додаткових залежностей). Ви можете додати Dashboard пізніше за потребою.

### Варіант B: локальний запуск API
1. Налаштуйте підключення до MySQL у файлі FindFi/appsettings.Development.json (ключ `ConnectionStrings:DB1`) або через змінні середовища (див. нижче).
2. Запустіть API:
   - Linux/macOS:
     ```bash
     dotnet run -p FindFi/FindFi.csproj
     ```
   - Windows PowerShell:
     ```powershell
     dotnet run -p .\FindFi\FindFi.csproj
     ```
3. За замовчуванням Swagger доступний у середовищі Development.

## Конфігурація
Додаток підтримує обидва ключі підключення: `ConnectionStrings:DB1` та `ConnectionStrings:Default` (для сумісності з Aspire). Порядок пошуку:
1. `ConnectionStrings:DB1` з конфігурації
3. змінна середовища `ConnectionStrings__DB1`
4. змінна середовища `ConnectionStrings__Default`

Приклади встановлення змінних середовища:
- Linux/macOS (bash):
  ```bash
  export ASPNETCORE_ENVIRONMENT=Development
  export ConnectionStrings__DB1="Server=localhost;Port=3306;Database=app;User ID=root;Password=12345678;SslMode=None;"
  ```
- Windows PowerShell:
  ```powershell
  $env:ASPNETCORE_ENVIRONMENT="Development"
  $env:ConnectionStrings__DB1="Server=localhost;Port=3306;Database=app;User ID=root;Password=12345678;SslMode=None;"
  ```

У складі репозиторію файл FindFi/appsettings.Development.json вже містить приклад з'єднання до MySQL на localhost.

## База даних (мінімальна схема для прикладу)
Репозиторій Product очікує таблицю `Product` з полями `Id`, `Name`, `Price`. Створіть її у вашій БД MySQL:
```sql
CREATE TABLE IF NOT EXISTS Product (
  Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  Name VARCHAR(200) NOT NULL,
  Price DECIMAL(18,2) NOT NULL
);
```
Якщо ви використовуєте власну схему — оновіть SQL відповідно або адаптуйте репозиторій.

## Перевірка роботи API
Після запуску скористайтесь одним із способів:

### 1) Swagger UI
- URL Swagger UI (Development): `https://localhost:5001/swagger` або `http://localhost:5000/swagger` (точний порт залежить від запуску).
- OpenAPI JSON: `/swagger/v1/swagger.json`
- Альтернативний OpenAPI (Minimal API docs .NET 9): `/openapi/v1.json`

### 2) curl приклади
Припустимо базова адреса — `http://localhost:5000` (замініть на вашу):

- Отримати всі продукти:
  ```bash
  curl -s http://localhost:5000/api/products | jq
  ```

- Створити продукт:
  ```bash
  curl -s -X POST http://localhost:5000/api/products \
    -H "Content-Type: application/json" \
    -d '{"name":"Sofa","price":199.99}' | jq
  ```

- Отримати продукт за Id:
  ```bash
  curl -s http://localhost:5000/api/products/1 | jq
  ```

- Оновити продукт:
  ```bash
  curl -s -X PUT http://localhost:5000/api/products/1 \
    -H "Content-Type: application/json" \
    -d '{"name":"Sofa XL","price":249.99}' -i
  ```

- Видалити продукт:
  ```bash
  curl -s -X DELETE http://localhost:5000/api/products/1 -i
  ```

Очікувані статуси:
- 201 Created — для POST
- 200 OK — для GET
- 204 No Content — для PUT/DELETE
- 400 Bad Request — помилки валідації (ProblemDetails з `extensions.errors`)
- 404 Not Found — ресурс відсутній
- 409 Conflict — бізнес-конфлікт

Приклад тілa помилки (ProblemDetails):
```json
{
  "type": "about:blank",
  "title": "Validation failed",
  "status": 400,
  "detail": "Product validation failed",
  "errors": {
    "Price": ["Price must be non-negative"]
  }
}
```

## Тести
Запустіть юніт-тести (перевірка конфігурації AutoMapper):
```bash
dotnet test
```

## Усунення несправностей
- Swagger не відкривається: перевірте, що `ASPNETCORE_ENVIRONMENT=Development`.
- Немає підключення до БД: перевірте рядок підключення (host/порт/логін/пароль), чи запущений MySQL (локально або через AppHost). Для Aspire рядок підключення інжектується автоматично як `ConnectionStrings__Default`.
- Помилки під час INSERT/UPDATE: переконайтесь, що створена таблиця `Product` з потрібними стовпцями.

## Структура рішення
- FindFi.Domain — доменні моделі та винятки
- FindFi.Dal — інтерфейси репозиторіїв, реалізації (ADO.NET/Dapper), Unit of Work, DI
- FindFi.Bll — DTO, AutoMapper профілі, сервіси, DI
- FindFi — Web/API (контролери, middleware, конфігурація, Swagger)
- FindFi.AppHost — .NET Aspire AppHost (MySQL + API)
- FindFi.Tests — тести (AutoMapper)
