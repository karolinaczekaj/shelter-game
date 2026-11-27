using UnityEngine;
using Schron.Core;
using Schron.Building;

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

    [ContextMenu("Pokaz pojemnosc schronu")]
    private void ShowCapacity()
    {
        Debug.Log("Miejsca w kwaterach: " + ShelterCapacity.GetTotalBeds());
    }

    [ContextMenu("Wpisz testowego do kwater")]
    private void AssignToQuarters()
    {
        var quarters = Room.AllRooms.Find(r => r.Data.roomType == RoomType.Quarters);
        if (quarters == null) return;

        var resident = new ResidentData { id = "sleeper", residentName = "Spioch" };
        bool ok = quarters.TryAssign(resident);
        Debug.Log($"Kwatery: przypisanie {(ok ? "udane" : "nieudane")}, stan {resident.currentState}, osob {quarters.Workers.Count}/{quarters.Capacity}");
    }

    private ResidentData testPatient;

    [ContextMenu("Wyslij rannego do lecznicy")]
    private void SendPatient()
    {
        var infirmary = Room.AllRooms.Find(r => r.Data.roomType == RoomType.Infirmary);
        if (infirmary == null) { Debug.Log("Najpierw zbuduj lecznice (7)"); return; }

        testPatient = new ResidentData { id = "patient", residentName = "Ranny Janek", health = 30 };
        bool ok = infirmary.TryAssign(testPatient);
        Debug.Log($"Lecznica: przyjecie {(ok ? "udane" : "nieudane")}, stan {testPatient.currentState}, zdrowie {testPatient.health}");
    }

    [ContextMenu("Pokaz pacjenta")]
    private void ShowPatient()
    {
        if (testPatient == null) return;
        Debug.Log($"{testPatient.residentName}: zdrowie {testPatient.health}, stan {testPatient.currentState}");
    }
}