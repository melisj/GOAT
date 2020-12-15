using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Goat.Grid.UI
{
    public enum GridUIElement
    {
        None,
        Building,
        Buying,
        Interactable
    }

    [CreateAssetMenu(fileName = "GridUIInfo", menuName = "ScriptableObjects/UI/GridUIInfo")]
    public class GridUIInfo : SerializedScriptableObject
    {
        [SerializeField] private GridUIElement currentUI;

        public delegate void GridUIChanged(GridUIElement currentUI, GridUIElement prevUI);
        public event GridUIChanged GridUIChangedEvent;

        public GridUIElement CurrentUIElement
        {
            get => currentUI;
            set
            {
                // Reset to none when same is selected
                GridUIElement newUI = value;
                if (currentUI == newUI)
                    newUI = GridUIElement.None;

                // Invoke new UI change
                if (currentUI != newUI)
                    GridUIChangedEvent?.Invoke(value, currentUI);
                currentUI = newUI;
            }
        }

        public bool IsUIActive { get => currentUI != GridUIElement.None; }
    }
}
