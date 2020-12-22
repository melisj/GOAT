using Goat.Grid.Interactions;

public class InteractableOnClick : CollisionDetection
{
    private BaseInteractable currentInteractable, previousInteractable;

    public override void OnEnter()
    {
        if (latestCollider)
        {
            currentInteractable = latestCollider.GetComponentInParent<BaseInteractable>();

            if (currentInteractable.IsClickedOn && !currentInteractable.UIOpen)
            {
                currentInteractable.OpenUIFully();
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
            previousInteractable.CloseUI();
            previousInteractable = null;
            previousCollider = null;
        }
    }
}