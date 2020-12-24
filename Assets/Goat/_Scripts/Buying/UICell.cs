using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICell : MonoBehaviour
{
    [SerializeField] private Button imageButton;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI nameText;

    public void OnClick(Action onClick)
    {
        imageButton.onClick.AddListener(() => onClick());
    }

    public virtual void Setup(Buyable buyable)
    {
        gameObject.name = buyable.name;
        nameText.text = buyable.name;
        image.sprite = buyable.Image;
    }
}