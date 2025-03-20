﻿namespace MonsterTradingCardsGame.DTOs
{
    public class UserCredentialsDTO
    {
        public string Username { get; init; }
        public string Password { get; init; }

        public UserCredentialsDTO(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}
