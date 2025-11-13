using System;

public static class EventBus
{
    public static event Action OnMatch;

    // New: card flipped
    public static event Action<CardsController> OnCardFlipped;

 
    public static void RaiseMatch() => OnMatch?.Invoke();

    public static void RaiseCardFlipped(CardsController card) => OnCardFlipped?.Invoke(card);
}
