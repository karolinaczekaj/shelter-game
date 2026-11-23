using System.Collections;
using UnityEngine;

namespace Schron.Residents
{
    public class TestSceneBootstrap : MonoBehaviour
    {
        [SerializeField] private StartingRosterSO roster;

        private IEnumerator Start()
        {
            // Wait one frame so the rooms are registered (Room.OnEnable).
            yield return null;

            if (ResidentManager.Instance == null)
            {
                Debug.LogError("No ResidentManager in the scene!");
                yield break;
            }

            int count = ResidentManager.Instance.SpawnStartingResidents(roster);
            Debug.Log($"[TEST] Spawned {count} starting residents.");
        }
    }
}