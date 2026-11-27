using Schron.Core;

namespace Schron.Building
{
    // ile osob moze mieszkac w schronie
    public static class ShelterCapacity
    {
        // pojemnosc jednych kwater: poziom 1 = maxWorkers, kazdy kolejny +50%
        public static int GetRoomBeds(Room room)
        {
            return room.Capacity;
        }

        // laczna liczba miejsc we wszystkich kwaterach
        public static int GetTotalBeds()
        {
            int total = 0;
            foreach (var room in Room.AllRooms)
            {
                if (room.Data.roomType == RoomType.Quarters)
                    total += GetRoomBeds(room);
            }
            return total;
        }

        // czy zmiesci sie jeszcze ktos
        public static bool HasFreeBed(int currentResidents)
        {
            return currentResidents < GetTotalBeds();
        }
    }
}