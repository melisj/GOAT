using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using Goat.Events;
using Goat.Grid.Interactions;
using Goat.Grid.UI;
using UnityAtoms.BaseAtoms;

namespace Goat.UI
{
    public class AnimateOpenInteractable : EventListenerInteractable
    {
        [Title("Settings")]
        [SerializeField, Range(2, 5)] private int closingMultiplier;
        [SerializeField] private float scalingDuration;
        [SerializeField] private VoidEvent closeButtonEvent;
        [Title("Refs")]
        [SerializeField] private GridUIInfo uiInfo;
        [SerializeField] private AnimateOpenWindow windowOpener;
        [SerializeField] private RectTransform crateSeperator;
        [SerializeField] private RectTransform crateInventory;
        [SerializeField] private RectTransform acceptedItems;
        [SerializeField] private RectTransform fillingUISeperator;
        [SerializeField] private RectTransform fillingUI;
        [SerializeField] private RectTransform stockButton;
        [SerializeField] private RectTransform header;

        private Sequence openSequence, closeSequence;
        private BaseInteractable prevInteractable;

        public override void OnEventRaised(BaseInteractable value)
        {
            if (value is CheckoutInteractable)
            {
                if (prevInteractable)
                    CloseInteractable(prevInteractable);
                return;
            }
            if (uiInfo.CurrentUIElement != UIElement.Interactable)
            {
                closeButtonEvent.Raise();
                windowOpener.OpenWindow(() => OpenInteractable(value), () => CloseInteractable(value));
                return;
            }

            if (!value.UIActivated)
                OpenInteractable(value);
            else
                CloseInteractable(value);
        }

        /// <summary>
        /// (1): Scale crate seperator
        /// (2): Scale crate inventory
        /// (2): Join, Scale accepted items
        /// (3): Scale filling UI seperator
        /// (4): Scale filling UI
        /// (5): Scale stock button
        /// (5): Join,Scale header
        /// </summary>
        private void OpenInteractable(BaseInteractable interactable)
        {
            interactable.UIActivated = true;
            prevInteractable = interactable;

            if (closeSequence.NotNull())
                closeSequence.Complete();
            if (openSequence.NotNull())
                openSequence.Complete();

            openSequence = DOTween.Sequence();
            openSequence.OnStart(() => interactable.OpenUIFully());
            openSequence.Append(crateSeperator.DOScale(Vector3.one, scalingDuration));
            openSequence.Append(crateInventory.DOScale(Vector3.one, scalingDuration));
            openSequence.Join(acceptedItems.DOScale(Vector3.one, scalingDuration));
            openSequence.Append(fillingUISeperator.DOScale(Vector3.one, scalingDuration));
            openSequence.Append(fillingUI.DOScale(Vector3.one, scalingDuration));
            openSequence.Append(stockButton.DOScale(Vector3.one, scalingDuration));
            openSequence.Join(header.DOScale(Vector3.one, scalingDuration));
        }

        /// <summary>
        /// (1): Scale crate seperator
        /// (2): Scale crate inventory
        /// (2): Join, Scale accepted items
        /// (3): Scale filling UI seperator
        /// (4): Scale filling UI
        /// (5): Scale stock button
        /// (5): Join,Scale header
        /// </summary>
        private void CloseInteractable(BaseInteractable interactable)
        {
            interactable.UIActivated = false;
            prevInteractable = interactable;

            if (openSequence.NotNull())
                openSequence.Complete();
            if (closeSequence.NotNull())
                closeSequence.Complete();

            Vector3 zeroRight = new Vector3(0, 1, 1);
            Vector3 zeroUp = new Vector3(1, 0, 1);

            closeSequence = DOTween.Sequence();
            closeSequence.OnStart(() => interactable.CloseUI());
            closeSequence.Append(stockButton.DOScale(zeroUp, scalingDuration / closingMultiplier));
            closeSequence.Join(header.DOScale(zeroUp, scalingDuration / closingMultiplier));
            closeSequence.Append(fillingUISeperator.DOScale(zeroRight, scalingDuration / closingMultiplier));
            closeSequence.Append(fillingUI.DOScale(zeroRight, scalingDuration / closingMultiplier));
            closeSequence.Append(acceptedItems.DOScale(zeroRight, scalingDuration / closingMultiplier));
            closeSequence.Join(crateInventory.DOScale(zeroRight, scalingDuration / closingMultiplier));
            closeSequence.Append(crateSeperator.DOScale(zeroRight, scalingDuration / closingMultiplier));
        }
    }
}