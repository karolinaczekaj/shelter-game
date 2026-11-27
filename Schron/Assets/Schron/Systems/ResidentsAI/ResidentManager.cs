using System.Collections.Generic;
using UnityEngine;
using Schron.Core;
using Schron.Building;

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
            ShelterActions.OnResidentStateChanged += HandleResidentStateChanged;
        }

        private void OnDisable()
        {
            ShelterActions.OnResidentAssigned -= HandleResidentAssigned;
            ShelterActions.OnResidentStateChanged -= HandleResidentStateChanged;
        }

        // Raised by Room.TryAssign -> ShelterActions.ResidentAssigned
        private void HandleResidentAssigned(ResidentData data, Vector2Int gridPos)
        {
            ResidentController controller = GetController(data);
            Room room = Room.GetRoomAt(gridPos);

            if (controller != null && room != null)
                controller.MoveToRoom(room);
        }

        // After Room.Remove() the resident is Idle without a room: send them back to the idle row.
        private void HandleResidentStateChanged(ResidentData data, ResidentState state)
        {
            if (state != ResidentState.Idle || data.assignedRoomGridPos != NoRoom)
                return;

            ResidentController controller = GetController(data);
            if (controller != null)
                controller.MoveTo(GetNextIdlePosition(data));
        }


        // Validates a template, makes a copy and spawns it in the idle row (no room yet).
        private ResidentController SpawnFromTemplate(ResidentData template)
        {
            if (template == null)
            {
                Debug.LogWarning("Resident template is missing - skipping.");
                return null;
            }

            // A resident must start unassigned. The room is set only by Room.TryAssign().
            if (template.assignedRoomGridPos != NoRoom)
            {
                Debug.LogError(
                    $"'{template.residentName}': template has assignedRoomGridPos " +
                    $"{template.assignedRoomGridPos}. Set it to (-1,-1). Skipping this resident.");
                return null;
            }

            ResidentData data = template.Clone();
            return Spawn(data, GetNextIdlePosition(), residentsParent);
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
                if (entry == null)
                {
                    Debug.LogWarning("Empty entry in StartingRoster - skipping.");
                    continue;
                }

                ResidentController controller = SpawnFromTemplate(entry.data);
                if (controller == null)
                    continue;

                spawned++;

                if (!entry.assignToRoom)
                    continue;

                Room room = Room.GetRoomAt(entry.roomGridPos);
                if (room == null)
                    Debug.LogWarning($"Room {entry.roomGridPos} not found for '{controller.Data.residentName}'.");
                else
                    room.TryAssign(controller.Data); // state comes from the room type; on failure the resident stays Idle
            }

            // Balance check only: the starting roster is not blocked by beds.
            int beds = ShelterCapacity.GetTotalBeds();
            if (beds > 0 && _residents.Count > beds)
                Debug.LogWarning($"Starting roster has {_residents.Count} residents, but there are only {beds} beds.");

            return spawned;
        }

        public bool HasFreeBed()
        {
            return ShelterCapacity.HasFreeBed(_residents.Count);
        }

        // Admits a new resident (recruitment, story event...). Checks the beds first.
        public ResidentController TryAddResident(ResidentData template)
        {
            if (!HasFreeBed())
            {
                GameEvents.Notify("Brak wolnych lozek w kwaterach");
                return null;
            }

            return SpawnFromTemplate(template);
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
        private Vector3 GetNextIdlePosition(ResidentData exclude = null)
        {
            int unassigned = 0;
            foreach (ResidentData r in _residents)
                if (r != exclude && r.assignedRoomGridPos == NoRoom)
                    unassigned++;

            return idleAreaStart + new Vector3(unassigned * idleSpacing, 0f, 0f);
        }
    }
}