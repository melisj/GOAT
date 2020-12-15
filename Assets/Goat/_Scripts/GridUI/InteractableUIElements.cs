using System;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;

namespace Goat.Grid.UI
{
    [CreateAssetMenu(fileName = "InteractableUIElements", menuName = "ScriptableObjects/GlobalVariables/InteractableUIElements")]
    public class InteractableUIElements : ScriptableObject
    {
        [SerializeField] private GameObject[] interactableUIElements;

        public GameObject[] GetInteractableUIElements => interactableUIElements;

        [Button]
        private void Load()
        {
            int elementAmount = Enum.GetValues(typeof(InteractableUIElement)).Length;
            interactableUIElements = new GameObject[elementAmount - 1];

            for (int i = 0, j = 1; i < elementAmount - 1; i++, j++)
            {
                string uiElementName = ((InteractableUIElement)j).ToString();
                interactableUIElements[i] = (GameObject)Resources.Load("InteractableUIElement-" + uiElementName);
            }
        }
    }
}