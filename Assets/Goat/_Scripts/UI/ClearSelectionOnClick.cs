using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.UI;

public class ClearSelectionOnClick : MonoBehaviour
{
    [SerializeField] private VoidEvent onCellSelectedEvent;
    [SerializeField] private Button button;

    private void Awake()
    {
        button.onClick.AddListener(ClearSelection);
    }

    private void ClearSelection()
    {
        onCellSelectedEvent.Raise();
    }
}