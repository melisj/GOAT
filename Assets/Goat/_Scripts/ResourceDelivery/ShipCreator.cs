using Goat.Delivery;
using Goat.Events;
using Goat.Pooling;
using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using Sirenix.OdinInspector;
using Goat.AI.Parking;

public class ShipCreator : EventListenerDeliveryResource
{
    [SerializeField] private GameObject cargoShipPrefab;
    [SerializeField] private UnloadLocations unloadLocations;
    [SerializeField, ReadOnly] private Queue<DeliveryResource> deliveryResources = new Queue<DeliveryResource>();

    public override void OnEventRaised(DeliveryResource value)
    {
        deliveryResources.Enqueue(value);
    }

    public void CreateCargoShip()
    {
        if (deliveryResources.Count <= 0 && unloadLocations.Locations.Count <= 0) return;
        GameObject cargo = PoolManager.Instance.GetFromPool(cargoShipPrefab, transform.position, Quaternion.identity);
        DeliveryMovementSystem delivery = cargo.GetComponent<DeliveryMovementSystem>();
        delivery.SetupMultiDelivery(deliveryResources);
    }
}