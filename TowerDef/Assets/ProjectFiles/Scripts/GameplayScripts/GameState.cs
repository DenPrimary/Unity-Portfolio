using System.Threading;
using TD.Interfaces;

namespace TD.Gameplay {
    public class GameState : IGameState {
        private readonly CancellationTokenSource _cts = new();
        private int _lives = 10;

        public int Lives => _lives;
        public bool IsGameOver => _lives <= 0;
        public CancellationToken GameToken => _cts.Token;

        public void LoseLife(int amount = 1) {
            if (IsGameOver) return;
            _lives -= amount;
            if (_lives <= 0) {
                _lives = 0;
                _cts.Cancel();
            }
        }
    }
}
