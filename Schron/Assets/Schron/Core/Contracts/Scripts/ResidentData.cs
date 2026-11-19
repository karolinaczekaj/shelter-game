using UnityEngine;

namespace Schron.Core
{
    [System.Serializable]
    public class ResidentData
    {
        [Header("Basic Info")]
        public string id;
        public string residentName;
        public Sprite residentIcon;
        public GameObject residentPrefab;

        [Header("Condition")]
        [Range(0, 100)] public int health = 100;
        [Range(0, 100)] public int morale = 50;

        [Header("Stats")]
        [Range(1, 10)] public int strength = 1;
        [Range(1, 10)] public int intelligence = 1;
        [Range(1, 10)] public int agility = 1;

        [Header("State & Position")]
        public ResidentState currentState = ResidentState.Idle;
        public Vector2Int assignedRoomGridPos = new Vector2Int(-1, -1); // (-1,-1) = bez przydziału

        public int GetStat(StatType stat)
        {
            switch (stat)
            {
                case StatType.Strength: return strength;
                case StatType.Intelligence: return intelligence;
                case StatType.Agility: return agility;
                default: return 0;
            }
        }
    }
}
