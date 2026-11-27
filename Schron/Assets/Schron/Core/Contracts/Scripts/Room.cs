using System.Collections.Generic;
using UnityEngine;

namespace Schron.Core
{
    public class Room : MonoBehaviour
    {
        public RoomDataSO Data;
        public Vector2Int GridPosition;
        public int Level = 1;

        // w kwaterach i lecznicy to nie pracownicy, tylko mieszkancy w srodku,
        // ale nazwa zostaje, zeby nie psuc kodu
        public List<ResidentData> Workers = new List<ResidentData>();

        // kwatery rosna z poziomem (4, 6, 8), reszta pokoi ma stala liczbe miejsc
        public int Capacity => Data.roomType == RoomType.Quarters
            ? Data.maxWorkers + (Level - 1) * (Data.maxWorkers / 2)
            : Data.maxWorkers;

        public bool IsFull => Data != null && Workers.Count >= Capacity;

        // wszystkie pokoje w schronie, zeby dalo sie znalezc pokoj po pozycji
        public static List<Room> AllRooms = new List<Room>();

        private void OnEnable()  => AllRooms.Add(this);
        private void OnDisable() => AllRooms.Remove(this);

        public static Room GetRoomAt(Vector2Int pos)
        {
            return AllRooms.Find(r => r.GridPosition == pos);
        }

        public bool TryAssign(ResidentData resident)
        {
            if (resident == null)
                return false;

            if (Workers.Contains(resident))
                return true;

            if (IsFull)
            {
                GameEvents.Notify("Pokoj jest pelny");
                return false;
            }

            // jak jest gdzies indziej, to najpierw go stamtad wypisujemy
            Room oldRoom = GetRoomAt(resident.assignedRoomGridPos);
            if (oldRoom != null)
                oldRoom.Remove(resident);

            ResidentState newState = GetStateForRoom();

            Workers.Add(resident);
            resident.assignedRoomGridPos = GridPosition;
            resident.currentState = newState;

            ShelterActions.ResidentAssigned(resident, GridPosition);
            ShelterActions.ResidentStateChanged(resident, newState);
            return true;
        }

        public void Remove(ResidentData resident)
        {
            if (!Workers.Remove(resident))
                return;

            resident.assignedRoomGridPos = new Vector2Int(-1, -1);
            resident.currentState = ResidentState.Idle;

            ShelterActions.ResidentStateChanged(resident, ResidentState.Idle);
        }

        public bool TryUpgrade()
        {
            if (Level >= Data.maxLevel)
            {
                GameEvents.Notify("Maksymalny poziom");
                return false;
            }

            if (!ResourceManager.Instance.TrySpend(Data.upgradeCost))
            {
                GameEvents.Notify("Brakuje zasobow na ulepszenie");
                return false;
            }

            Level++;
            ShelterActions.RoomUpgraded(GridPosition, Level);
            return true;
        }

        // jaki stan dostaje mieszkaniec w tym pokoju
        private ResidentState GetStateForRoom()
        {
            switch (Data.roomType)
            {
                case RoomType.Quarters:  return ResidentState.Rest;
                case RoomType.Infirmary: return ResidentState.Recovery;
                default:                 return ResidentState.Working;
            }
        }
    }
}