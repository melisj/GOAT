using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SelectAllContentOnClick : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Button button;

    private void Awake()
    {
        button.onClick.AddListener(SelectAllContent);
    }

    private void SelectAllContent()
    {
        for (int i = 0; i < scrollRect.content.transform.childCount; i++)
        {
            UICell cell = scrollRect.content.transform.GetChild(i).GetComponent<UICell>();
            cell.InvokeOnClick();
        }
    }
}