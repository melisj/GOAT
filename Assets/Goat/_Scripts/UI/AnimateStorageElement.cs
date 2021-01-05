using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Goat.UI
{
    public class AnimateStorageElement : MonoBehaviour
    {
        [SerializeField] private ScrollRect[] contents;
        [SerializeField] private float contentScaleDuration;
        [SerializeField, Range(2, 5)] private int closingMultiplier;
        private Sequence storageSequence;

        /// <summary>
        /// (1): Scale all content StoredItems
        /// (2): Scale all content AcceptedItems
        /// </summary>
        public void OpenStorage()
        {
            if (storageSequence.NotNull())
                storageSequence.Complete();

            storageSequence = DOTween.Sequence();
            storageSequence.OnComplete(() => gameObject.SetActive(true));
            for (int i = 0; i < contents.Length; i++)
            {
                for (int j = 0; j < contents[i].content.childCount; j++)
                {
                    Transform child = contents[i].content.GetChild(j);
                    storageSequence.Append(child.DOScale(Vector3.one, contentScaleDuration));
                }
            }
        }

        /// <summary>
        /// (1): Down scale all content StoredItems
        /// (2): Down scale all content AcceptedItems
        /// </summary>
        public void CloseStorage()
        {
            if (storageSequence.NotNull())
                storageSequence.Complete();

            storageSequence = DOTween.Sequence();
            storageSequence.OnComplete(() => gameObject.SetActive(false));

            for (int i = 0; i < contents.Length; i++)
            {
                for (int j = 0; j < contents[i].content.childCount; j++)
                {
                    Transform child = contents[i].content.GetChild(j);
                    storageSequence.Append(child.DOScale(Vector3.zero, contentScaleDuration / closingMultiplier));
                }
            }
        }
    }
}