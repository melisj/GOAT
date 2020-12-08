using Goat.Delivery;
using Goat.Events;
using Goat.Pooling;
using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using Sirenix.OdinInspector;

public class ShipCreator : EventListenerDeliveryResource
{
    [SerializeField] private GameObject cargoShipPrefab;
    [SerializeField, ReadOnly] private Queue<DeliveryResource> deliveryResources = new Queue<DeliveryResource>();

    public override void OnEventRaised(DeliveryResource value)
    {
        deliveryResources.Enqueue(value);
    }

    public void CreateCargoShip()
    {
        GameObject cargo = PoolManager.Instance.GetFromPool(cargoShipPrefab, transform.position, Quaternion.identity);
        DeliveryMovementSystem delivery = cargo.GetComponent<DeliveryMovementSystem>();
        delivery.SetupMultiDelivery(deliveryResources);
    }
}