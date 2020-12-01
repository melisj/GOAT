using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Goat.Storage
{
    public class ResourceUI : CellWithAmount
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI hotKeyText;
        [SerializeField] private Image image;
        [SerializeField] private Button imageButton;
        private int alpha = 48;
        private int numpad = 256;
        private int index;

        public Button ImageButton => imageButton;

        //private void Awake()
        //{
        //    image.sprite = resource.Image;
        //    textMesh.text = resource.ResourceType.ToString() + ": " + resource.Amount.ToString();
        //    resource.AmountChanged += Resource_AmountChanged;
        //}

        public void SetupUI(Resource res, int index = 0)
        {
            this.index = index + 1;
            Setup(res);
            image.sprite = res.Image;
            nameText.text = res.name;
            hotKeyText.text = this.index.ToString();
            if (index <= 9)
            {
                InputManager.Instance.OnInputEvent += Instance_OnInputEvent;
            }
        }

        //49 and 257 start
        private void Instance_OnInputEvent(KeyCode code, InputManager.KeyMode keyMode, InputMode inputMode)
        {
            if (code == (KeyCode)alpha + index | code == (KeyCode)numpad + index)
            {
                if (keyMode == InputManager.KeyMode.Down)
                {
                    imageButton.onClick.Invoke();
                }
            }
        }
    }
}