using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using Goat.Events;
using Goat.Grid.Interactions;
using Goat.Grid.UI;

namespace Goat.UI
{
    public class AnimateOpenCheckout : EventListenerInteractable
    {
        [Title("Settings")]
        [SerializeField, Range(2, 5)] private int closingMultiplier;
        [SerializeField] private float scalingDuration;
        [Title("Refs")]
        [SerializeField] private GridUIInfo uiInfo;
        [SerializeField] private AnimateOpenWindow windowOpener;
        [SerializeField] private RectTransform npcInventory;
        [SerializeField] private RectTransform sellButton;
        [SerializeField] private RectTransform header;
        private BaseInteractable prevInteractable;
        private Sequence openCheckout, closeCheckout;

        public override void OnEventRaised(BaseInteractable value)
        {
            if (!(value is CheckoutInteractable))
            {
                if (prevInteractable is CheckoutInteractable && prevInteractable.UIActivated)
                    CloseCheckout(prevInteractable);
                return;
            }

            if (uiInfo.CurrentUIElement != UIElement.Interactable)
            {
                windowOpener.OpenWindow(() => OpenCheckout(value), () => CloseCheckout(value));
                return;
            }

            if (!value.UIActivated)
                OpenCheckout(value);
            else
                CloseCheckout(value);
        }

        private void OpenCheckout(BaseInteractable interactable)
        {
            interactable.UIActivated = true;
            prevInteractable = interactable;

            if (closeCheckout.NotNull())
                closeCheckout.Complete();
            if (openCheckout.NotNull())
                openCheckout.Complete();

            openCheckout = DOTween.Sequence();
            openCheckout.OnComplete(() => interactable.OpenUI());
            openCheckout.Append(npcInventory.DOScale(Vector3.one, scalingDuration));
            openCheckout.Append(sellButton.DOScale(Vector3.one, scalingDuration));
            openCheckout.Append(header.DOScale(Vector3.one, scalingDuration));
        }

        private void CloseCheckout(BaseInteractable interactable)
        {
            interactable.UIActivated = false;
            prevInteractable = interactable;

            if (openCheckout.NotNull())
                openCheckout.Complete();
            if (closeCheckout.NotNull())
                closeCheckout.Complete();

            Vector3 zeroRight = new Vector3(0, 1, 1);
            Vector3 zeroUp = new Vector3(1, 0, 1);

            closeCheckout = DOTween.Sequence();
            closeCheckout.OnComplete(() => interactable.CloseUI());

            closeCheckout.Append(header.DOScale(zeroUp, scalingDuration / closingMultiplier));
            closeCheckout.Append(sellButton.DOScale(zeroUp, scalingDuration / closingMultiplier));
            closeCheckout.Append(npcInventory.DOScale(zeroRight, scalingDuration / closingMultiplier));
        }
    }
}