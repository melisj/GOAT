using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.UI;

namespace Goat.UI
{
    public class CloseButton : MonoBehaviour
    {
        [SerializeField] private VoidEvent closeButtonEvent;
        [SerializeField] private Button closeButton;

        private void Awake()
        {
            closeButton.onClick.AddListener(closeButtonEvent.Raise);
        }
    }
}