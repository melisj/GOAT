using UnityEngine;
using UnityEngine.UI;

public abstract class OnButtonClick : MonoBehaviour
{
    [SerializeField] private bool manualSubscribe;
    [SerializeField] private Button button;

    protected virtual void Awake()
    {
        if (!manualSubscribe)
            button.onClick.AddListener(OnClick);
    }

    public abstract void OnClick();
}