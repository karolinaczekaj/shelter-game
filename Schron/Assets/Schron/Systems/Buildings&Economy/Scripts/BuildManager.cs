using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Schron.Core;

namespace Schron.Building
{
    public class BuildManager : MonoBehaviour
    {
        public static BuildManager Instance { get; private set; }

        [Header("Rooms")]
        [SerializeField] private List<RoomDataSO> buildableRooms = new List<RoomDataSO>();

        // NOWE: pokoje stojace od poczatku gry
        [System.Serializable]
        public struct StartingRoom
        {
            public RoomDataSO data;
            public Vector2Int cell;
        }

        [Header("Start")]
        [SerializeField] private List<StartingRoom> startingRooms = new List<StartingRoom>();

        private RoomDataSO selectedRoom;
        private Sprite placeholderSprite;

        private void Awake()
        {
            Instance = this;

            // tymczasowy bialy kwadrat zamiast grafik
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            placeholderSprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1);
        }

        // NOWE: stawia pokoje startowe za darmo
        private void Start()
        {
            foreach (var r in startingRooms)
                PlaceRoom(r.data, r.cell);
        }

        private void OnEnable()  => GameEvents.OnNotification += LogNotification;
        private void OnDisable() => GameEvents.OnNotification -= LogNotification;

        private void LogNotification(string msg) => Debug.Log("[Info] " + msg);
        // to bedzie wolac menu budowania Norberta
        public void SelectRoom(RoomDataSO data)
        {
            selectedRoom = data;
            Debug.Log(data != null ? "Wybrano: " + data.roomName : "Anulowano budowanie");
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            var mouse = Mouse.current;
            if (keyboard == null || mouse == null)
                return;

            // na razie wybor pokoju klawiszami 1-6, potem zrobi to UI
            for (int i = 0; i < buildableRooms.Count && i < 9; i++)
            {
                if (keyboard[Key.Digit1 + i].wasPressedThisFrame)
                    SelectRoom(buildableRooms[i]);
            }

            if (keyboard.escapeKey.wasPressedThisFrame || mouse.rightButton.wasPressedThisFrame)
                SelectRoom(null);

            if (selectedRoom != null && mouse.leftButton.wasPressedThisFrame)
            {
                Vector3 world = Camera.main.ScreenToWorldPoint(mouse.position.ReadValue());
                Vector2Int cell = ShelterGrid.Instance.WorldToCell(world);
                TryBuild(selectedRoom, cell);
            }
        }

        public Room TryBuild(RoomDataSO data, Vector2Int cell)
        {
            if (!ShelterGrid.Instance.CanPlace(cell, data.size))
            {
                GameEvents.Notify("Tu nie mozna budowac");
                return null;
            }

            if (!ResourceManager.Instance.TrySpend(data.buildCost))
            {
                GameEvents.Notify("Brakuje zasobow na " + data.roomName);
                return null;
            }

            return PlaceRoom(data, cell);
        }

        // stawia pokoj bez placenia, np. pokoje startowe
        public Room PlaceRoom(RoomDataSO data, Vector2Int cell)
        {
            ShelterGrid grid = ShelterGrid.Instance;

            GameObject go = new GameObject($"Room_{data.roomId}_{cell.x}_{cell.y}");
            go.transform.SetParent(grid.transform);
            go.transform.position = grid.AreaCenter(cell, data.size);
            go.transform.localScale = new Vector3(
                data.size.x * grid.CellSize.x * 0.95f,
                data.size.y * grid.CellSize.y * 0.9f,
                1);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = data.roomIcon != null ? data.roomIcon : placeholderSprite;
            sr.color = GetColor(data.roomType);

            Room room = go.AddComponent<Room>();
            room.Data = data;
            room.GridPosition = cell;

            grid.Occupy(cell, data.size, room);
            ShelterActions.RoomBuilt(data, cell);
            return room;
        }

        private Color GetColor(RoomType type)
        {
            switch (type)
            {
                case RoomType.Quarters:   return new Color(0.6f, 0.5f, 0.8f);
                case RoomType.Farm:       return new Color(0.4f, 0.8f, 0.3f);
                case RoomType.WaterPlant: return new Color(0.3f, 0.6f, 0.95f);
                case RoomType.Generator:  return new Color(0.95f, 0.8f, 0.2f);
                case RoomType.Mine:       return new Color(0.55f, 0.4f, 0.3f);
                case RoomType.Workshop:   return new Color(0.9f, 0.5f, 0.2f);
                default:                  return Color.white;
            }
        }
    }
}