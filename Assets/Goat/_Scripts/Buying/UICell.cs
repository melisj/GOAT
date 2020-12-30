using Goat.Events;
using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityAtoms;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.UI;

public enum BorderChange
{
    none,
    color,
    sprite
}

public class UICell : MonoBehaviour, IAtomListener<UnityAtoms.Void>
{
    [SerializeField] protected Button imageButton;
    [SerializeField] private Image icon;
    [SerializeField] private bool hasBorder;
    [SerializeField, ShowIf("hasBorder")] protected Image border;
    [SerializeField, ShowIf("showEvent")] private VoidEvent onSelectEvent;
    [SerializeField, ShowIf("hasBorder")] private BorderChange borderChangeType;
    [SerializeField, ShowIf("hasBorder")] private bool multipleSelections;
    [SerializeField, ShowIf("borderChangeType", BorderChange.color), ColorPalette] private Color selectedColor;
    [SerializeField, ShowIf("borderChangeType", BorderChange.color), ColorPalette] private Color deselectedColor;
    [SerializeField, ShowIf("borderChangeType", BorderChange.sprite)] private Sprite selectedSprite;
    [SerializeField, ShowIf("borderChangeType", BorderChange.sprite)] private Sprite deselectedSprite;

    [SerializeField] private bool hasName;
    [SerializeField, ShowIf("hasName")] protected TextMeshProUGUI nameText;
    private bool selected;
    private bool showEvent => hasBorder && !multipleSelections;
    public Image Border => border;

    public Image Icon => icon;

    private void OnEnable()
    {
        if (hasBorder && !multipleSelections && onSelectEvent)
            onSelectEvent.RegisterListener(this);
    }

    private void OnDisable()
    {
        if (hasBorder && !multipleSelections && onSelectEvent)
            onSelectEvent.UnregisterListener(this);
    }

    public void OnClick(Action onClick)
    {
        if (hasBorder)
            imageButton.onClick.AddListener(() => OnSelect());
        imageButton.onClick.AddListener(() => onClick());
    }

    public void InvokeOnClick()
    {
        imageButton.onClick.Invoke();
    }

    public void OnSelect()
    {
        if (!multipleSelections)
            onSelectEvent.Raise();
        else if (selected)
        {
            OnDeselect();
            return;
        }

        selected = true;

        switch (borderChangeType)
        {
            case BorderChange.color:
                border.color = selectedColor;
                break;

            case BorderChange.sprite:
                border.sprite = selectedSprite;
                break;
        }
    }

    public void OnDeselect()
    {
        selected = false;
        switch (borderChangeType)
        {
            case BorderChange.color:
                border.color = deselectedColor;
                break;

            case BorderChange.sprite:
                border.sprite = deselectedSprite;
                break;
        }
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

    public void OnEventRaised(UnityAtoms.Void item)
    {
        if (hasBorder && selected)
            OnDeselect();
    }
}