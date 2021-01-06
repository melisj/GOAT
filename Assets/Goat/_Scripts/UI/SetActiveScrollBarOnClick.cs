using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ScrollBarAxis
{
    horizontal,
    vertical,
    both
}

public class SetActiveScrollBarOnClick : MonoBehaviour
{
    [SerializeField] private ScrollRect contentRect;
    [SerializeField] private Button button;
    [SerializeField] private bool setActive;
    [SerializeField] private ScrollBarAxis barAxis;

    private void Awake()
    {
        button.onClick.AddListener(SetActive);
    }

    private void SetActive()
    {
        switch (barAxis)
        {
            case ScrollBarAxis.horizontal:
                contentRect.horizontal = setActive;
                break;

            case ScrollBarAxis.vertical:
                contentRect.vertical = setActive;
                break;

            case ScrollBarAxis.both:
                contentRect.horizontal = setActive;
                contentRect.vertical = setActive;
                break;
        }
    }
}