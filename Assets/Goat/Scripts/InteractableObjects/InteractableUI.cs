using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GOAT.Grid.UI
{
    public class InteractableUI : BasicGridUIElement
    {
        [Space(20)]

        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI infoText;
        [SerializeField] private Image interactableIcon;

        public void Awake() {
            HideUI();
        }

        public void SetUI(string title, string description, string info) {
            titleText.text = title;
            descriptionText.text = description;
            infoText.text = info;
        }

        // [TODO] For icon a type of object needs to be send in a enum
    }
}