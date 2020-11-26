using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.Grid.UI
{
    public class EditModeUI : BasicGridUIElement
    {
        [SerializeField] private GameObject switchButton;

        public void ToggleSwitchButton(bool active)
        {
            switchButton.SetActive(active);
        }
        public void SetPreviewFloor(int type)
        {
            grid.ChangePreviewObject(TilePartEditing.Floor, type);
        }
        public void SetPreviewBuilding(int type)
        {
            grid.ChangePreviewObject(TilePartEditing.Building, type);
        }
        public void SetPreviewWall(int type)
        {
            grid.ChangePreviewObject(TilePartEditing.Wall, type);
        }

        public void EnterExitEditMode()
        {
            grid.EnterExitEditMode();
        }
    }
}

