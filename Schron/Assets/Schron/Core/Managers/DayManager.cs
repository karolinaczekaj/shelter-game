using UnityEngine;

namespace Schron.Core
{
    public class DayManager : MonoBehaviour
    {
        public static DayManager Instance { get; private set; }

        [Header("Days")]
        [SerializeField] private int preparationDays = 60;
        [SerializeField] private int disasterDays = 5;

        public int CurrentDay { get; private set; } = 1;
        public bool IsDisasterPhase => CurrentDay > preparationDays;
        public int LastDay => preparationDays + disasterDays;

        private bool gameOver = false;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            // zeby UI od razu pokazalo dzien 1
            ShelterActions.DayStarted(CurrentDay);
        }

        // pod przycisk "Nastepny dzien"
        [ContextMenu("Next Day")]
        public void NextDay()
        {
            if (gameOver)
                return;

            ShelterActions.DayEnded(CurrentDay);

            // przetrwalismy ostatni dzien katastrofy
            if (CurrentDay >= LastDay)
            {
                EndGame(true);
                return;
            }

            CurrentDay++;
            Debug.Log($"Dzien {CurrentDay}");

            if (CurrentDay == preparationDays + 1)
                GameEvents.DisasterStarted();

            ShelterActions.DayStarted(CurrentDay);
        }

        // moze to wywolac kazdy, np. jak wszyscy zgina
        public void EndGame(bool won)
        {
            if (gameOver)
                return;

            gameOver = true;
            Debug.Log(won ? "Wygrana" : "Przegrana");
            GameEvents.GameOver(won);
        }
    }
}