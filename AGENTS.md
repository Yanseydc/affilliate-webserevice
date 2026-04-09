# Rules

- Do not change public API routes
    - Keep Minimal APIs
    - Do not introduce CQRS, MediatR, or complex architecture
    - Keep DTO mapping manual
    - Always preserve /r/{articleId}/{productId} behavior
    - Do not change response shape of GET /api/articles/{slug}

# Dev commands

- Run backend: dotnet run
    - Migrations:
- dotnet ef migrations add <Name>
    - dotnet ef database update

# Notes

    - Language supported: en, es
    - Articles grouped by TranslationGroupId
    - Products are shared across languages