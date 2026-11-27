using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Schron.Core;

namespace Schron.UI
{
    public class ShelterHUD : MonoBehaviour
    {
        [Header("Dzień i Tura")]
        [SerializeField] private TextMeshProUGUI dayText;
        [SerializeField] private Button nextDayButton;
        [SerializeField] private int victoryDay = 66; // Po tym dniu wygrana (na razie 66)

        [Header("Zasoby Przetrwania")]
        [SerializeField] private TextMeshProUGUI foodText;
        [SerializeField] private TextMeshProUGUI waterText;
        [SerializeField] private TextMeshProUGUI energyText;
        [SerializeField] private TextMeshProUGUI moraleText;

        [Header("Materiały Budowlane")]
        [SerializeField] private TextMeshProUGUI scrapText;
        [SerializeField] private TextMeshProUGUI oreText;
        [SerializeField] private TextMeshProUGUI componentsText;

        [Header("Ostrzeżenia niskich zasobów")]
        [SerializeField] private float lowResourceThreshold = 20f; // nwm ile zasobów to mało ale na razie uznaje że 20
        [SerializeField] private Color normalTextColor = Color.white;
        [SerializeField] private Color warningTextColor = Color.red;

        [Header("Ekran Końca Gry (Game Over / Wygrana)")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TextMeshProUGUI gameOverTitleText;
        [SerializeField] private TextMeshProUGUI gameOverStatsText;
        [SerializeField] private Button restartButton;

        private int currentDay = 1; // aktualny dzień w grze
        private int successfulExpeditions = 0; // tylko do wyświetlenia czegoś w statystykach na końcu gry
        private bool isGameEnded = false;

        // OnEnable nasłuje zmian w zasobach i dniu, oraz przyciskach nextDay i restart
        // OnDisable usuwa nasłuchiwanie
        private void OnEnable()
        {
            ShelterActions.OnDayStarted += UpdateDayUI;
            ShelterActions.OnResourceChanged += UpdateResourceUI;

            GameEvents.OnGameOver += ShowGameOverScreen;
            GameEvents.OnExpeditionReturned += HandleExpeditionReturned;

            if (nextDayButton != null)
                nextDayButton.onClick.AddListener(OnNextDayClicked);

            if (restartButton != null)
                restartButton.onClick.AddListener(RestartScene);
        }

        private void OnDisable()
        {
            ShelterActions.OnDayStarted -= UpdateDayUI;
            ShelterActions.OnResourceChanged -= UpdateResourceUI;

            GameEvents.OnGameOver -= ShowGameOverScreen;
            GameEvents.OnExpeditionReturned -= HandleExpeditionReturned;

            if (nextDayButton != null)
                nextDayButton.onClick.RemoveListener(OnNextDayClicked);

            if (restartButton != null)
                restartButton.onClick.RemoveListener(RestartScene);
        }

        
        private void Start()
        {
            if (gameOverPanel != null)
                gameOverPanel.SetActive(false);

            UpdateDayUI(currentDay);

            // Odswieza pasek na starcie na podstawie ResourceManagera (jesli jest na scenie)
            if (ResourceManager.Instance != null)
            {
                UpdateResourceUI(ResourceType.Food, ResourceManager.Instance.Get(ResourceType.Food));
                UpdateResourceUI(ResourceType.Water, ResourceManager.Instance.Get(ResourceType.Water));
                UpdateResourceUI(ResourceType.Energy, ResourceManager.Instance.Get(ResourceType.Energy));
                UpdateResourceUI(ResourceType.Morale, ResourceManager.Instance.Get(ResourceType.Morale));
                UpdateResourceUI(ResourceType.Scrap, ResourceManager.Instance.Get(ResourceType.Scrap));
                UpdateResourceUI(ResourceType.Ore, ResourceManager.Instance.Get(ResourceType.Ore));
                UpdateResourceUI(ResourceType.Components, ResourceManager.Instance.Get(ResourceType.Components));
            }
        }

        private void OnNextDayClicked()
        {
            if (isGameEnded) return;

            ShelterActions.DayEnded(currentDay);
            currentDay++;

            if (currentDay > victoryDay)
            {
                GameEvents.GameOver(true);
                return;
            }

            ShelterActions.DayStarted(currentDay);
        }

        private void UpdateDayUI(int day)
        {
            currentDay = day;
            if (dayText != null)
                dayText.text = $"Dzień {currentDay}";
        }

        private void UpdateResourceUI(ResourceType type, float newValue)
        {
            int val = Mathf.RoundToInt(newValue);
            Color survivalColor = newValue < lowResourceThreshold ? warningTextColor : normalTextColor;

            switch (type)
            {
                case ResourceType.Food:
                    if (foodText != null)
                    {
                        foodText.text = $"Jedzenie: {val}";
                        foodText.color = survivalColor;
                    }
                    break;
                case ResourceType.Water:
                    if (waterText != null)
                    {
                        waterText.text = $"Woda: {val}";
                        waterText.color = survivalColor;
                    }
                    break;
                case ResourceType.Energy:
                    if (energyText != null)
                    {
                        energyText.text = $"Energia: {val}";
                        energyText.color = survivalColor;
                    }
                    break;
                case ResourceType.Morale:
                    if (moraleText != null)
                    {
                        moraleText.text = $"Morale: {val}%";
                        moraleText.color = survivalColor;
                    }
                    break;
                case ResourceType.Scrap:
                    if (scrapText != null)
                    {
                        scrapText.text = $"Złom: {val}";
                        scrapText.color = normalTextColor;
                    }
                    break;
                case ResourceType.Ore:
                    if (oreText != null)
                    {
                        oreText.text = $"Ruda: {val}";
                        oreText.color = normalTextColor;
                    }
                    break;
                case ResourceType.Components:
                    if (componentsText != null)
                    {
                        componentsText.text = $"Komponenty: {val}";
                        componentsText.color = normalTextColor;
                    }
                    break;
            }
        }

        // po nic na razie
        private void HandleExpeditionReturned(ResidentData resident, bool success)
        {
            if (success)
                successfulExpeditions++;
        }

        private void ShowGameOverScreen(bool won)
        {
            isGameEnded = true;

            if (nextDayButton != null)
                nextDayButton.interactable = false;

            if (gameOverPanel != null)
                gameOverPanel.SetActive(true);

            if (gameOverTitleText != null)
            {
                gameOverTitleText.text = won ? "PRZETRWAŁEŚ KATASTROFĘ!" : "SCHRON UPADŁ";
                gameOverTitleText.color = won ? Color.green : Color.red;
            }

            if (gameOverStatsText != null)
            {
                int survivedDays = won ? victoryDay : currentDay;
                gameOverStatsText.text =
                    $"Przetrwane dni: {survivedDays}\n" +
                    $"Udane ekspedycje: {successfulExpeditions}";
            }
        }

        private void RestartScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        // ==============================================
        // Poniżej są metody testowe do testowania.
        // ==============================================
        [ContextMenu("Test: Niskie zasoby (< 20)")]
        private void TestLowResources()
        {
            ShelterActions.ResourceChanged(ResourceType.Food, 15);
            ShelterActions.ResourceChanged(ResourceType.Water, 8);
            ShelterActions.ResourceChanged(ResourceType.Energy, 45);
            ShelterActions.ResourceChanged(ResourceType.Morale, 19);
        }

        [ContextMenu("Test: Symuluj Przegraną")]
        private void TestDefeat()
        {
            GameEvents.GameOver(false);
        }
    }
}