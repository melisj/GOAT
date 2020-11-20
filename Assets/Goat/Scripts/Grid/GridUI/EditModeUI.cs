using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAT.Grid.UI
{
    public class EditModeUI : BasicGridUIElement
    {
        [SerializeField] private GameObject switchButton;

        public void ToggleSwitchButton(bool active)
        {
            switchButton.SetActive(active);
        }
        public void SetSelectionBuilding(int type)
        {
            grid.SetSelectionBuilding(type);
        }
        public void SetSelectionFloor(int type)
        {
            grid.SetSelectionFloor(type);
        }
    }
}

