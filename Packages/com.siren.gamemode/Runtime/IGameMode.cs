using Cysharp.Threading.Tasks;

namespace GameMode
{
    public enum GameModeState
    {
        Ended,
        Starting,
        Started,
        Ending
    }

    public interface IGameMode
    {
        GameModeState State { get; }
        
        UniTask OnStartAsync();
        UniTask OnEndAsync();
    }
    
}