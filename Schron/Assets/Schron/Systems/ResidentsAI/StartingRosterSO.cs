using System.Collections.Generic;
using UnityEngine;
using Schron.Core;

namespace Schron.Residents
{
    [System.Serializable]
    public class StartingResident
    {
        [Tooltip("Keep assignedRoomGridPos at (-1,-1). Choose the room below.")]
        public ResidentData data;

        [Tooltip("Tick to assign this resident to a room right after spawning.")]
        public bool assignToRoom;

        [Tooltip("GridPosition of the target room.")]
        public Vector2Int roomGridPos;
    }

    [CreateAssetMenu(fileName = "StartingRoster", menuName = "Schron/Starting Roster")]
    public class StartingRosterSO : ScriptableObject
    {
        public List<StartingResident> residents = new List<StartingResident>();
    }
}