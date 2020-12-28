using Goat.Grid.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.Grid.Interactions.UI
{
    [System.Serializable]
    public class UISlotElement : MonoBehaviour
    {
        [SerializeField] private InteractableUIElement uiElementType;

        public InteractableUIElement UiElementType => uiElementType;

        public virtual void InitUI()
        {
        }

        public virtual void SetUI(object[] args)
        {
        }
    }
}