using UnityEngine;
using Goat.Grid.UI;

namespace Goat.UI
{
    public class CheckForHoverInfoOnWindows : CheckForHoverInfo
    {
        [SerializeField] private GeneralUIManager uiManager;

        protected override void Update()
        {
            if (uiManager.CurrentUIOpen == null) return;
            base.Update();
        }

        protected override void GetHoverInfo()
        {
            graphicRayCaster = uiManager.CurrentUIOpen.GraphicRaycaster;
            if (graphicRayCaster)
            {
                base.GetHoverInfo();
            }
        }
    }
}