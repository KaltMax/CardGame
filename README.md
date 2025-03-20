# Card Game

## Project Overview
The **ards Game** is a REST-based server application for a card-based battle and trading game. The project is implemented in **C#**, following a layered architecture (API, BLL, DAL) with PostgreSQL as the database. It provides a platform where users can register, acquire cards, create decks, trade cards, and engage in battles against each other.

## Features
- **User Management**: Register and authenticate users.
- **Card Management**: Acquire and manage monster and spell cards.
- **Deck Configuration**: Select four best cards for battles.
- **Trading System**: Trade cards based on type and damage requirements.
- **Battle System**: Compete against other players with your deck.
- **Scoreboard**: Track rankings based on **ELO rating**.
- **Profile Customization**: Editable user profiles.
- **Security Measures**: User authentication via tokens and password hashing.
Parallel Processing: The server handles multiple battles, transactions, and requests asynchronously using multi-threading and task-based concurrency.
- **Friend System** for sending, accepting, and managing friend requests.

## Technologies Used
- **C#** (Backend Implementation)
- **PostgreSQL** (Database Management)
- **Custom HTTP Server** (No external frameworks for HTTP handling)
- **Docker** (Containerized PostgreSQL database)
- **Unit Testing**: **NUnit**, **NSubstitute** for mocking

## Architecture
The application follows a **three-layered architecture**:
```plaintext
API → BLL → DAL
```

## Battle System
- **Monster Cards**: Pure damage comparison.
- **Spell Cards**: Element-based damage multipliers.
- **Spell Cards Effectiveness**:
  - **Water → Fire** (Water is effective against Fire, damage doubled)
  - **Fire → Normal** (Fire is effective against Normal, damage doubled)
  - **Normal → Water** (Normal is effective against Water, damage doubled)
  - **Fire → Water** (Fire is ineffective against Water, damage halved)
  - **Water → Normal** (Water is ineffective against Normal, damage halved)
  - **Normal → Fire** (Normal is ineffective against Fire, damage halved)
  - **Same type** (No effect, direct damage comparison)
- **Special Rules**:
  - Goblins fear Dragons.
  - Wizards control Orks.
  - Knights drown against WaterSpells.
  - Kraken is immune to spells.
  - FireElves evade Dragon attacks.
- **Win Condition**: The player with no cards left loses.
- **Draw Condition**: After 100 rounds, ELO remains unchanged.

## Trading System
- Players can **offer** a card for trade.
- Trades include **requirements**:
  - Specific card type (Monster/Spell)
  - Minimum damage value
- Cards in trade **cannot be used in a deck**.

## Friend System
- Players can **send**, **accept**, and **remove** friends.
- Friend-related operations require authentication.
- Stored in a new **Friends** table.

## Unit Tests
- The project includes **~60 unit tests** covering:
  - API request handling
  - Business logic (battles, matchmaking, trade validation)
  - Database operations

## Integration Tests
- **End-to-End Testing**: Ensures that all API endpoints work correctly together.
- **Database Integration**: Tests interactions between the application and PostgreSQL.
- **Authentication and Authorization**: Verifies token handling and security.
- **Game Flow Validation**:
  - User Registration & Login
  - Acquiring Cards & Packages
  - Deck Management
  - Trading & Battles
  - Scoreboard Updates
- **Automated CURL Testing Script**: A comprehensive **batch script** that tests various functionalities including:
  - User Registration & Authentication
  - Card and Deck Management
  - Trading and Battle Logic
  - Profile Updates & Scoreboard Queries
  - Friend System Operations
