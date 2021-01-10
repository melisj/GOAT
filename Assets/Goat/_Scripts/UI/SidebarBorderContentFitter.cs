using UnityEngine;

public class SidebarBorderContentFitter : MonoBehaviour
{
    [SerializeField] private AnimateSideBarOnClick animateSideBarOnClick;
    [SerializeField] private RectTransform buttonParent;
    [SerializeField] private RectTransform[] middleBorders;
    private int amountActivated = 0;
    private float size => 48 * (((amountActivated - 1) * 2) + 1);

    public void FitContent()
    {
        amountActivated = 0;
        for (int i = 0; i < animateSideBarOnClick.ButtonRects.Length; i++)
        {
            RectTransform button = animateSideBarOnClick.ButtonRects[i];
            if (button.gameObject.activeSelf)
                amountActivated++;
        }

        for (int i = 0; i < middleBorders.Length; i++)
        {
            middleBorders[i].sizeDelta = new Vector2(size, middleBorders[i].sizeDelta.y);
        }
        buttonParent.sizeDelta = new Vector2(size + (96), buttonParent.sizeDelta.y);
    }
}