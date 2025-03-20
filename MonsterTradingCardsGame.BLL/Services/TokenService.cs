using MonsterTradingCardsGame.BLL.Exceptions;
using MonsterTradingCardsGame.DTOs;
using MonsterTradingCardsGame.DAL.Repositories;
using MonsterTradingCardsGame.DAL.Exceptions;

namespace MonsterTradingCardsGame.BLL.Services
{
    public class TokenService : ITokenService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenRepository _tokenRepository;

        public TokenService(ITokenRepository tokenRepository, IUserRepository userRepository)
        {
            _tokenRepository = tokenRepository;
            _userRepository = userRepository;
        }

        public string Login(UserCredentialsDTO userCredentials)
        {
            var user = _userRepository.GetUserCredentials(userCredentials.Username);

            if (!BCrypt.Net.BCrypt.Verify(userCredentials.Password, user.Password))
            {
                throw new InvalidCredentialsException("Invalid username/password provided");
            }

            var token = userCredentials.Username + "-mtcgToken";
            var userToken = new TokenDTO(userCredentials.Username, token);

            _tokenRepository.SaveOrUpdateToken(userToken);
            return token;
        }

        public void ValidateToken(string? authorizationHeader, string targetUsername)
        {
            // Check if the header exists and follows the expected format
            if (authorizationHeader == null || !authorizationHeader.StartsWith("Bearer "))
            {
                throw new UnauthorizedAccessException("Access token is missing or invalid");
            }

            // Extract token from authorization header
            var token = authorizationHeader.Substring("Bearer ".Length).Trim();

            try
            {
                // Retrieve the stored token for the target username from the database
                var storedToken = _tokenRepository.GetToken(targetUsername);
                if (storedToken == null || storedToken != token)
                {
                    throw new UnauthorizedAccessException("Access token is missing or invalid");
                }
            }
            catch (TokenNotFoundException)
            {
                // Handle cases where no token exists for the target user
                throw new UnauthorizedAccessException("Access token is missing or invalid");
            }

            // Check if the token in the request matches the stored token and if the user is authorized
            var requestingUsername = token.Split('-')[0];
            if (requestingUsername != targetUsername && requestingUsername != "admin")
            {
                throw new UnauthorizedAccessException("Access token is missing or invalid");
            }
        }

        public void ValidateAdminToken(string? authorizationHeader)
        {
            // Check if the header exists and follows the expected format
            if (authorizationHeader == null || !authorizationHeader.StartsWith("Bearer "))
            {
                throw new UnauthorizedAccessException("Access token is missing or invalid");
            }

            // Extract token from authorization header
            var token = authorizationHeader.Substring("Bearer ".Length).Trim();
            var requestingUsername = token.Split('-')[0];

            // Validate token existence and check if user is "admin"
            var storedToken = _tokenRepository.GetToken("admin");
            if (storedToken == null || storedToken != token || requestingUsername != "admin")
            {
                throw new UnauthorizedAccessException("Access token is missing or invalid");
            }
        }

        public string GetUsernameFromToken(string? authorizationHeader)
        {
            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                throw new UnauthorizedAccessException("Access token is missing or invalid");
            }

            // Extract username from token (assuming format 'username-tokenString')
            var token = authorizationHeader.Substring("Bearer ".Length).Trim();
            var username = token.Split('-')[0];
            return username;
        }
    }
}
