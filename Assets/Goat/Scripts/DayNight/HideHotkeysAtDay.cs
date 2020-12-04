using Goat.Events;
using Goat.Grid.UI;
using UnityEngine;

namespace Goat
{
    public class HideHotkeysAtDay : EventListenerBool
    {
        [SerializeField] private GameObject[] hotkeys;
        [SerializeField] private GridUIInfo gridUIInfo;

        public override void OnEventRaised(bool value)
        {
            for (int i = 0; i < hotkeys.Length; i++)
            {
                hotkeys[i].SetActive(!value);
            }
            gridUIInfo.CurrentUIElement = GridUIElement.None;
        }
    }
}