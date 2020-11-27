using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GOAT.Grid.UI
{
    [Serializable]
    public class OnClickStorageItem : UnityEvent<ItemInstance> { }

    /// <summary>
    /// UI element for storage objects
    /// </summary>
    public class StorageElement : UISlotElement
    {
        [SerializeField] private TextMeshProUGUI itemsText;
        [SerializeField] private Transform gridParent;
        [SerializeField] private string StorageIconPrefabname = "ItemIcon";
        private GameObject StorageIconPrefab;
        private List<Image> itemIcons = new List<Image>();
        private List<Button> itemButtons = new List<Button>();

        private int amountOfPrewarmedStorageElements = 10;

        private OnClickStorageItem onClickItemEvt = new OnClickStorageItem();

        /// <summary>
        /// Assign a callback for clicking on a storage item
        /// This event will return a ItemInstance of the object that was selected
        /// </summary>
        /// <param name="evt"> Callback for clicking on item in the UI </param>
        public void AssignCallback(UnityAction<ItemInstance> evt) {
            onClickItemEvt.RemoveAllListeners();
            onClickItemEvt.AddListener(evt);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void InitUI() {
            base.InitUI();
            StorageIconPrefab = (GameObject)Resources.Load(StorageIconPrefabname);

            for (int i = 0; i < amountOfPrewarmedStorageElements; i++) {
                AddStorageIcon();
            }
        }

        // Create a new storage icon
        private void AddStorageIcon() {
            GameObject instance = Instantiate(StorageIconPrefab, gridParent);
            instance.SetActive(false);
            itemIcons.Add(instance.GetComponent<Image>());
            itemButtons.Add(instance.GetComponent<Button>());
        }

        // Disable a storage icon
        private void DisableIcon(int iconIndex) {
            itemIcons[iconIndex].gameObject.SetActive(false);
        }

        // Enable a storage icon with a new sprite
        private void EnableIcon(int iconIndex, Sprite newIcon) {
            // Reset listeners
            itemButtons[iconIndex].onClick.RemoveAllListeners();

            // Set the element active and set the icon
            itemIcons[iconIndex].gameObject.SetActive(true);
            itemIcons[iconIndex].sprite = newIcon;
        }

        /// <summary>
        /// Sets the storage UI up with the items given by the arguments
        /// </summary>
        /// <param name="args"> 0 = item text : 1 = items : 2 = StorageInteractable </param>
        public override void SetUI(object[] args) {
            base.SetUI(args);
            if (args.Length != 3)
                return;

            itemsText.text = args[0].ToString();
            if (args[1] is List<ItemInstance>) {
                List<ItemInstance> itemList = (List<ItemInstance>)args[1];
                // Add icons if pool is not enough
                while(itemList.Count > itemIcons.Count) {
                    AddStorageIcon();
                }

                for (int i = 0; i < itemIcons.Count; i++) {
                    // Disable the items that are not being used
                    if (i >= itemList.Count) {
                        DisableIcon(i);
                        continue;
                    }

                    EnableIcon(i, itemList[i].GetResource.Image);

                    // Add the custom event to the resource
                    if (args[2] is StorageInteractable) {
                        int index = i;
                        itemButtons[i].onClick.AddListener(() => {
                            onClickItemEvt?.Invoke(((StorageInteractable)args[2]).GetResource(index)); 
                        });
                    }
                }
            }
        }
    }
}