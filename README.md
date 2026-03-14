# Monster Trading Cards Game

## Project Overview

The **Monster Trading Cards Game** is a REST-based server application for a card-based battle and trading game. The project is implemented in **C#/.NET 10**, following a layered architecture (API, BLL, DAL) with PostgreSQL as the database. It provides a platform where users can register, acquire cards, create decks, trade cards, and engage in battles against each other.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker](https://www.docker.com/get-started) (for PostgreSQL)

## Getting Started

### 1. Start the Database

Run the PostgreSQL container:

```bash
docker run -d --rm --name cardgame \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -p 5432:5432 \
  -v pgdata:/var/lib/postgresql/data \
  postgres
```

Then create the database:

```bash
docker exec -it cardgame psql -U postgres -c "CREATE DATABASE cardgame;"
```

### 2. Initialize the Schema

Apply the database schema:

```bash
docker exec -i cardgame psql -U postgres -d cardgame < Database/database.sql
```

### 3. Build & Run

```bash
dotnet build
dotnet run --project MonsterTradingCardsGame.API
```

The server starts on **http://localhost:10001**.

### 4. Run Tests

Unit tests:

```bash
dotnet test
```

Integration tests (requires the server to be running):

```bash
cd IntegrationTests
MonsterTradingCards.exercise.complete.curl.bat
```

## Architecture

The application follows a **three-layered architecture**:

```
MonsterTradingCardsGame.API     → HTTP server, routing, request handlers
MonsterTradingCardsGame.BLL     → Business logic, services, battle engine
MonsterTradingCardsGame.DAL     → Database access, repositories (Npgsql)
MonsterTradingCardsGame.DTOs    → Shared data transfer objects
MonsterTradingCardsGame.Tests   → Unit tests (NUnit + NSubstitute)
```

## Features

- **User Management**: Register and authenticate users via token-based auth.
- **Card Management**: Acquire and manage monster and spell cards.
- **Deck Configuration**: Select four best cards for battles.
- **Trading System**: Trade cards based on type and damage requirements.
- **Battle System**: Compete against other players with your deck (ELO-rated).
- **Scoreboard**: Track rankings based on ELO rating.
- **Profile Customization**: Editable user profiles.
- **Friend System**: Send, accept, and manage friend requests.
- **Parallel Processing**: Multi-threaded request handling with task-based concurrency.

## Technologies Used

- **C# / .NET 10** (Backend)
- **PostgreSQL** (Database)
- **Npgsql** (PostgreSQL driver)
- **BCrypt.Net** (Password hashing)
- **Custom HTTP Server** (No external frameworks for HTTP handling)
- **Docker** (Containerized database)
- **NUnit / NSubstitute** (Unit testing)

## Battle System

- **Monster Cards**: Pure damage comparison.
- **Spell Cards**: Element-based damage multipliers.
- **Elemental Effectiveness**:
  - **Water > Fire** (damage doubled)
  - **Fire > Normal** (damage doubled)
  - **Normal > Water** (damage doubled)
  - Reversed matchups halve damage. Same element = direct comparison.
- **Special Rules**:
  - Goblins fear Dragons.
  - Wizards control Orks.
  - Knights drown against WaterSpells.
  - Kraken is immune to spells.
  - FireElves evade Dragon attacks.
- **Win Condition**: The player with no cards left loses.
- **Draw Condition**: After 100 rounds, ELO remains unchanged.

## Trading System

- Players can **offer** a card for trade with requirements (card type, minimum damage).
- Cards in active trades **cannot be used in a deck**.

## API Documentation

The full API specification is available in [`mtcg-api-with-friends.yaml`](mtcg-api-with-friends.yaml) (OpenAPI 3.0).

## Testing

### Unit Tests (~60 tests)

Covering API request handling, business logic (battles, matchmaking, trade validation), and database operations.

### Integration Tests

Automated CURL scripts in `IntegrationTests/` covering the full game flow:
user registration, card acquisition, deck management, trading, battles, scoreboard, and friend system.
