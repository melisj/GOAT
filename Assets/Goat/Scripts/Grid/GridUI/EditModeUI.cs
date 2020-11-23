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
            grid.ChangePreviewObject(false, type);
        }
        public void SetSelectionFloor(int type)
        {
            grid.ChangePreviewObject(true, type);
        }
        public void EnterExitEditMode()
        {
            grid.EnterExitEditMode();
        }
    }
}

