# Barber API

API para gestão de barbearia, com foco em:

- cadastro de clientes e barbeiros;
- autenticação básica;
- agenda do barbeiro;
- criação/cancelamento de agendamentos;
- visão de dashboard diário.

O projeto foi construído para estudar arquitetura em camadas no .NET 8, separando regras de negócio, domínio e acesso a dados.

## Por que este projeto existe

A ideia é ter uma base prática para evoluir um sistema de barbearia com:

- fluxo real de agendamento;
- consultas de agenda e métricas de faturamento;
- base para autenticação/autorização futura;
- organização de código que permita crescer sem acoplamento excessivo.

## Arquitetura

Estrutura principal:

- `src/Barber.Domain`
  - Entidades (`User`, `Barber`, `Client`, `Service`, `Order`, etc.)
  - Contratos de repositório (`IUserRepository`, `IBarberRepository`, `IAppointmentRepository`)
- `src/Barber.Application`
  - DTOs de entrada/saída
  - Serviços de aplicação (`AuthService`, `BookingService`, `DashboardService`, etc.)
- `src/Barber.Infrastrecture`
  - `BarberDbContext` (EF Core + PostgreSQL)
  - Implementações de repositório
- `src/Endpoints`
  - Endpoints minimal API organizados por domínio funcional
- `docker-compose.yml`
  - API + PostgreSQL para desenvolvimento local
- `db_initialize.db`
  - Script SQL de criação e seed do banco

## Tecnologias

- .NET 8 (ASP.NET Core Minimal API)
- Entity Framework Core
- PostgreSQL 16
- Docker / Docker Compose
- Swagger (OpenAPI)

Pacotes principais:

- `Npgsql`
- `Npgsql.EntityFrameworkCore.PostgreSQL`
- `EFCore.NamingConventions`

## Pré-requisitos

### Opção recomendada (Docker)

- Docker Desktop instalado e em execução

### Opção alternativa (sem Docker)

- .NET SDK 8+
- PostgreSQL 16+

## Como executar com Docker

Na raiz do projeto:

```bash
docker compose up --build -d
```

Serviços expostos:

- API: `http://localhost:5000`
- Swagger: `http://localhost:5000/swagger`
- PostgreSQL: `localhost:5432`
- RabbitMQ (broker): `localhost:5672`
- RabbitMQ UI: `http://localhost:15672` (guest/guest)

Parar o ambiente:

```bash
docker compose down
```

Parar removendo volume (reset completo do banco):

```bash
docker compose down -v
```

## Configuração de banco

A conexão é injetada via variável de ambiente no `docker-compose.yml`:

- Host: `db`
- Port: `5432`
- Database: `barber_db`
- Username: `admin`
- Password: `password123`

O script `db_initialize.db` cria tabelas e insere dados iniciais (admin, barbeiro, cliente, serviço e barbearia).

## Dados seed úteis para testes

Usuário admin:

- Email: `admin@barber.com`
- Senha: `admin123`

Usuário barbeiro:

- Email: `joao@barber.com`
- Senha: `123456`

Usuário cliente:

- Email: `rafael@email.com`
- Senha: `client123`

## Endpoints disponíveis

### Health

- `GET /`
- `GET /health`
- `GET /db-health`

### Auth

- `POST /auth/login`
- `GET /auth/me`

### Clientes

- `POST /clients/register`

### Barbeiros

- `POST /barbers/register`

### Agenda

- `GET /appointments/{barberId}`

### Agendamentos

- `POST /bookings`
- `PATCH /bookings/{id}/cancel`

### Dashboard

- `GET /dashboard/daily`

### Listagens paginadas

- `GET /shops?page=1&pageSize=10`
- `GET /users?page=1&pageSize=10`
- `GET /barbers?page=1&pageSize=10`
- `GET /clients?page=1&pageSize=10`
- `GET /services?page=1&pageSize=10`
- `GET /appointments?page=1&pageSize=10`
- `GET /orders?page=1&pageSize=10`
- `GET /transactions?page=1&pageSize=10`

Formato de retorno:

```json
{
  "data": [],
  "meta": {
    "currentPage": 1,
    "pageSize": 10,
    "totalItems": 150,
    "totalPages": 15,
    "hasNext": true,
    "hasPrevious": false
  }
}
```

## Exemplos de requisição

### Login

```bash
curl -X POST http://localhost:5000/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@barber.com",
    "password": "admin123"
  }'
```

### Registrar cliente

```bash
curl -X POST http://localhost:5000/clients/register \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Lucas",
    "email": "lucas@email.com",
    "password": "123456",
    "phone": "31999999999",
    "birthDate": "1998-06-20"
  }'
```

### Registrar barbeiro

```bash
curl -X POST http://localhost:5000/barbers/register \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Carlos",
    "email": "carlos@barber.com",
    "password": "123456",
    "specialty": "Degradê",
    "documentId": "DOC-998877"
  }'
```

### Criar agendamento

```bash
curl -X POST http://localhost:5000/bookings \
  -H "Content-Type: application/json" \
  -d '{
    "clientId": "661f9511-f30c-52e5-b827-557766551111",
    "barberId": "550e8400-e29b-41d4-a716-446655440000",
    "serviceId": "SUBSTITUA_PELO_ID_DO_SERVICO",
    "startTime": "2026-03-02T14:00:00-03:00"
  }'
```

### Cancelar agendamento

```bash
curl -X PATCH http://localhost:5000/bookings/{orderId}/cancel
```

### Dashboard diário

```bash
curl http://localhost:5000/dashboard/daily
```

## Como executar sem Docker (opcional)

1. Ajuste a connection string em `appsettings.json` ou variáveis de ambiente.
2. Garanta que o PostgreSQL esteja em execução.
3. Rode a API:

```bash
dotnet restore src/Barber.Api.csproj
dotnet run --project src/Barber.Api.csproj
```

## Observações do estado atual

- O projeto usa minimal APIs e alguns endpoints estão sem versionamento.
- O script SQL inicial está apontado diretamente para `db_initialize.db` no compose.
- A pasta `init.sql/` existe no repositório, mas o compose atual utiliza `db_initialize.db`.
- Senhas estão em texto simples por ser ambiente de estudo/desenvolvimento.
- Ao criar/cancelar agendamento, a API publica eventos na fila `booking-events` do RabbitMQ.

## Melhorias sugeridas (próximos passos)

- Adicionar JWT e autorização por perfil (`admin`, `barber`, `client`).
- Hash de senha (`BCrypt` ou `ASP.NET Identity`).
- Versionamento de API (`/v1`).
- Migrations EF Core no lugar de SQL estático para evolução de schema.
- Testes automatizados (unitários e integração).
- Padronização de respostas de erro (ProblemDetails).
- Padronizar todas as rotas com versionamento e convenção única de recursos.

## Licença

Projeto de estudo pessoal.
