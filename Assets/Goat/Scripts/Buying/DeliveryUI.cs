using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.Pooling;
using UnityEngine.UI;
using TMPro;

namespace Goat.UI
{
    public class DeliveryUI : CellWithAmount, IPoolObject
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Image image;
        [SerializeField] private Image progressBar;

        public int PoolKey { get; set; }
        public ObjectInstance ObjInstance { get; set; }
        public Image ProgressBar => progressBar;

        public void Setup(Buyable buyable, int amount)
        {
            nameText.text = buyable.name;
            image.sprite = buyable.Image;
            ChangeText(amount);
        }

        public void OnGetObject(ObjectInstance objectInstance, int poolKey)
        {
            ObjInstance = objectInstance;
            PoolKey = poolKey;
        }

        public void OnReturnObject()
        {
            gameObject.SetActive(false);
            PoolManager.Instance.SetParent(gameObject);
        }
    }
}