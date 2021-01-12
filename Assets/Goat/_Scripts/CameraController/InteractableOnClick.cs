using Goat.Events;
using Goat.Grid.Interactions;
using Goat.Grid.UI;
using UnityEngine;

public class InteractableOnClick : CollisionDetection
{
    private BaseInteractable currentInteractable, previousInteractable;
    [SerializeField] private InteractableEvent interactableEvent;
    [SerializeField] private GridUIInfo gridUIInfo;

    public override void OnEnter()
    {
        if (latestCollider)
        {
            currentInteractable = latestCollider.GetComponentInParent<BaseInteractable>();

            if (currentInteractable != null && !currentInteractable.UIActivated && (gridUIInfo.CurrentUIElement == UIElement.None || gridUIInfo.CurrentUIElement == UIElement.Interactable))
            {
                interactableEvent.Raise(currentInteractable);
            }
        }
    }

    public override void OnExit()
    {
        if (!previousCollider) return;
        previousInteractable = previousCollider.GetComponentInParent<BaseInteractable>();
        if (previousInteractable)
        {
            previousInteractable.IsClickedOn = false;
            if (previousInteractable.UIActivated)
                interactableEvent.Raise(previousInteractable);
            previousInteractable = null;
            previousCollider = null;
        }
    }
}