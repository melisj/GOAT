using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;
using Goat.Events;
using UnityEngine.AI;
using DG.Tweening;
using Goat.Storage;
using Goat.Pooling;
using UnityAtoms;

namespace Goat.Delivery
{
    public class DeliveryMovementSystem : EventListenerVoid, IPoolObject
    {
        [SerializeField] private UnloadLocations unload;
        [SerializeField] private float rotationDuration = 0.1f;
        [SerializeField] private float yOffset = 2;
        [SerializeField] private float baseDuration;
        [SerializeField] private float speed;
        [SerializeField] private GameObject packPrefab;
        private bool isDelivering;
        private Buyable buyable;
        private int amount;
        private Vector3 arrivePosition;
        private Queue<DeliveryResource> deliveryResources;
        private Sequence moveSequence;

        public int PoolKey { get; set; }
        public ObjectInstance ObjInstance { get; set; }

        public void SetupDelivery(Buyable buyable, int amount)
        {
            if (isDelivering) return;

            this.buyable = buyable;
            this.amount = amount;
            isDelivering = true;
            MoveTo(GetNearest(unload.Locations));
        }

        public void SetupMultiDelivery(Queue<DeliveryResource> deliveryResources)
        {
            if (isDelivering) return;

            this.deliveryResources = deliveryResources;
            isDelivering = true;
            MoveTo(GetNearest(unload.Locations));
        }

        public override void OnEventRaised(Void value)
        {
            if (isDelivering) return;

            isDelivering = true;
            MoveTo(GetNearest(unload.Locations));
        }

        private Vector3 GetNearest(List<Vector3> positions)
        {
            Vector3 nearestPos = positions[0];
            float nearestDist = 0;

            for (int i = 0; i < positions.Count; i++)
            {
                float currentDist = (transform.position - positions[i]).sqrMagnitude;
                if (currentDist < nearestDist)
                {
                    nearestDist = currentDist;
                    nearestPos = positions[i];
                }
            }
            return nearestPos;
        }

        //Speed = 10 distance per second
        //-100 to 11.5
        //
        private void MoveTo(Vector3 target)
        {
            arrivePosition = transform.position;

            Vector3 changedTarget = target;
            changedTarget.y = transform.position.y;

            float distance = (target - arrivePosition).magnitude;
            float duration = distance / (speed);

            float changedY = target.y + yOffset;

            if (moveSequence.NotNull())
                moveSequence.Complete();

            moveSequence = DOTween.Sequence();
            moveSequence.SetUpdate(UpdateType.Normal, false);
            moveSequence.Append(transform.DOLookAt(changedTarget, rotationDuration, AxisConstraint.Y));
            moveSequence.Append(transform.DOMove(changedTarget, GetDuration(target, transform.position)));
            moveSequence.Append(transform.DOMoveY(changedY, GetDuration(changedY, transform.position.y)).OnComplete(OnMultiDelivery));
        }

        private float GetDuration(Vector3 a, Vector3 b)
        {
            return Mathf.Abs((a - b).magnitude / speed);
        }

        private float GetDuration(float a, float b)
        {
            return Mathf.Abs((a - b) / speed);
        }

        private void OnDelivery()
        {
            ResourcePack resPack = PoolManager.Instance.GetFromPool(packPrefab, transform.position, Quaternion.identity).GetComponent<ResourcePack>();
            if (resPack)
            {
                resPack.SetupResPack(buyable, amount);
            }
            if (moveSequence.NotNull())
                moveSequence.Complete();

            moveSequence = DOTween.Sequence();
            moveSequence.SetUpdate(UpdateType.Normal, false);
            moveSequence.Append(transform.DOLookAt(arrivePosition, rotationDuration, AxisConstraint.Y));
            moveSequence.Append(transform.DOMove(arrivePosition, GetDuration(transform.position, arrivePosition)).OnComplete(() => { PoolManager.Instance.ReturnToPool(gameObject); }));
        }

        private void OnMultiDelivery()
        {
            while (deliveryResources.Count > 0)
            {
                DeliveryResource deliRes = deliveryResources.Peek();
                ResourcePack resPack = PoolManager.Instance.GetFromPool(packPrefab, transform.position, Quaternion.identity).GetComponent<ResourcePack>();
                if (resPack)
                {
                    resPack.SetupResPack(deliRes.Buyable, deliRes.Amount);
                }
                deliveryResources.Dequeue();
            }

            if (moveSequence.NotNull())
                moveSequence.Complete();

            moveSequence = DOTween.Sequence();
            moveSequence.Append(transform.DOLookAt(arrivePosition, rotationDuration, AxisConstraint.Y));
            moveSequence.Append(transform.DOMove(arrivePosition, GetDuration(transform.position, arrivePosition)).OnComplete(() => { PoolManager.Instance.ReturnToPool(gameObject); }));
        }

        #region Pooling

        public void OnGetObject(ObjectInstance objectInstance, int poolKey)
        {
            ObjInstance = objectInstance;
            PoolKey = poolKey;
        }

        public void OnReturnObject()
        {
            isDelivering = false;
            gameObject.SetActive(false);
        }

        #endregion Pooling
    }
}