using System.Collections.Generic;
using UnityEngine;

namespace Schron.Core
{
    // Para "zasób + ilość", np. 10 komponentów. Używana do kosztów i przeróbki.
    [System.Serializable]
    public struct ResourceAmount
    {
        public ResourceType type;
        public int amount;
    }

    [CreateAssetMenu(fileName = "NewRoomData", menuName = "Schron/Room Data")]
    public class RoomDataSO : ScriptableObject
    {
        [Header("Basic Info")]
        public string roomId;
        public string roomName;
        public RoomType roomType;
        public Sprite roomIcon;
        public GameObject roomPrefab;

        [Header("Grid & Build Parameters")]
        public Vector2Int size = new Vector2Int(1, 1);
        public List<ResourceAmount> buildCost = new List<ResourceAmount>();
        public List<ResourceAmount> upgradeCost = new List<ResourceAmount>();
        public int maxLevel = 3;

        [Header("Production & Workers")]
        public bool producesResources;
        public ResourceType producedResource;
        public float baseProductionPerWorker = 1f;
        public StatType requiredStat;          // statystyka, która zwiększa produkcję
        public int maxWorkers = 3;
        public float energyUpkeepPerDay = 1f;

        [Header("Conversion (np. warsztat)")]
        public List<ResourceAmount> inputPerDay = new List<ResourceAmount>(); // co pokój zużywa, żeby produkować
    }
}
