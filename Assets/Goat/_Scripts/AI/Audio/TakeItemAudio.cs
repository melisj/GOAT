using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Goat.Storage;
using Goat.Grid.Interactions;

public class TakeItemAudio : AudioCue
{
    [SerializeField] private StorageInteractable storage;

    private void OnEnable()
    {
        storage.Inventory.InventoryRemovedEvent += Inventory_InventoryRemovedEvent;
    }

    private void OnDisable()
    {
        storage.Inventory.InventoryRemovedEvent -= Inventory_InventoryRemovedEvent;
    }

    private void Inventory_InventoryRemovedEvent(object sender, EventArgs e)
    {
        PlayAudioCue();
    }
}
