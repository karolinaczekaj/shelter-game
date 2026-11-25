using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Schron.Core;
using Schron.Building;

namespace Schron.UI
{
    public class BuildMenuUI : MonoBehaviour
    {
        [Header("Dostępne pokoje w sklepie")]
        [SerializeField] private List<RoomDataSO> availableRooms = new List<RoomDataSO>();

        [Header("Referencje UI")]
        [SerializeField] private GameObject shopPanel;
        [SerializeField] private Transform cardsContainer;
        [SerializeField] private RoomCardUI cardPrefab;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private Button openShopButton;
        [SerializeField] private Button closeShopButton;
        [SerializeField] private GameObject nextDayButton;      // Przycisk Następny dzień do ukrycia

        [Header("Przewijanie kółkiem myszy")]
        [SerializeField] private float scrollWheelSpeed = 0.15f;

        private readonly List<RoomCardUI> spawnedCards = new List<RoomCardUI>();

        private void Awake()
        {
            if (openShopButton != null)
                openShopButton.onClick.AddListener(OpenShop);

            if (closeShopButton != null)
                closeShopButton.onClick.AddListener(CloseShop);
        }

        private void Start()
        {
            GenerateCards();
            CloseShop(); // Na starcie sklep jest zamknięty
        }

        private void Update()
        {
            if (shopPanel == null || !shopPanel.activeSelf) return;

            // Zamykanie sklepu klawiszem Escape
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                CloseShop();
                return;
            }

            // Przewijanie poziome kółkiem myszy
            if (scrollRect != null && Mouse.current != null)
            {
                float scrollY = Mouse.current.scroll.ReadValue().y;
                if (Mathf.Abs(scrollY) > 0.01f)
                {
                    float direction = Mathf.Sign(scrollY);
                    float nextPos = scrollRect.horizontalNormalizedPosition - direction * scrollWheelSpeed;
                    scrollRect.horizontalNormalizedPosition = Mathf.Clamp01(nextPos);
                }
            }
        }

        private void GenerateCards()
        {
            if (cardsContainer == null || cardPrefab == null) return;

            foreach (Transform child in cardsContainer)
                Destroy(child.gameObject);

            spawnedCards.Clear();

            foreach (var room in availableRooms)
            {
                if (room == null) continue;

                RoomCardUI card = Instantiate(cardPrefab, cardsContainer);
                card.Setup(room, HandleRoomChosen);
                spawnedCards.Add(card);
            }
        }

        public void OpenShop()
        {
            if (shopPanel == null) return;

            if (BuildManager.Instance != null)
                BuildManager.Instance.SelectRoom(null);

            shopPanel.SetActive(true);

            // Ukrywa przyciski HUD-u podczas przeglądania sklepu
            if (nextDayButton != null)
                nextDayButton.SetActive(false);

            if (openShopButton != null)
                openShopButton.gameObject.SetActive(false);

            if (scrollRect != null)
                scrollRect.horizontalNormalizedPosition = 0f;

            foreach (var card in spawnedCards)
                card.RefreshAffordability();
        }

        public void CloseShop()
        {
            if (shopPanel != null)
                shopPanel.SetActive(false);

            // Przywraca przyciski HUD-u po zamknięciu sklepu
            if (nextDayButton != null)
                nextDayButton.SetActive(true);

            if (openShopButton != null)
                openShopButton.gameObject.SetActive(true);
        }

        private void HandleRoomChosen(RoomDataSO room)
        {
            CloseShop();

            if (BuildManager.Instance != null)
                BuildManager.Instance.SelectRoom(room);
        }
    }
}