using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.UI;

namespace Goat.UI
{
    public class ChangeInputModeOnClick : MonoBehaviour
    {
        [SerializeField] private VoidEvent onBarClicked;
        [SerializeField] private InputModeVariable inputMode;
        [SerializeField] private Button button;

        private void Awake()
        {
            button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            inputMode.InputMode = InputMode.Destroy;
            onBarClicked.Raise();
        }
    }
}