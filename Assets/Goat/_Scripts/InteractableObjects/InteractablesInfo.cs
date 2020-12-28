using UnityEngine;

namespace Goat.Grid.Interactions
{
    [CreateAssetMenu(fileName = "InteractableInfo", menuName = "ScriptableObjects/RuntimeVariables/InteractableInfo")]
    public class InteractablesInfo : ScriptableObject
    {
        public delegate void InteractableChangeEvent(BaseInteractable interactable);

        public event InteractableChangeEvent SelectedInteractableChangeEvt;

        public delegate void InteractableUpdateEvent(BaseInteractable interactable);

        public event InteractableUpdateEvent InteractableUpdateEvt;

        [Header("Prefabs")]
        [SerializeField] private GameObject storageIconPrefab;
        [SerializeField] private GameObject inventoryIconPrefab;

        [Header("Names"), Space(10)]
        [SerializeField] private string itemHolderName = "ItemHolder";
        [SerializeField] private string itemHolderParentName = "ItemHolderParent";

        [Header("Highlight Material"), Space(10)]
        [SerializeField] private Material itemMaterial;

        [Header("Runtime Variables"), Space(10)]
        [SerializeField] private BaseInteractable currentSelected;

        public GameObject StorageIconPrefab => storageIconPrefab;
        public GameObject InventoryIconPrefab => inventoryIconPrefab;
        public string ItemHolderName => itemHolderName;
        public string ItemHolderParentName => itemHolderParentName;
        public Material ItemMaterial => itemMaterial;

        public BaseInteractable CurrentSelected
        {
            get => currentSelected;
            set
            {
                currentSelected = value;
                SelectedInteractableChangeEvt?.Invoke(value);

                if (value != null)
                {
                    InteractableUpdateEvt?.Invoke(value);
                }
            }
        }

        public void UpdateInteractable()
        {
            if (CurrentSelected != null)
                InteractableUpdateEvt?.Invoke(CurrentSelected);
        }
    }
}