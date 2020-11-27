using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goat.Pooling;
using UnityEngine.UI;
using TMPro;

namespace Goat.Buying
{
    public class DeliveryUI : MonoBehaviour, IPoolObject
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI amountText;

        [SerializeField] private Image image;
        [SerializeField] private Image progressBar;

        public int PoolKey { get; set; }
        public ObjectInstance ObjInstance { get; set; }
        public TextMeshProUGUI Name => nameText;
        public TextMeshProUGUI Amount => amountText;

        public Image Image => image;
        public Image ProgressBar => progressBar;

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