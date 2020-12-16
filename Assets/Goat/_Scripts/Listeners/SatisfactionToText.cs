using Goat.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SatisfactionToText : EventListenerInt
{
    [SerializeField] private TextMeshProUGUI satisfactionText;
    [SerializeField] private Image icon;
    [SerializeField] private Sprite happyIcon;
    [SerializeField] private Sprite unHappyIcon;
    [SerializeField] private Sprite neutralIcon;

    private void Awake()
    {
        ChangeText(0);
    }

    public override void OnEventRaised(int value)
    {
        ChangeText(value);
    }

    private void ChangeText(int value)
    {
        satisfactionText.text = value.ToString();
        icon.color = value > 0 ? Color.green : value == 0 ? Color.white : Color.red;
        icon.sprite = value > 0 ? happyIcon : value == 0 ? neutralIcon : unHappyIcon;
    }
}