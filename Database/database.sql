-- Bash-Command for creating the databse:
-- docker run -d --rm --name swen1mtcg -e POSTGRES_USER=if23b013 -e POSTGRES_PASSWORD=postgres -p 5432:5432 -v pgdata:/var/lib/postgresql/data postgres

-- Bash-Command for connecting to the database:
-- docker exec -it swen1mtcg psql -U if23b013

-- Bash-Command for creating the database:
-- CREATE DATABASE swen1mtcg;

-- Move into the database swen1mtcg:
-- \c swen1mtcg

-- Execute script:

-- Create the Users table
CREATE TABLE IF NOT EXISTS Users (
    UserId SERIAL PRIMARY KEY,
    Username VARCHAR(50) NOT NULL UNIQUE,
    Password VARCHAR(100) NOT NULL,
    Coins DECIMAL,
    Elo INT,
    Wins INT,
    Losses INT,
    Name VARCHAR(100),
    Bio TEXT,
    ProfilePicture TEXT
);

-- Create the Tokens table
CREATE TABLE IF NOT EXISTS Tokens (
    TokenId SERIAL PRIMARY KEY,
    Username VARCHAR(50) UNIQUE NOT NULL,
    Token VARCHAR(255) UNIQUE NOT NULL
);

-- Create the Cards table
CREATE TABLE IF NOT EXISTS Cards (
    CardId VARCHAR(50) PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Damage FLOAT NOT NULL,
    UserId INT REFERENCES Users(UserId) ON DELETE SET NULL,
    InDeck BOOLEAN DEFAULT FALSE
);

-- Create the Packages table
CREATE TABLE IF NOT EXISTS Packages (
    PackageId SERIAL PRIMARY KEY,
    IsAvailable BOOLEAN DEFAULT TRUE,
    Card1Id VARCHAR(50) REFERENCES Cards(CardId) ON DELETE CASCADE,
    Card2Id VARCHAR(50) REFERENCES Cards(CardId) ON DELETE CASCADE,
    Card3Id VARCHAR(50) REFERENCES Cards(CardId) ON DELETE CASCADE,
    Card4Id VARCHAR(50) REFERENCES Cards(CardId) ON DELETE CASCADE,
    Card5Id VARCHAR(50) REFERENCES Cards(CardId) ON DELETE CASCADE
);

-- Create the Trades table
CREATE TABLE IF NOT EXISTS Trades (
    TradeId VARCHAR(50) PRIMARY KEY,
    OfferingUserId INT REFERENCES Users(UserId) ON DELETE CASCADE,
    CardToTradeId VARCHAR(50) REFERENCES Cards(CardId) ON DELETE CASCADE,
    RequestedCardType VARCHAR(50),
    RequestedMinDamage FLOAT,
    IsTradeActive BOOLEAN DEFAULT TRUE
);

-- Create the Friends table
CREATE TABLE Friends (
    FriendshipId SERIAL PRIMARY KEY,
    UserId1 INT REFERENCES Users(UserId) ON DELETE CASCADE, --- sender
    UserId2 INT REFERENCES Users(UserId) ON DELETE CASCADE, --- receipient
    Status VARCHAR(20) DEFAULT 'pending' -- 'pending', 'accepted'
);
