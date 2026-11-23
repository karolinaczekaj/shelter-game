using System.Collections.Generic;
using UnityEngine;
using Schron.Core;

namespace Schron.Residents
{
    // NOTE: never write assignedRoomGridPos or currentState here.
    public class ResidentManager : MonoBehaviour
    {
        public static ResidentManager Instance { get; private set; }

        [Header("Residents without a room (Idle)")]
        [SerializeField] private Transform residentsParent;
        [SerializeField] private Vector3 idleAreaStart = Vector3.zero;
        [SerializeField] private float idleSpacing = 2f;

        private static readonly Vector2Int NoRoom = new Vector2Int(-1, -1);

        // Source of truth: resident data, independent from scene objects.
        private readonly List<ResidentData> _residents = new List<ResidentData>();
        private readonly Dictionary<string, ResidentController> _controllers =
            new Dictionary<string, ResidentController>();

        public IReadOnlyList<ResidentData> AllResidents => _residents;
        public IEnumerable<ResidentController> Controllers => _controllers.Values;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void OnEnable()
        {
            if (Instance != this)
                return;

            ShelterActions.OnResidentAssigned += HandleResidentAssigned;
        }

        private void OnDisable()
        {
            ShelterActions.OnResidentAssigned -= HandleResidentAssigned;
        }

        // Raised by Room.TryAssign -> ShelterActions.ResidentAssigned
        private void HandleResidentAssigned(ResidentData data, Vector2Int gridPos)
        {
            ResidentController controller = GetController(data);
            Room room = Room.GetRoomAt(gridPos);

            if (controller != null && room != null)
                controller.MoveToRoom(room);
        }

        // Spawns the starting residents. Call once at the start of a NEW game,
        // after the rooms exist. Returns the number of spawned residents.
        public int SpawnStartingResidents(StartingRosterSO roster)
        {
            if (roster == null)
            {
                Debug.LogError("SpawnStartingResidents: roster is missing.");
                return 0;
            }

            int spawned = 0;

            foreach (StartingResident entry in roster.residents)
            {
                if (entry == null || entry.data == null)
                {
                    Debug.LogWarning("Empty entry in StartingRoster - skipping.");
                    continue;
                }

                // A resident must start unassigned. The room is set only by Room.TryAssign().
                if (entry.data.assignedRoomGridPos != NoRoom)
                {
                    Debug.LogError(
                        $"'{entry.data.residentName}': template has assignedRoomGridPos " +
                        $"{entry.data.assignedRoomGridPos}. Set it to (-1,-1) and choose the room " +
                        "in 'Room Grid Pos'. Skipping this resident.");
                    continue;
                }

                ResidentData data = entry.data.Clone();

                ResidentController controller = Spawn(data, GetNextIdlePosition(), residentsParent);
                if (controller == null)
                    continue;

                spawned++;

                if (!entry.assignToRoom)
                    continue;

                Room room = Room.GetRoomAt(entry.roomGridPos);
                if (room == null)
                    Debug.LogWarning($"Room {entry.roomGridPos} not found for '{data.residentName}'.");
                else
                    room.TryAssign(data); // on failure the resident stays Idle, state stays consistent
            }

            return spawned;
        }

        public ResidentController Spawn(ResidentData data, Vector3 position, Transform parent = null)
        {
            if (data == null)
            {
                Debug.LogError("Spawn: data is null.");
                return null;
            }

            if (data.residentPrefab == null)
            {
                Debug.LogError($"Resident '{data.residentName}' has no prefab assigned!");
                return null;
            }

            if (string.IsNullOrEmpty(data.id))
                data.id = System.Guid.NewGuid().ToString("N");

            if (_controllers.ContainsKey(data.id))
            {
                Debug.LogError($"Resident with id '{data.id}' already exists!");
                return null;
            }

            GameObject go = Instantiate(data.residentPrefab, position, Quaternion.identity, parent);

            ResidentController controller = go.GetComponent<ResidentController>();
            if (controller == null)
                controller = go.AddComponent<ResidentController>();

            controller.Initialize(data);
            _residents.Add(data);
            _controllers.Add(data.id, controller);
            return controller;
        }

        public ResidentData GetResident(string id)
        {
            return _residents.Find(r => r.id == id);
        }

        public ResidentController GetController(ResidentData data)
        {
            if (data == null || string.IsNullOrEmpty(data.id))
                return null;

            _controllers.TryGetValue(data.id, out ResidentController controller);
            return controller;
        }

        // Next free slot in the row of residents without a room (reads data only).
        private Vector3 GetNextIdlePosition()
        {
            int unassigned = 0;
            foreach (ResidentData r in _residents)
                if (r.assignedRoomGridPos == NoRoom)
                    unassigned++;

            return idleAreaStart + new Vector3(unassigned * idleSpacing, 0f, 0f);
        }
    }
}