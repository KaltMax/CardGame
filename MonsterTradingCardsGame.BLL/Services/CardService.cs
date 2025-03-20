using MonsterTradingCardsGame.BLL.Exceptions;
using MonsterTradingCardsGame.DAL.Repositories;
using MonsterTradingCardsGame.DAL.Exceptions;
using MonsterTradingCardsGame.DTOs;

namespace MonsterTradingCardsGame.BLL.Services
{
    public class CardService : ICardService
    {
        private readonly ICardRepository _cardRepository;

        public CardService(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }

        public List<CardDTO> GetUserCards(string username)
        {
            try
            {
                return _cardRepository.GetUserCards(username);
            }
            catch (DataAccessException ex)
            {
                throw new CardRetrievalException("An error occurred while retrieving the user's cards.", ex);
            }
        }

        public UserDeckDTO GetUserDeck(string username)
        {
            try
            {
                return _cardRepository.GetUserDeck(username);
            }
            catch (DataAccessException ex)
            {
                throw new DeckRetrievalException("An error occurred while retrieving the user's deck.", ex);
            }
        }

        public bool ConfigureUserDeck(string username, ConfigureDeckRequestDTO cardIds)
        {
            try
            {
                // Validate that the cards exists
                foreach (var cardId in cardIds.CardIds)
                {
                    _cardRepository.CardExists(cardId);
                }

                // Validate that the provided cards belong to the user
                var userCards = _cardRepository.GetUserCards(username);
                foreach (var cardId in cardIds.CardIds)
                {
                    if (userCards.All(c => c.Id != cardId))
                    {
                        throw new CardNotAvailableException("At least one of the provided cards does not belong to the user or is not available");
                    }
                }

                // Proceed with deck configuration
                return _cardRepository.ConfigureUserDeck(username, cardIds);
            }
            catch (CardDoesNotExistException ex)
            {
                throw new CardNotAvailableException("At least one of the provided cards does not belong to the user or is not available.", ex);
            }
            catch (CardNotFromUserException ex)
            {
                throw new CardNotAvailableException("At least one of the provided cards does not belong to the user or is not available.", ex);
            }
            catch (DataAccessException ex)
            {
                throw new ConfigureDeckException("An error occurred while configuring the user's deck.", ex);
            }
        }
    }
}
