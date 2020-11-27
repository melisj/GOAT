using Goat.Grid.Interactions;
using System.Collections.Generic;
using Goat.Storage;

namespace Goat.Manager
{
    public class GameManager
    {
        private static GameManager instance;
        // targets
        private List<StorageInteractable> storageShelves = new List<StorageInteractable>();
        // boodschappen
        private Dictionary<ResourceType, int> resources = new Dictionary<ResourceType, int>();
        public List<StorageInteractable> StorageShelves { get => storageShelves; }

        public Dictionary<ResourceType, int> AvailableResources { get => resources; }

        public int money = 1;

        private GameManager()
        {
        }

        public static GameManager Instance { get { if (instance == null) { instance = new GameManager(); } return instance; } }

        public void AddStorageShelve(StorageInteractable storage)
        {
            storageShelves.Add(storage);
        }

        public void RemoveStorageShelve(StorageInteractable storage)
        {
            storageShelves.Remove(storage);
        }

        public void AddAvailableResource(ResourceType type, int amount)
        {
            if (resources.ContainsKey(type))
                resources[type] += amount;
            else
                resources.Add(type, amount);
        }

        public void RemoveAvailableResource(ResourceType type, int amount)
        {
            if (resources.ContainsKey(type))
            {
                resources[type] -= amount;
                if (resources[type] <= 0) resources.Remove(type);
            }
        }
    }
}
