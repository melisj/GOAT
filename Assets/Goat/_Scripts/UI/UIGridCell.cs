using UnityEngine;

namespace Goat.UI
{
    [System.Serializable]
    public class UIGridCell
    {
        [SerializeField] private RectTransform transform;
        [SerializeField] private string resourcePath;

        public RectTransform Transform => transform;
        public string ResourcePath => resourcePath;
    }
}