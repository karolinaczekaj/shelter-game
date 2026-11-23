using System.Collections;
using UnityEngine;

namespace Schron.Residents
{
    public class NewGameInitializer : MonoBehaviour
    {
        [SerializeField] private StartingRosterSO startingRoster;

        private IEnumerator Start()
        {
            // For now: wait one frame so rooms placed in the scene are registered.
            // Replace with an explicit call after the starting rooms are built (to agree with the build system owner).
            yield return null;

            // Same method that was tested in the TEST scene.
            ResidentManager.Instance.SpawnStartingResidents(startingRoster);
        }
    }
}