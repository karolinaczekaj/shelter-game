using System;

namespace Schron.Core
{
    // Zdarzenia fabularne: wydarzenia z wyborem, ekspedycje, katastrofa, komunikaty.
    public static class GameEvents
    {
        // Story events (odkomentować, gdy powstanie GameEventSO)
        // public static event Action<GameEventSO> OnGameEventTriggered;
        // public static event Action<GameEventSO, int> OnGameEventResolved;

        // Expeditions
        public static event Action<ResidentData> OnExpeditionStarted;
        public static event Action<ResidentData, bool> OnExpeditionReturned;

        // Other shelters (opcjonalnie)
        // public static event Action<string, int> OnShelterRelationChanged;

        // Disaster & Game Over
        public static event Action OnDisasterStarted;
        public static event Action<bool> OnGameOver;

        // Errors & Messages
        public static event Action<string> OnNotification;

        // Methods
        // public static void GameEventTriggered(GameEventSO e) => OnGameEventTriggered?.Invoke(e);
        // public static void GameEventResolved(GameEventSO e, int choice) => OnGameEventResolved?.Invoke(e, choice);
        public static void ExpeditionStarted(ResidentData r) => OnExpeditionStarted?.Invoke(r);
        public static void ExpeditionReturned(ResidentData r, bool ok) => OnExpeditionReturned?.Invoke(r, ok);
        // public static void ShelterRelationChanged(string id, int trust) => OnShelterRelationChanged?.Invoke(id, trust);
        public static void DisasterStarted() => OnDisasterStarted?.Invoke();
        public static void GameOver(bool won) => OnGameOver?.Invoke(won);
        public static void Notify(string msg) => OnNotification?.Invoke(msg);
    }
}
