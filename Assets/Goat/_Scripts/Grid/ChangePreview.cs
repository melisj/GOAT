using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grid = Goat.Grid.Grid;
using Goat.Events;

public class ChangePreview : EventListenerPlaceable
{
    [SerializeField] private Grid grid;

    public override void OnEventRaised(Placeable value)
    {
        grid.ChangePreviewObject(value);
    }
}