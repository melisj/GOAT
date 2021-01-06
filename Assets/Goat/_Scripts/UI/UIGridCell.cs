using Sirenix.OdinInspector;
using UnityEngine;

namespace Goat.UI
{
    [System.Serializable]
    public class UIGridCell
    {
        [SerializeField] private RectTransform transform;
        [SerializeField] private string resourcePath;
        [SerializeField] private bool useDifferentPrefabs;
        [SerializeField, ShowIf("useDifferentPrefabs")] private GameObject otherPrefab;
        public RectTransform Transform => transform;
        public string ResourcePath => resourcePath;
        public GameObject OtherPrefab => otherPrefab;

        public bool UseDifferentPrefabs => useDifferentPrefabs;
    }
}