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
    [SerializeField] private VoidEvent deliveryIncoming;
    [SerializeField, ReadOnly] private Queue<DeliveryResource> deliveryResources = new Queue<DeliveryResource>();

    public override void OnEventRaised(DeliveryResource value)
    {
        //GameObject cargo = PoolManager.Instance.GetFromPool(cargoShipPrefab, transform.position, Quaternion.identity);
        //DeliveryMovementSystem delivery = cargo.GetComponent<DeliveryMovementSystem>();
        //delivery.SetupDelivery(value.Buyable, value.Amount);
        // deliveryIncoming.Raise();
        deliveryResources.Enqueue(value);
    }

    public void CreateCargoShip()
    {
        GameObject cargo = PoolManager.Instance.GetFromPool(cargoShipPrefab, transform.position, Quaternion.identity);
        DeliveryMovementSystem delivery = cargo.GetComponent<DeliveryMovementSystem>();
        delivery.SetupMultiDelivery(deliveryResources);
    }
}