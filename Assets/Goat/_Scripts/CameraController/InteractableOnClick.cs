using Goat.Grid.Interactions;

public class InteractableOnClick : CollisionDetection
{
    private BaseInteractable currentInteractable, previousInteractable;

    public override void OnEnter()
    {
        if (latestCollider)
        {
            currentInteractable = latestCollider.GetComponentInParent<BaseInteractable>();

            if (currentInteractable.IsClickedOn)
            {
                currentInteractable.OpenUIFully();
            }
            previousInteractable = currentInteractable;
        }
    }

    public override void OnExit()
    {
        if (previousInteractable != currentInteractable)
        {
            previousInteractable.CloseUI();
            previousInteractable = null;
            latestCollider = null;
        }
    }
}