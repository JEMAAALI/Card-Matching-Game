using System;

public static class EventBus
{
    public static event Action OnGameOver;
    public static event Action OnWin;
    public static event Action OnMatch;

    // New: card flipped
    public static event Action<CardsController> OnCardFlipped;

    public static void RaiseGameOver() => OnGameOver?.Invoke();
    public static void RaiseWin() => OnWin?.Invoke();
    public static void RaiseMatch() => OnMatch?.Invoke();

    public static void RaiseCardFlipped(CardsController card) => OnCardFlipped?.Invoke(card);
}
