# PIM-2025-2

Descrição
---------
PIM-2025-2 é uma solução .NET 9 organizada como uma solução única (PIM.sln) com uma API REST em C# (pasta `API`) e uma aplicação cliente MAUI desktop (pasta `App`). Há um projeto de testes (`API.Tests`) e recursos estáticos/frontend em `Web`. Este README contém instruções concretas para clonar, configurar, compilar, executar e contribuir no repositório.

Visão geral da estrutura
------------------------
- PIM.sln — solução .NET que agrega os projetos
- API/ — backend ASP.NET Core (TargetFramework: net9.0)
  - API.csproj
  - Program.cs
  - appsettings.json / appsettings.Development.json
  - API.http — coleção de requisições para teste (VS Code REST Client)
  - test_payload_user.json — payload JSON de exemplo
  - runlog.txt — arquivo de log (repositório contém um log grande)
  - Pastas: Endpoints, Models, Services, Migrations, Hubs, Logging, wwwroot, Data, Docs
- App/ — cliente MAUI (desktop only)
- Web/ — recursos HTML/CSS/JS servidos se necessário
- API.Tests/ — testes automatizados
- Docs/ — documentação complementar (quando presente)
- .gitignore

Tecnologias principais
---------------------
- .NET 9 / C#
- ASP.NET Core (Minimal APIs + endpoints organizados em arquivos)
- Entity Framework Core (SQL Server)
- SignalR (servidor e cliente nas dependências)
- Swashbuckle / OpenAPI (Swagger)
- Scalar.AspNetCore
- SixLabors.ImageSharp
- MAUI para cliente desktop (Windows)

Requisitos (ambiente)
---------------------
- .NET SDK 9 instalado
- Workload MAUI instalado para compilar/rodar o App desktop:
  dotnet workload install maui
- SQL Server acessível (local ou remoto) para a connection string do EF Core
- Visual Studio com suporte MAUI recomendado para desenvolvimento do client desktop
- dotnet-ef (opcional, recomendado para migrações):
  dotnet tool install --global dotnet-ef

Clonar o repositório e preparar
--------------------------------
1. Clone a repo e vá para a branch base:
   git clone https://github.com/fsbertelli/PIM-2025-2.git
   cd PIM-2025-2
   git checkout 120925

2. Restaurar pacotes e compilar a solução:
   dotnet restore PIM.sln
   dotnet build PIM.sln

Configuração (variáveis e secrets)
---------------------------------
- Configure a connection string em `API/appsettings.Development.json` ou via variável de ambiente `ConnectionStrings__DefaultConnection`.
- Remova chaves sensíveis do código. O cliente HTTP "gpt" em `API/Program.cs` contém uma chave que deve ser movida para variáveis de ambiente ou secrets.
- Exemplo mínimo de variáveis:
  ConnectionStrings__DefaultConnection="Server=localhost;Database=PimDb;Trusted_Connection=True;"
  GPT__ApiKey="REPLACE_WITH_SECRET"

Banco de dados e migrações (EF Core)
------------------------------------
1. Configure ConnectionStrings.
2. Aplicar migrações:
   dotnet ef database update --project API/API.csproj --startup-project API

Executando a API (desenvolvimento)
----------------------------------
1. Defina ambiente para Development se quiser usar appsettings.Development.json:
   PowerShell: $env:ASPNETCORE_ENVIRONMENT = "Development"
   Bash: export ASPNETCORE_ENVIRONMENT=Development

2. Executar:
   dotnet run --project API/API.csproj

3. Observações:
   - Swagger disponível em /swagger.
   - Endpoints úteis:
     - GET /info
     - GET /sniffer -> /sniffer.html
     - GET /logtest
   - Hub SignalR: /loghub
   - Arquivo `API/API.http` tem requisições de exemplo.
   - Para rodar em porta específica:
     dotnet run --project API/API.csproj --urls "http://localhost:5000"

Executando o cliente MAUI (desktop only)
---------------------------------------
- Abra a solução no Visual Studio com suporte MAUI e execute selecionando target Desktop (Windows).
- CLI (após instalar workload):
  dotnet run --project App/App.csproj -f net9.0-windows

Executando testes
-----------------
- Rodar todos os testes:
  dotnet test PIM.sln
- Rodar projeto de testes:
  dotnet test API.Tests

Observações sobre arquivos relevantes
------------------------------------
- API/Program.cs — entrada; configura DB factory, serviços, SignalR, Swagger e endpoints mapeados.
- API/API.csproj — TargetFramework net9.0; referências: EF Core, SignalR, Swashbuckle, ImageSharp, Scalar.AspNetCore.
- API/test_payload_user.json — payload de exemplo.
- API/API.http — coleção de requisições para testes.
- API/runlog.txt — arquivo de log grande; deve ser removido do repositório e adicionado no .gitignore.

Recomendações e boas práticas
----------------------------
- Nunca commitar secrets (use User Secrets, variáveis de ambiente ou Key Vault).
- Remover runlog.txt do repo e adicionar logs a .gitignore.
- Adicionar `.env.example` com variáveis necessárias.
- Configurar CI para rodar `dotnet build` e `dotnet test`.

Contribuindo
------------
1. Crie uma branch a partir de `120925`:
   git checkout -b feat/nome-da-feature
2. Faça commits pequenos e claros.
3. Rode os testes localmente:
   dotnet test
4. Abra um Pull Request contra `120925` com descrição e passos para testar.

Licença e mantenedor
--------------------
- Repositório privado. Adicione LICENSE ao torná-lo público.
- Mantenedor: Felipe Bertelli — https://github.com/fsbertelli
- Issues: https://github.com/fsbertelli/PIM-2025-2/issues

Resumo rápido (comandos úteis)
------------------------------
- Restaurar e compilar:
  dotnet restore PIM.sln && dotnet build PIM.sln
- Rodar API:
  dotnet run --project API/API.csproj
- Aplicar migrações:
  dotnet ef database update --project API/API.csproj --startup-project API
- Rodar testes:
  dotnet test PIM.sln
- Rodar MAUI desktop (CLI):
  dotnet workload install maui
  dotnet run --project App/App.csproj -f net9.0-windows

Observação final
----------------
A backend API está pronta para desenvolvimento local (EF Core + SQL Server, Swagger e SignalR). O cliente é MAUI focado em Desktop (Windows). Configure connection strings e secrets antes de executar.
