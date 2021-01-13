using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class OnHoverInfo : MonoBehaviour
{
    [SerializeField] private string infoToShow;

    public string InfoToShow { get => infoToShow; set => infoToShow = value; }

#if UNITY_EDITOR

    private void OnValidate()
    {
        if (!gameObject.CompareTag("HoverInfo"))
            gameObject.tag = "HoverInfo";
    }

#endif
}