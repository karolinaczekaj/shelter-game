using System.Collections.Generic;
using UnityEngine;

namespace Schron.Core
{
    // Konkretny pokój postawiony na mapie.
    // KONTRAKT: nazw i parametrów metod nie zmieniamy bez ustalenia z resztą.
    // Środek metod uzupełnia osoba od ekonomii i budowania.
    public class Room : MonoBehaviour
    {
        public RoomDataSO Data;
        public Vector2Int GridPosition;
        public int Level = 1;
        public List<ResidentData> Workers = new List<ResidentData>();

        public bool IsFull => Data != null && Workers.Count >= Data.maxWorkers;

        // Dodaje mieszkańca do pokoju. Zwraca false, jeśli nie ma miejsca.
        public bool TryAssign(ResidentData resident)
        {
            // TODO
            return false;
        }

        // Wypisuje mieszkańca z pokoju.
        public void Remove(ResidentData resident)
        {
            // TODO
        }

        // Ulepsza pokój o jeden poziom. Zwraca false, jeśli brakuje zasobów lub to max poziom.
        public bool TryUpgrade()
        {
            // TODO
            return false;
        }
    }
}
