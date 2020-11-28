using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class MoneyToText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private Money money;
    [SerializeField] private float animationDuration;
    private Sequence changeOverTimeSequence;

    private void Awake()
    {
        ChangeText(money.Amount);
        money.AmountChanged += Money_AmountChanged;
        changeOverTimeSequence = DOTween.Sequence();
    }

    private void ChangeText(float value)
    {
        moneyText.text = value.ToString();
    }

    private void Money_AmountChanged(object sender, float e)
    {
        float startValue = money.OldAmount;
        changeOverTimeSequence.Append(DOTween.To(() => startValue, x => startValue = x, e, animationDuration)
            .OnUpdate(() => { ChangeText(startValue); }));
    }
}