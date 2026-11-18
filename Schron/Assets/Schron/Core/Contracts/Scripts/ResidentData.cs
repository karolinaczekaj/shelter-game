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

        [Header("Stats")]
        [Range(0, 100)] public int health = 100;
        [Range(0, 100)] public int morale = 50;
        [Range(0f, 1f)] public float skill = 0.5f;

        [Header("State & Position")]
        public ResidentState currentState = ResidentState.Idle;
        public Vector2Int assignedRoomGridPos = new Vector2Int(-1, -1); // (-1,-1) = bez przydziału
    }
}
