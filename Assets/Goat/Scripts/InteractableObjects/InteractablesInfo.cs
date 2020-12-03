using UnityEngine;

namespace Goat.Grid.Interactions
{
    [CreateAssetMenu(fileName = "InteractableInfo", menuName = "ScriptableObjects/InteractableInfo")]
    public class InteractablesInfo : ScriptableObject {
        public delegate void SelectedInteractableChangeEvent(BaseInteractable newInteractable);
        public event SelectedInteractableChangeEvent SelectedInteractableChangeEvt;

        [Header("Prefabs")]
        [SerializeField] private GameObject StorageIconPrefab;

        [Header("Names"), Space(10)]
        [SerializeField] private string ItemHolderName = "ItemHolder";
        [SerializeField] private string ItemHolderParentName = "ItemHolderParent";

        [Header("Highlight Material"), Space(10)]
        [SerializeField] private Material ItemMaterial;

        [Header("Runtime Variables"), Space(10)]
        [SerializeField] private BaseInteractable currentSelected;

        public BaseInteractable CurrentSelected { 
            get => currentSelected; 
            set {
                if (currentSelected != value)
                    SelectedInteractableChangeEvt?.Invoke(value);
                currentSelected = value;
            } 
        }
    }
}