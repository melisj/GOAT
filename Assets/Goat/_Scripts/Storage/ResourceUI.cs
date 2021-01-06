using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Goat.Storage
{
    public class ResourceUI : CellWithAmount
    {
        [SerializeField] private TextMeshProUGUI hotKeyText;
        private int alpha = 48;
        private int numpad = 256;
        private int index;

        public Button ImageButton => imageButton;

        public void SetupUI(Resource res, int index = 0)
        {
            Setup(res);
            this.index = index + 1;
            hotKeyText.text = this.index.ToString();
            if (index <= 9)
            {
                //InputManager.Instance.OnInputEvent += Instance_OnInputEvent;
            }
        }

        //49 and 257 start
        private void OnInput(KeyCode code, KeyMode keyMode, InputMode inputMode)
        {
            if (code == (KeyCode)alpha + index | code == (KeyCode)numpad + index)
            {
                if (keyMode == KeyMode.Down)
                {
                    imageButton.onClick.Invoke();
                }
            }
        }
    }
}