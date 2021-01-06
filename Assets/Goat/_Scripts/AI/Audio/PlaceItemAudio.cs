using System;
using UnityEngine;
using Goat.Storage;
using Goat.Grid.Interactions;

public class PlaceItemAudio : AudioCue
{
    [SerializeField] private StorageInteractable storage;

    private void Awake()
    {
        parent = transform.parent;
    }

    private void OnEnable()
    {
        storage.Inventory.InventoryAddedEvent += Inventory_InventoryAddedEvent;
    }

    private void OnDisable()
    {
        storage.Inventory.InventoryAddedEvent -= Inventory_InventoryAddedEvent;
    }

    private void Inventory_InventoryAddedEvent(object sender, EventArgs e)
    {
        PlayAudioCue();
    }
}