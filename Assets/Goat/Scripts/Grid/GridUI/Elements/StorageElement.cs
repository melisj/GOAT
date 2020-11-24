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
    public class StorageElement : UISlotElement
    {
        [SerializeField] private TextMeshProUGUI itemsText;
        [SerializeField] private Transform gridParent;
        [SerializeField] private string ItemIconName = "ItemIcon";
        private List<Image> itemIcons = new List<Image>();
        private List<Button> itemButtons = new List<Button>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"> 0 = max amount of grid elements </param>
        public override void InitStorageUI(object[] args) {
            base.InitStorageUI(args);

            GameObject prefab = (GameObject)Resources.Load(ItemIconName);

            if (args.Length == 1) {
                for (int i = 0; i < (int)args[0]; i++) {
                    GameObject instance = Instantiate(prefab, gridParent);
                    instance.SetActive(false);
                    itemIcons.Add(instance.GetComponent<Image>());
                    itemButtons.Add(instance.GetComponent<Button>());
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"> 0 = item text : 1 = items </param>
        public override void SetUI(object[] args) {
            base.SetUI(args);
            if (args.Length != 3)
                return;

            itemsText.text = args[0].ToString();
            if (args[1] is List<ItemInstance>) {
                List<ItemInstance> itemList = (List<ItemInstance>)args[1];
                for (int i = 0; i < itemIcons.Count; i++) {
                    itemButtons[i].onClick.RemoveAllListeners();

                    if (i >= itemList.Count) {
                        itemIcons[i].gameObject.SetActive(false);
                        continue;
                    }

                    itemIcons[i].gameObject.SetActive(true);
                    itemIcons[i].sprite = itemList[i].GetResource.Image;

                    if (args[2] is StorageInteractable) {
                        int index = i;
                        itemButtons[i].onClick.AddListener(() => { ((StorageInteractable)args[2]).RemoveResource(index);  });
                    }
                }
            }
        }
    }
}