using Goat.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SatisfactionToText : EventListenerInt
{
    [SerializeField] private TextMeshProUGUI satisfactionText;
    [SerializeField] private Image icon;
    [SerializeField] private SatisfactionSprites satisfactionSprites;

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
        if (satisfactionText)
            satisfactionText.text = value.ToString();
        icon.sprite = value > 0 ? satisfactionSprites.Happy : value == 0 ? satisfactionSprites.Neutral : satisfactionSprites.UnHappy;
    }
}