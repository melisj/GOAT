using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HideShowOnClick : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private ShowHideElement[] elements;

    private void Awake()
    {
        button.onClick.AddListener(SetActive);
    }

    private void SetActive()
    {
        for (int i = 0; i < elements.Length; i++)
        {
            ShowHideElement element = elements[i];

            if (element.UiHideType == UIHide.transform)
                element.Transform.gameObject.SetActive(element.Show);
            else if (element.UiHideType == UIHide.canvas)
                element.Canvas.enabled = element.Show;
        }
    }
}