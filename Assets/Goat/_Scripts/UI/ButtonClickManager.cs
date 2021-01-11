using UnityEngine;
using UnityEngine.UI;

public class ButtonClickManager : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private IButtonClick[] buttonClicks;

    private void Awake()
    {
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        for (int i = 0; i < buttonClicks.Length; i++)
        {
            buttonClicks[i].OnClick();
        }
    }
}