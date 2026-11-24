using UnityEngine;
using Schron.Core;

// tylko do testow, dopoki nie ma mieszkancow Bartka
public class TestWorkers : MonoBehaviour
{
    [SerializeField] private int workersPerRoom = 2;

    [ContextMenu("Dodaj testowych pracownikow")]
    private void AddWorkers()
    {
        int n = 1;

        foreach (var room in Room.AllRooms)
        {
            if (!room.Data.producesResources)
                continue;

            for (int i = 0; i < workersPerRoom; i++)
            {
                var resident = new ResidentData
                {
                    id = "test_" + n,
                    residentName = "Tester " + n,
                    strength = Random.Range(1, 11),
                    intelligence = Random.Range(1, 11),
                    agility = Random.Range(1, 11)
                };
                n++;
                room.TryAssign(resident);
            }
        }

        Debug.Log("Dodano testowych pracownikow");
    }

    [ContextMenu("Ulepsz pierwszy pokoj")]
    private void UpgradeFirstRoom()
    {
        var room = Room.AllRooms.Find(r => r.Data.producesResources);
        if (room == null) return;

        bool ok = room.TryUpgrade();
        Debug.Log($"{room.Data.roomName}: ulepszenie {(ok ? "udane" : "nieudane")}, poziom {room.Level}");
    }
}