using UnityEngine;
using Schron.Core;

namespace Schron.Building
{
    public class ShelterGrid : MonoBehaviour
    {
        public static ShelterGrid Instance { get; private set; }

        [Header("Grid")]
        [SerializeField] private int width = 8;
        [SerializeField] private int height = 6;
        [SerializeField] private Vector2 cellSize = new Vector2(3f, 2f);
        [SerializeField] private int elevatorColumn = 0;

        public int Width => width;
        public int Height => height;
        public Vector2 CellSize => cellSize;
        public int ElevatorColumn => elevatorColumn;

        private Room[,] cells;

        private void Awake()
        {
            Instance = this;
            cells = new Room[width, height];
        }

        public bool IsInside(Vector2Int c)
        {
            return c.x >= 0 && c.x < width && c.y >= 0 && c.y < height;
        }

        // wolne = w siatce, nie winda i nic tam nie stoi
        public bool IsFree(Vector2Int c)
        {
            return IsInside(c) && c.x != elevatorColumn && cells[c.x, c.y] == null;
        }

        // czy pokoj o danym rozmiarze sie zmiesci
        public bool CanPlace(Vector2Int start, Vector2Int size)
        {
            for (int x = 0; x < size.x; x++)
                for (int y = 0; y < size.y; y++)
                    if (!IsFree(start + new Vector2Int(x, y)))
                        return false;
            return true;
        }

        public void Occupy(Vector2Int start, Vector2Int size, Room room)
        {
            for (int x = 0; x < size.x; x++)
                for (int y = 0; y < size.y; y++)
                    cells[start.x + x, start.y + y] = room;
        }

        public void Free(Vector2Int start, Vector2Int size)
        {
            Occupy(start, size, null);
        }

        public Room GetRoom(Vector2Int c)
        {
            return IsInside(c) ? cells[c.x, c.y] : null;
        }

        // y = 0 to najwyzsze pietro, im wieksze y tym glebiej
        public Vector3 AreaCenter(Vector2Int start, Vector2Int size)
        {
            return transform.position + new Vector3(
                (start.x + size.x / 2f) * cellSize.x,
                -(start.y + size.y / 2f) * cellSize.y,
                0);
        }

        public Vector2Int WorldToCell(Vector3 world)
        {
            Vector3 local = world - transform.position;
            return new Vector2Int(
                Mathf.FloorToInt(local.x / cellSize.x),
                Mathf.FloorToInt(-local.y / cellSize.y));
        }

        // podglad siatki w oknie Scene
        private void OnDrawGizmos()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Gizmos.color = x == elevatorColumn ? Color.yellow : Color.gray;
                    Vector3 center = AreaCenter(new Vector2Int(x, y), Vector2Int.one);
                    Gizmos.DrawWireCube(center, new Vector3(cellSize.x, cellSize.y, 0));
                }
            }
        }
    }
}