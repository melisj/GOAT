using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAT.Grid.UI {
    public class EditModeUI : BasicGridUIElement
    {
        [SerializeField] private GameObject switchButton;

        public void ToggleSwitchButton(bool active) {
            switchButton.SetActive(active);
        }
    }
}

