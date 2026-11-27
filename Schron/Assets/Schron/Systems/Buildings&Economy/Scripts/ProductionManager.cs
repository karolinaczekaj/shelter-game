using UnityEngine;
using Schron.Core;
using System.Collections.Generic;

namespace Schron.Building
{
    public class ProductionManager : MonoBehaviour
    {
        [Header("Mine")]
        [SerializeField] private float mineScrapChance = 0.3f;
        [SerializeField] private int mineScrapPerWorker = 1;

        [Header("Infirmary")]
        [SerializeField] private int healPerDay = 20;

        [Header("Debug")]
        [SerializeField] private bool logSummary = true;

        private void OnEnable()  => ShelterActions.OnDayEnded += HandleDayEnded;
        private void OnDisable() => ShelterActions.OnDayEnded -= HandleDayEnded;

        private void HandleDayEnded(int day)
        {
            var rm = ResourceManager.Instance;

            // najpierw generatory, bo reszta potrzebuje pradu
            foreach (var room in Room.AllRooms)
            {
                if (room.Data.roomType == RoomType.Generator)
                    Produce(room);
            }

            // potem reszta: prad -> materialy -> produkcja
            foreach (var room in Room.AllRooms)
            {
                if (room.Data.roomType == RoomType.Generator)
                    continue;

                if (!rm.TrySpend(ResourceType.Energy, room.Data.energyUpkeepPerDay))
                {
                    GameEvents.Notify("Brak pradu: " + room.Data.roomName);
                    continue;
                }

                // warsztat musi miec z czego robic
                if (room.Data.inputPerDay.Count > 0 && room.Workers.Count > 0)
                {
                    if (!rm.TrySpend(room.Data.inputPerDay))
                    {
                        GameEvents.Notify(room.Data.roomName + ": brak materialow");
                        continue;
                    }
                }

                Produce(room);

                if (room.Data.roomType == RoomType.Mine)
                    MineScrapBonus(room);

                    
                if (room.Data.roomType == RoomType.Infirmary)
                    Heal(room);
            }

            if (logSummary)
                LogResources(day);
        }

        private void Produce(Room room)
        {
            if (!room.Data.producesResources || room.Workers.Count == 0)
                return;

            ResourceManager.Instance.Add(room.Data.producedResource, GetProduction(room));
        }

        // ile pokoj wyprodukuje dzisiaj
        public float GetProduction(Room room)
        {
            float total = 0;

            foreach (var worker in room.Workers)
            {
                int stat = worker.GetStat(room.Data.requiredStat);
                // stat 1 -> x0.6, stat 5 -> x1.0, stat 10 -> x1.5
                total += room.Data.baseProductionPerWorker * (0.5f + stat / 10f);
            }

            // kazdy poziom wyzej = +50%
            return total * (1f + 0.5f * (room.Level - 1));
        }

        // kopalnia czasem daje troche zlomu
        private void MineScrapBonus(Room room)
        {
            foreach (var worker in room.Workers)
            {
                if (Random.value < mineScrapChance)
                    ResourceManager.Instance.Add(ResourceType.Scrap, mineScrapPerWorker);
            }
        }

        // lecznica leczy wszystkich w srodku, kazdy poziom wyzej = +50%
        private void Heal(Room room)
        {
            int amount = Mathf.RoundToInt(healPerDay * (1f + 0.5f * (room.Level - 1)));

            // kopia listy, bo wypisanie zdrowych ja zmienia
            foreach (var patient in new List<ResidentData>(room.Workers))
            {
                patient.health = Mathf.Min(100, patient.health + amount);

                if (patient.health >= 100)
                {
                    room.Remove(patient);
                    GameEvents.Notify(patient.residentName + " wyzdrowial");
                }
            }
        }

        private void LogResources(int day)
        {
            string text = $"Koniec dnia {day}: ";
            foreach (ResourceType t in System.Enum.GetValues(typeof(ResourceType)))
                text += $"{t} {ResourceManager.Instance.Get(t):0.#}  ";
            Debug.Log(text);
        }
    }
}