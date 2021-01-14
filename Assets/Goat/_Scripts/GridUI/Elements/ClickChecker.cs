using UnityEngine;

namespace Goat.Grid.Interactions.UI
{
    public enum ClickMode
    {
        normalClick,
        shiftClick
    }

    public class ClickChecker : MonoBehaviour
    {
        [SerializeField] private ClickModeVariable clickModeVariable;

        private void Update()
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                clickModeVariable.ClickMode = ClickMode.shiftClick;
            }
            else
            {
                clickModeVariable.ClickMode = ClickMode.normalClick;
            }
        }
    }
}