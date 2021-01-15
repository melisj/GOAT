using Goat.Grid.UI;
using UnityAtoms;
using UnityAtoms.BaseAtoms;
using UnityEngine;

public class HideUIOnDay : MonoBehaviour, IAtomListener<bool>
{
    [SerializeField] private RectTransform[] uiToHide;
    [SerializeField] private BoolEvent onCycleChange;
    [SerializeField] private VoidEvent onCloseSideBar;
    [SerializeField] private VoidEvent onCloseButton;

    private void OnEnable()
    {
        onCycleChange.RegisterSafe(this);
    }

    private void OnDisable()
    {
        onCycleChange.UnregisterSafe(this);
    }

    public void OnEventRaised(bool isDay)
    {
        if (isDay)
            HideUI();
        else
            ShowUI();
    }

    private void HideUI()
    {
        for (int i = 0; i < uiToHide.Length; i++)
        {
            uiToHide[i].gameObject.SetActive(false);
        }

        onCloseSideBar.Raise();
        onCloseButton.Raise();
    }

    private void ShowUI()
    {
        for (int i = 0; i < uiToHide.Length; i++)
        {
            uiToHide[i].gameObject.SetActive(true);
        }
        onCloseSideBar.Raise();
    }
}