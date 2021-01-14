using UnityAtoms;
using UnityEngine;

namespace Goat.Grid.Interactions.UI
{
    [EditorIcon("atom-icon-lush")]
    [CreateAssetMenu(menuName = "Unity Atoms/Variables/ClickModeVariable", fileName = "ClickModeVariable")]
    public class ClickModeVariable : ScriptableObject
    {
        [SerializeField] private ClickMode clickMode;

        public ClickMode ClickMode
        {
            get => clickMode;
            set
            {
                clickMode = value;
            }
        }
    }
}