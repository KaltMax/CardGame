using MonsterTradingCardsGame.BLL.Exceptions;
using System.Collections.Concurrent;

namespace MonsterTradingCardsGame.BLL.Models
{
    public class Lobby
    {
        private readonly ConcurrentQueue<User> _waitingPlayers = new();
        private readonly ConcurrentDictionary<User, TaskCompletionSource<Dictionary<string, string>>> _battleLogs = new();
        private readonly object _lock = new();

        public Dictionary<string, string> EnterLobby(User player)
        {
            User? opponent;
            TaskCompletionSource<Dictionary<string, string>>? playerTcs = null;

            lock (_lock)
            {
                // Check if the player is already in the lobby or in a battle
                if (_battleLogs.Keys.Any(p => p.Name == player.Name) || _waitingPlayers.Any(p => p.Name == player.Name))
                {
                    throw new PlayerAlreadyInLobbyOrBattleException("Player is already in the lobby or in a battle.");
                }

                // Check for an available opponent
                if (!_waitingPlayers.TryDequeue(out opponent))
                {
                    // No opponent available; enqueue this player and wait for an opponent
                    playerTcs = new TaskCompletionSource<Dictionary<string, string>>();
                    if (!_battleLogs.TryAdd(player, playerTcs))
                    {
                        throw new BattleFailedException($"Failed to register player {player.Name} in the lobby.");
                    }

                    _waitingPlayers.Enqueue(player);
                    Console.WriteLine($"{player.Name} is waiting for an opponent...");
                }
            }

            // If there is already a player in the lobby, start the battle
            if (opponent != null)
            {
                return StartBattle(player, opponent);
            }

            // If the player is enqueued, wait for an opponent and the battle result
            if (playerTcs == null)
            {
                throw new BattleFailedException($"Failed to find a battle result for player {player.Name}.");
            }

            try
            {
                return playerTcs.Task.Result; // Blocking call
            }
            finally
            {
                // Clean up after the player leaves the lobby
                _battleLogs.TryRemove(player, out _);
            }
        }

        private Dictionary<string, string> StartBattle(User player, User opponent)
        {
            Console.WriteLine($"{player.Name} is pairing with {opponent.Name} for a battle.");

            // Ensure TaskCompletionSources exist for both players
            var playerTcs = _battleLogs.GetOrAdd(player, new TaskCompletionSource<Dictionary<string, string>>());
            var opponentTcs = _battleLogs.GetOrAdd(opponent, new TaskCompletionSource<Dictionary<string, string>>());

            // Run the battle logic in a separate thread
            Task.Run(() =>
            {
                try
                {
                    var battle = new Battle(opponent, player);
                    battle.StartBattle();

                    var result = battle.BattleLog;

                    // Notify both players with the battle result
                    playerTcs.TrySetResult(result);
                    opponentTcs.TrySetResult(result);
                }
                catch (Exception ex)
                {
                    // Handle any exceptions during the battle
                    var exception = new BattleFailedException($"Battle failed: {ex.Message}", ex);
                    playerTcs.TrySetException(exception);
                    opponentTcs.TrySetException(exception);
                }
                finally
                {
                    // Ensure both players are removed from the battle logs
                    _battleLogs.TryRemove(player, out _);
                    _battleLogs.TryRemove(opponent, out _);
                }
            });

            return playerTcs.Task.Result; // Blocking call
        }
    }
}
