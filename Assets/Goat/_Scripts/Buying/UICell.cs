using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICell : MonoBehaviour
{
    [SerializeField] protected Button imageButton;
    [SerializeField] private Image icon;
    [SerializeField] private bool hasBorder;
    [SerializeField, ShowIf("hasBorder")] protected Image border;
    [SerializeField] private bool hasName;
    [SerializeField, ShowIf("hasName")] protected TextMeshProUGUI nameText;

    public Image Border => border;

    public void OnClick(Action onClick)
    {
        imageButton.onClick.AddListener(() => onClick());
    }

    public virtual void Setup(Buyable buyable)
    {
        gameObject.name = buyable.name;
        if (hasName)
            nameText.text = buyable.name;
        icon.sprite = buyable.Image;
    }

    public void ResetOnClick()
    {
        imageButton.onClick.RemoveAllListeners();
    }
}