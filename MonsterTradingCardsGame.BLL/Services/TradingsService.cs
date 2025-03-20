using MonsterTradingCardsGame.BLL.Enums;
using MonsterTradingCardsGame.BLL.Exceptions;
using MonsterTradingCardsGame.BLL.Models;
using MonsterTradingCardsGame.DAL.Exceptions;
using MonsterTradingCardsGame.DAL.Repositories;
using MonsterTradingCardsGame.DTOs;

namespace MonsterTradingCardsGame.BLL.Services
{
    public class TradingsService : ITradingsService
    {
        private readonly ITradingsRepository _tradingsRepository;
        private readonly ICardRepository _cardRepository;

        public TradingsService(ITradingsRepository tradingsRepository, ICardRepository cardRepository)
        {
            _tradingsRepository = tradingsRepository;
            _cardRepository = cardRepository;
        }

        public void CreateTradingDeal(string username, TradingDealDTO tradingDeal)
        {
            try
            {
                // Check if the card exists
                _cardRepository.CardExists(tradingDeal.CardToTrade);

                // Check if the card is available
                if (!CheckIfCardIsAvailable(username, tradingDeal.CardToTrade))
                {
                    throw new CardNotAvailableException("The deal contains a card that is not owned by the user or locked in the deck.");
                }

                // Save the new Trading Deal to the database
                _tradingsRepository.SaveTradingDeal(username, tradingDeal);
            }
            catch (TradingDealIdAlreadyTakenException)
            {
                throw new TradingDealAlreadyExistsException("A deal with this deal ID already exists.");
            }
            catch (DataAccessException ex)
            {
                throw new InvalidTradingDealException("An error occurred while updating user data.", ex);
            }
        }

        public List<TradingDealDTO> GetAvailableTradingDeals()
        {
            return _tradingsRepository.GetAvailableTradingDeals();
        }

        public void DeleteTradingDeal(string username, string tradingDealId)
        {
            try
            {
                _tradingsRepository.DeleteTradingDeal(username, tradingDealId);
            }
            catch (CardNotFromUserException)
            {
                throw new ForbiddenAccessException("The deal contains a card that is not owned by the user.");
            }
            catch (TradingDealDoesNotExistException)
            {
                throw new TradingDealNotFoundException("The provided deal ID was not found.");
            }
            catch (DataAccessException ex)
            {
                throw new InvalidTradingDealException("An error occurred while updating user data.", ex);
            }
        }

        public void ExecuteTradingDeal(string username, string tradingDealId, string offeredCardId)
        {
            try
            {
                // 1) Check if the offered card is available
                if (!CheckIfCardIsAvailable(username, offeredCardId))
                {
                    throw new CardNotAvailableException(
                        "The deal contains a card that is not owned by the user or locked in the deck.");
                }

                // 2) Get the Trading Deal and the offered Card
                var tradingDeal = _tradingsRepository.GetTradingDealById(tradingDealId);
                var offeredCard = new Card(_cardRepository.GetCardById(offeredCardId));

                // 3) Check if the offered card meets the requirements of the Trading Deal
                ValidateOfferedCardAgainstDeal(tradingDeal, offeredCard);

                // 4) Retrieve user IDs for the buyer and offering user
                var buyerUserId = _tradingsRepository.GetUserId(username);
                var offeringUserId = _tradingsRepository.GetOfferingUserId(tradingDealId);

                // 5) Ensure the buyer is not trading with themselves
                if (buyerUserId == offeringUserId)
                {
                    throw new ForbiddenTradeException("Users cannot trade with themselves.");
                }

                // 6) Execute the trading deal
                _tradingsRepository.TradingDealTransaction(
                    username, buyerUserId, offeringUserId,
                    tradingDealId, offeredCardId, tradingDeal
                );
            }
            catch (ForbiddenTradeException)
            {
                throw new TradingRequirementsNotMetException("Cannot trade with yourself!");
            }
            catch (CardDoesNotExistException)
            {
                throw new InvalidTradingDealException("The provided Card does not exist.");
            }
            catch (TradingDealDoesNotExistException)
            {
                throw new TradingDealNotFoundException("The provided deal ID was not found.");
            }
            catch (DataAccessException ex)
            {
                throw new InvalidTradingDealException("An error occurred while updating user data.", ex);
            }
        }
        private void ValidateOfferedCardAgainstDeal(TradingDealDTO tradingDeal, Card offeredCard)
        {
            // "Monster" means any non-spell card; "Spell" means CardType.Spell
            var isOfferedCardMonster = offeredCard.Type != CardType.Spell;
            var isOfferedCardSpell = offeredCard.Type == CardType.Spell;

            // 1) Check trade type requirement
            if (tradingDeal.Type.Equals("Monster", StringComparison.OrdinalIgnoreCase))
            {
                if (!isOfferedCardMonster)
                    throw new TradingRequirementsNotMetException("Offered card is not a Monster, but the trade requires a Monster.");
            }
            else if (tradingDeal.Type.Equals("Spell", StringComparison.OrdinalIgnoreCase))
            {
                if (!isOfferedCardSpell)
                    throw new TradingRequirementsNotMetException("Offered card is not a Spell, but the trade requires a Spell.");
            }
            else
            {
                // If the trading deal type is something else, reject it
                throw new TradingRequirementsNotMetException("The provided trading deal type is invalid.");
            }

            // 2) Check minimum damage requirement
            if (offeredCard.Damage < tradingDeal.MinimumDamage)
            {
                throw new TradingRequirementsNotMetException("Offered card damage is too low for this trade.");
            }
        }

        private bool CheckIfCardIsAvailable(string username, string cardId)
        {
            // Fetch user's cards and deck
            var userCards = _cardRepository.GetUserCards(username);
            var userDeck = _cardRepository.GetUserDeck(username);

            // Check if the card is owned by the user and not locked in the deck
            var userOwnsCard = userCards.Any(card => card.Id == cardId);
            var cardIsInDeck = userDeck.Cards.Any(card => card.Id == cardId);

            // Card is available if the user owns it and it is not in the deck
            return userOwnsCard && !cardIsInDeck;
        }
    }
}
