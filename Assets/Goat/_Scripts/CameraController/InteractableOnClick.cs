using Goat.Events;
using Goat.Grid.Interactions;
using UnityEngine;

public class InteractableOnClick : CollisionDetection
{
    private BaseInteractable currentInteractable, previousInteractable;
    [SerializeField] private InteractableEvent interactableEvent;

    public override void OnEnter()
    {
        if (latestCollider)
        {
            currentInteractable = latestCollider.GetComponentInParent<BaseInteractable>();

            if (currentInteractable != null && !currentInteractable.UIActivated)
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