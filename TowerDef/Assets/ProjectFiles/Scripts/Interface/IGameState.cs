using System.Threading;

namespace TD.Interfaces {
    public interface IGameState { 
        int Lives { get; }
        bool IsGameOver { get; }
        CancellationToken GameToken { get; }
        void LoseLife(int amount = 1);
    }
}
