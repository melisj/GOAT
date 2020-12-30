using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Goat.Grid.UI
{
    public enum UIElement
    {
        None,
        Tiles,
        Furniture,
        Harvester,
        Buying,
        Interactable,
        Employees,
        Review,
        Income
    }

    [CreateAssetMenu(fileName = "GridUIInfo", menuName = "ScriptableObjects/UI/GridUIInfo")]
    public class GridUIInfo : SerializedScriptableObject
    {
        [SerializeField] private UIElement currentUI;

        public delegate void GridUIChanged(UIElement currentUI, UIElement prevUI);

        public event GridUIChanged GridUIChangedEvent;

        public UIElement CurrentUIElement
        {
            get => currentUI;
            set
            {
                // Reset to none when same is selected
                UIElement newUI = value;
                if (currentUI == newUI)
                    newUI = UIElement.None;

                // Invoke new UI change
                if (currentUI != newUI)
                    GridUIChangedEvent?.Invoke(value, currentUI);
                currentUI = newUI;
            }
        }

        public bool IsUIActive { get => currentUI != UIElement.None; }
    }
}