using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

namespace Goat.UI
{
    public class CheckForHoverInfo : MonoBehaviour
    {
        [SerializeField] protected GraphicRaycaster graphicRayCaster;
        [SerializeField] private EventSystem eventSystem;
        [SerializeField] private InfoBox infoBox;
        [SerializeField] private float timeTillShow = 2;
        private PointerEventData pointEvtData;
        private OnHoverInfo currentInfo;
        private List<RaycastResult> results = new List<RaycastResult>();

        private float timer;
        private Vector3 mousePos, previousMousePos;

        protected virtual void Update()
        {
            mousePos = Input.mousePosition;

            if (mousePos == previousMousePos)
            {
                timer += Time.unscaledDeltaTime;
                if (timer >= timeTillShow)
                {
                    timer = 0;
                    GetHoverInfo();
                }
            }
            else
            {
                currentInfo = null;
                infoBox.Deactivate();
            }

            previousMousePos = mousePos;
        }

        protected virtual void GetHoverInfo()
        {
            //Set up the new Pointer Event
            pointEvtData = new PointerEventData(eventSystem);
            results.Clear();
            //Set the Pointer Event Position to that of the mouse position
            pointEvtData.position = Input.mousePosition;
            graphicRayCaster.Raycast(pointEvtData, results);
            for (int i = 0; i < results.Count; i++)
            {
                if (!results[i].gameObject.CompareTag("HoverInfo"))
                    continue;
                OnHoverInfo info = results[i].gameObject.GetComponent<OnHoverInfo>();
                if (info != currentInfo)
                {
                    currentInfo = info;
                    ShowInfo();
                    break;
                }
            }
        }

        private void ShowInfo()
        {
            infoBox.Setup(Input.mousePosition, currentInfo);
        }
    }
}