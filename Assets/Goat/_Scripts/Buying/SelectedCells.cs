using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SelectedCells : MonoBehaviour
{
    [SerializeField, ReadOnly] protected List<UICell> cells = new List<UICell>();

    public virtual void Add(UICell cell)
    {
        cells.Add(cell);
    }

    public virtual void Subtract(UICell cell)
    {
        cells.Remove(cell);
    }

    protected virtual void Clear()
    {
        cells.Clear();
    }
}