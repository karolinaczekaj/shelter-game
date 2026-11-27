using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Schron.Core;

// ==========================================================
// to jest wzór do wyświetlania kart pokoi w menu budowy
// ==========================================================

namespace Schron.UI
{
    public class RoomCardUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Image previewImage;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private Button selectButton;

        [Header("Colors")]
        [SerializeField] private Color affordableCostColor = Color.white;
        [SerializeField] private Color unaffordableCostColor = new Color(1f, 0.35f, 0.35f);

        private RoomDataSO roomData;
        private Action<RoomDataSO> onSelected;

        public void Setup(RoomDataSO data, Action<RoomDataSO> onSelectedCallback)
        {
            roomData = data;
            onSelected = onSelectedCallback;

            if (nameText != null)
                nameText.text = data.roomName;

            if (previewImage != null)
            {
                if (data.roomIcon != null)
                {
                    previewImage.sprite = data.roomIcon;
                    previewImage.color = Color.white;
                }
                else
                {
                    // Kolor identyczny z tym, jakim BuildManager rysuje pokoje na siatce
                    previewImage.sprite = null;
                    previewImage.color = GetRoomColor(data.roomType);
                }
            }

            if (costText != null)
                costText.text = FormatCost(data.buildCost);

            if (selectButton != null)
            {
                selectButton.onClick.RemoveAllListeners();
                selectButton.onClick.AddListener(() => onSelected?.Invoke(roomData));
            }

            RefreshAffordability();
        }

        public void RefreshAffordability()
        {
            if (roomData == null || ResourceManager.Instance == null) return;

            bool canAfford = ResourceManager.Instance.CanAfford(roomData.buildCost);

            if (costText != null)
                costText.color = canAfford ? affordableCostColor : unaffordableCostColor;

            if (selectButton != null)
                selectButton.interactable = canAfford;
        }

        private string FormatCost(List<ResourceAmount> cost)
        {
            if (cost == null || cost.Count == 0)
                return "Darmowy";

            List<string> parts = new List<string>();
            foreach (var c in cost)
                parts.Add($"{GetResourceLabel(c.type)}: {c.amount}");

            return string.Join("\n", parts);
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
                case ResourceType.Morale:     return "Morale";
                default:                      return type.ToString();
            }
        }

        private Color GetRoomColor(RoomType type)
        {
            switch (type)
            {
                case RoomType.Quarters:   return new Color(0.6f, 0.5f, 0.8f);
                case RoomType.Farm:       return new Color(0.4f, 0.8f, 0.3f);
                case RoomType.WaterPlant: return new Color(0.3f, 0.6f, 0.95f);
                case RoomType.Generator:  return new Color(0.95f, 0.8f, 0.2f);
                case RoomType.Mine:       return new Color(0.55f, 0.4f, 0.3f);
                case RoomType.Workshop:   return new Color(0.9f, 0.5f, 0.2f);
                case RoomType.Infirmary:  return new Color(0.9f, 0.35f, 0.45f);
                default:                  return Color.white;
            }
        }
    }
}