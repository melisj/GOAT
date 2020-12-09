using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace Goat.Player
{
    public class PlayerPointToClick : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent player;
        [SerializeField] private Camera mainCam;
        private bool inSelectMode;
        private RaycastHit m_HitInfo = new RaycastHit();

        private void OnEnable()
        {
            InputManager.Instance.OnInputEvent += Instance_OnInputEvent;
            InputManager.Instance.InputModeChanged += Instance_InputModeChanged;
            player.enabled = true;
        }

        private void Instance_InputModeChanged(object sender, InputMode e)
        {
            inSelectMode = e == InputMode.Select;
        }

        private void Instance_OnInputEvent(KeyCode code, InputManager.KeyMode keyMode, InputMode inputMode)
        {
            if (code == KeyCode.Mouse0 && keyMode == InputManager.KeyMode.Down)
            {
                if (inputMode == InputMode.Select)
                {
                    MoveTo(Input.mousePosition);
                }
            }
        }

        private void MoveTo(Vector3 mousePos)
        {
            //Vector3 origin = Vector3.zero;
            //origin = mainCam.ScreenToViewportPoint(mousePos);
            //Vector3 screenPos = mainCam.ScreenToViewportPoint(mousePos) - origin;

            //player.SetDestination(mousePos);
            if (EventSystem.current.IsPointerOverGameObject()) return;
            var ray = mainCam.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray.origin, ray.direction, out m_HitInfo))
                player.destination = m_HitInfo.point;
        }

        private void OnDisable()
        {
            player.enabled = false;

            InputManager.Instance.OnInputEvent -= Instance_OnInputEvent;
            InputManager.Instance.InputModeChanged -= Instance_InputModeChanged;
        }
    }
}