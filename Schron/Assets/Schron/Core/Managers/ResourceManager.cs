using System;
using System.Collections.Generic;
using UnityEngine;

namespace Schron.Core
{
    public class ResourceManager : MonoBehaviour
    {
        public static ResourceManager Instance { get; private set; }

        [Header("Start Resources")]
        [SerializeField] private List<ResourceAmount> startResources = new List<ResourceAmount>();

        private Dictionary<ResourceType, float> resources = new Dictionary<ResourceType, float>();

        private void Awake()
        {
            Instance = this;

            foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
                resources[type] = 0;

            foreach (var res in startResources)
                resources[res.type] = res.amount;
        }

        public float Get(ResourceType type) => resources[type];

        public void Add(ResourceType type, float value)
        {
            resources[type] = Mathf.Max(0, resources[type] + value);
            ShelterActions.ResourceChanged(type, resources[type]);
        }

        public bool TrySpend(ResourceType type, float cost)
        {
            if (resources[type] < cost)
                return false;

            Add(type, -cost);
            return true;
        }

        public bool CanAfford(List<ResourceAmount> cost)
        {
            foreach (var c in cost)
            {
                if (resources[c.type] < c.amount)
                    return false;
            }
            return true;
        }

        // all or nothing
        public bool TrySpend(List<ResourceAmount> cost)
        {
            if (!CanAfford(cost))
                return false;

            foreach (var c in cost)
                Add(c.type, -c.amount);

            return true;
        }

        // morale schronu = srednia z mieszkancow, wolane raz na dzien
        public void UpdateShelterMorale(List<ResidentData> residents)
        {
            float avg = 0;

            if (residents != null && residents.Count > 0)
            {
                foreach (var r in residents)
                    avg += r.morale;
                avg /= residents.Count;
            }

            resources[ResourceType.Morale] = avg;
            ShelterActions.ResourceChanged(ResourceType.Morale, avg);
        }
    }
}