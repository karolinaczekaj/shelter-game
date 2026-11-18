using System;
using UnityEngine;

namespace Schron.Core
{
    // Zdarzenia symulacji schronu: dni, zasoby, pokoje, mieszkańcy.
    public static class ShelterActions
    {
        // Day Cycle
        public static event Action<int> OnDayEnded;
        public static event Action<int> OnDayStarted;

        // Resources
        public static event Action<ResourceType, float> OnResourceChanged;

        // Rooms
        public static event Action<RoomDataSO, Vector2Int> OnRoomBuilt;
        public static event Action<Vector2Int, int> OnRoomUpgraded;

        // Residents
        public static event Action<ResidentData, Vector2Int> OnResidentAssigned;
        public static event Action<ResidentData, ResidentState> OnResidentStateChanged;

        // Methods
        public static void DayEnded(int day) => OnDayEnded?.Invoke(day);
        public static void DayStarted(int day) => OnDayStarted?.Invoke(day);
        public static void ResourceChanged(ResourceType t, float v) => OnResourceChanged?.Invoke(t, v);
        public static void RoomBuilt(RoomDataSO data, Vector2Int pos) => OnRoomBuilt?.Invoke(data, pos);
        public static void RoomUpgraded(Vector2Int pos, int level) => OnRoomUpgraded?.Invoke(pos, level);
        public static void ResidentAssigned(ResidentData r, Vector2Int pos) => OnResidentAssigned?.Invoke(r, pos);
        public static void ResidentStateChanged(ResidentData r, ResidentState s) => OnResidentStateChanged?.Invoke(r, s);
    }
}
