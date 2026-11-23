using UnityEngine;

namespace Schron.Core
{
    public class ResidentController : MonoBehaviour
    {
        public ResidentData Data { get; private set; }

        public void Initialize(ResidentData data)
        {
            Data = data;
            gameObject.name = $"Resident_{data.residentName}";
        }

        // For now instant relocation. Later I'll add
        // moving and ResidentState.Moving state.
        public void MoveToRoom(Room room)
        {
            if (room == null)
                return;

            int index = Mathf.Max(0, room.Workers.IndexOf(Data));
            transform.position = room.transform.position + new Vector3(index * 0.5f, 0f, 0f);
        }
    }
}