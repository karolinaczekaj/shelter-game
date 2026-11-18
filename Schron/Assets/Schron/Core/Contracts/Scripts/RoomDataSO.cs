using UnityEngine;

namespace Schron.Core
{
    [CreateAssetMenu(fileName = "NewRoomData", menuName = "Schron/Room Data")]
    public class RoomDataSO : ScriptableObject
    {
        [Header("Basic Info")]
        public string roomId;
        public string roomName;
        public Sprite roomIcon;
        public GameObject roomPrefab;

        [Header("Grid & Build Parameters")]
        public Vector2Int size = new Vector2Int(1, 1);
        public int buildCost = 50;
        public int upgradeCost = 100;
        public int maxLevel = 3;

        [Header("Production & Workers")]
        public bool producesResources;
        public ResourceType producedResource;
        public float baseProductionPerWorker = 1f;
        public int maxWorkers = 5;
        public float energyUpkeepPerDay = 1f;
    }
}
