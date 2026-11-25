using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;
using Schron.Core;
using Schron.Building;

namespace Schron.UI
{
    public class RoomInfoUI : MonoBehaviour
    {
        [Header("Referencje UI")]
        [SerializeField] private GameObject infoPanel;
        [SerializeField] private TextMeshProUGUI roomTitleText;
        [SerializeField] private TextMeshProUGUI workersText;
        [SerializeField] private TextMeshProUGUI upgradeCostText;
        [SerializeField] private Button upgradeButton;
        [SerializeField] private Button closeButton;

        [Header("Kolory kosztu")]
        [SerializeField] private Color affordableCostColor = Color.white;
        [SerializeField] private Color unaffordableCostColor = new Color(1f, 0.35f, 0.35f);

        private Room selectedRoom;
        private bool isBuildModeActive;

        private void OnEnable()
        {
            ShelterActions.OnRoomUpgraded += HandleRoomUpgraded;
            ShelterActions.OnResourceChanged += HandleResourceChanged;
        }

        private void OnDisable()
        {
            ShelterActions.OnRoomUpgraded -= HandleRoomUpgraded;
            ShelterActions.OnResourceChanged -= HandleResourceChanged;
        }

        private void Awake()
        {
            if (upgradeButton != null)
                upgradeButton.onClick.AddListener(OnUpgradeClicked);

            if (closeButton != null)
                closeButton.onClick.AddListener(ClosePanel);
        }

        // na starcie gry panel jest zamknięty
        private void Start()
        {
            ClosePanel();
        }

        // update 
        private void Update()
        {
            var mouse = Mouse.current;
            var keyboard = Keyboard.current;
            if (mouse == null) return;

            if (keyboard != null)
            {
                for (int i = 0; i < 9; i++)
                    if (keyboard[Key.Digit1 + i].wasPressedThisFrame) isBuildModeActive = true;

                if (keyboard.escapeKey.wasPressedThisFrame)
                {
                    isBuildModeActive = false;
                    ClosePanel();
                }
            }

            // Prawy przycisk myszy anuluje tryb budowania i zamyka panel informacji
            if (mouse.rightButton.wasPressedThisFrame)
            {
                isBuildModeActive = false;
                ClosePanel();
            }

            if (mouse.leftButton.wasPressedThisFrame)
            {
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                    return;

                if (isBuildModeActive)
                {
                    isBuildModeActive = false;
                    return;
                }

                if (ShelterGrid.Instance != null && Camera.main != null)
                {
                    Vector3 world = Camera.main.ScreenToWorldPoint(mouse.position.ReadValue());
                    Vector2Int cell = ShelterGrid.Instance.WorldToCell(world);
                    Room clickedRoom = FindRoomCoveringCell(cell);

                    if (clickedRoom != null)
                        OpenForRoom(clickedRoom);
                    else
                        ClosePanel();
                }
            }
        }

        // zwraca pokój zajmujący daną komórkę
        private Room FindRoomCoveringCell(Vector2Int cell)
        {
            foreach (var room in Room.AllRooms)
            {
                if (room == null || room.Data == null) continue;

                bool inX = cell.x >= room.GridPosition.x && cell.x < room.GridPosition.x + room.Data.size.x;
                bool inY = cell.y >= room.GridPosition.y && cell.y < room.GridPosition.y + room.Data.size.y;

                if (inX && inY)
                    return room;
            }
            return null;
        }

        // otwiera panel i wypełnia go informacjami o danym pokoju
        public void OpenForRoom(Room room)
        {
            selectedRoom = room;
            if (infoPanel != null)
                infoPanel.SetActive(true);

            RefreshUI();
        }

        public void ClosePanel()
        {
            selectedRoom = null;
            if (infoPanel != null)
                infoPanel.SetActive(false);
        }

        // Wywołuje logikę ulepszenia pokoju po kliknięciu przycisku od ulepszania
        private void OnUpgradeClicked()
        {
            if (selectedRoom == null) return;

            if (selectedRoom.TryUpgrade())
                RefreshUI();
        }

        private void HandleRoomUpgraded(Vector2Int pos, int newLevel)
        {
            if (selectedRoom != null && selectedRoom.GridPosition == pos)
                RefreshUI();
        }

        private void HandleResourceChanged(ResourceType type, float newValue)
        {
            if (infoPanel != null && infoPanel.activeSelf && selectedRoom != null)
                RefreshUI();
        }

        private void RefreshUI()
        {
            if (selectedRoom == null || selectedRoom.Data == null) return;

            var data = selectedRoom.Data;

            if (roomTitleText != null)
                roomTitleText.text = $"{data.roomName} (Poz. {selectedRoom.Level}/{data.maxLevel})";

            if (workersText != null)
                workersText.text = $"Pracownicy: {selectedRoom.Workers.Count}/{data.maxWorkers}";

            bool isMax = selectedRoom.Level >= data.maxLevel;
            bool canAfford = !isMax && ResourceManager.Instance != null && ResourceManager.Instance.CanAfford(data.upgradeCost);

            if (upgradeCostText != null)
            {
                upgradeCostText.text = isMax ? "Maksymalny poziom" : $"Koszt ulepszenia: {FormatCost(data.upgradeCost)}";
                upgradeCostText.color = (isMax || canAfford) ? affordableCostColor : unaffordableCostColor;
            }

            if (upgradeButton != null)
                upgradeButton.interactable = canAfford;
        }

        private string FormatCost(List<ResourceAmount> cost)
        {
            if (cost == null || cost.Count == 0)
                return "Darmowe";

            List<string> parts = new List<string>();
            foreach (var c in cost)
                parts.Add($"{GetResourceLabel(c.type)}: {c.amount}");

            return string.Join(", ", parts);
        }

        private string GetResourceLabel(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.Food:       return "Jedzenie";
                case ResourceType.Water:      return "Woda";
                case ResourceType.Energy:     return "Energia";
                case ResourceType.Scrap:      return "Złom";
                case ResourceType.Ore:        return "Ruda";
                case ResourceType.Components: return "Komponenty";
                default:                      return type.ToString();
            }
        }
    }
}