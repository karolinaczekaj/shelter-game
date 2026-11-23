#if UNITY_EDITOR || DEVELOPMENT_BUILD
using Schron.Residents;
using System.Collections.Generic;
using UnityEngine;

namespace Schron.Core
{
    public class ResidentDebugTools : MonoBehaviour
    {
        [SerializeField] private string residentId;
        [SerializeField] private Vector2Int roomGridPos;

        private static readonly Vector2Int NoRoom = new Vector2Int(-1, -1);

        private void OnEnable()  => GameEvents.OnNotification += LogNotification;
        private void OnDisable() => GameEvents.OnNotification -= LogNotification;

        private void LogNotification(string msg) => Debug.Log($"[Notify] {msg}");

        private static List<ResidentData> GetAllResidents()
        {
            var list = new List<ResidentData>();
            if (ResidentManager.Instance == null)
                return list;

            foreach (ResidentController c in ResidentManager.Instance.Controllers)
                list.Add(c.Data);
            return list;
        }

        private static ResidentData FindResident(string id)
        {
            foreach (ResidentData r in GetAllResidents())
                if (r.id == id)
                    return r;
            return null;
        }

        [ContextMenu("Dump state")]
        private void Dump()
        {
            foreach (ResidentData r in GetAllResidents())
                Debug.Log($"{r.id} | {r.residentName} | stan: {r.currentState} | pokój: {r.assignedRoomGridPos}");

            foreach (Room room in Room.AllRooms)
            {
                string names = "";
                foreach (ResidentData w in room.Workers)
                    names += w.residentName + " ";

                Debug.Log($"Pokój {room.GridPosition}: pracowników {room.Workers.Count} -> {names}");
            }
        }

        [ContextMenu("Assign to room")]
        private void AssignToRoom()
        {
            ResidentData data = FindResident(residentId);
            Room room = Room.GetRoomAt(roomGridPos);

            if (data == null || room == null)
            {
                Debug.LogWarning("Nie znaleziono postaci o tym id albo pokoju o tej pozycji.");
                return;
            }

            Debug.Log($"TryAssign -> {room.TryAssign(data)}");
        }

        [ContextMenu("Remove from room")]
        private void RemoveFromRoom()
        {
            ResidentData data = FindResident(residentId);
            if (data == null)
            {
                Debug.LogWarning("Nie znaleziono postaci o tym id.");
                return;
            }

            Room room = Room.GetRoomAt(data.assignedRoomGridPos);
            if (room == null)
            {
                Debug.Log("Ta postać nie ma pokoju.");
                return;
            }

            room.Remove(data);
        }

        // Sprawdza zgodność dyrektywy: lista pracowników w pokojach <-> pozycja zapisana w mieszkańcu.
        [ContextMenu("Check consistency")]
        private void CheckConsistency()
        {
            bool ok = true;

            foreach (ResidentData r in GetAllResidents())
            {
                if (r.assignedRoomGridPos == NoRoom)
                {
                    foreach (Room room in Room.AllRooms)
                    {
                        if (room.Workers.Contains(r))
                        {
                            Debug.LogError($"'{r.residentName}' nie ma pokoju, a jest na liście pokoju {room.GridPosition}.");
                            ok = false;
                        }
                    }
                }
                else
                {
                    Room room = Room.GetRoomAt(r.assignedRoomGridPos);
                    if (room == null || !room.Workers.Contains(r))
                    {
                        Debug.LogError($"'{r.residentName}' ma pokój {r.assignedRoomGridPos}, ale nie ma go na liście pracowników.");
                        ok = false;
                    }
                }
            }

            foreach (Room room in Room.AllRooms)
            {
                foreach (ResidentData w in room.Workers)
                {
                    if (w.assignedRoomGridPos != room.GridPosition)
                    {
                        Debug.LogError($"'{w.residentName}' jest na liście pokoju {room.GridPosition}, ale ma zapisany pokój {w.assignedRoomGridPos}.");
                        ok = false;
                    }
                }
            }

            if (ok)
                Debug.Log("Spójność OK.");
        }
    }
}
#endif