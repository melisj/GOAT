using Goat.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace Goat.Player
{
    public class PlayerPointToClick : EventListenerKeyCodeModeEvent
    {
        [SerializeField] private NavMeshAgent player;
        [SerializeField] private Camera mainCam;
        [SerializeField] private InputModeVariable currentMode;
        [SerializeField] private LayerMask gridLayer;
        private bool inSelectMode;
        private RaycastHit m_HitInfo = new RaycastHit();

        protected override void InitOnEnable()
        {
            base.InitOnEnable();
            player.enabled = true;
        }

        public override void OnEventRaised(KeyCodeMode value)
        {
            KeyCode code = KeyCode.None;
            KeyMode mode = KeyMode.None;

            value.Deconstruct(out code, out mode);
            OnInput(code, mode);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && currentMode.InputMode == InputMode.Select)
            {
                MoveTo(Input.mousePosition);
            }
        }

        private void OnInput(KeyCode code, KeyMode keyMode)
        {
            if (code == KeyCode.Mouse0 && keyMode.HasFlag(KeyMode.Down))
            {
                if (currentMode.InputMode == InputMode.Select)
                {
                    MoveTo(Input.mousePosition);
                }
            }
        }

        private void MoveTo(Vector3 mousePos)
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            var ray = mainCam.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray.origin, ray.direction, out m_HitInfo, 1000, gridLayer))
                player.destination = m_HitInfo.point;
        }

        protected override void InitOnDisable()
        {
            base.InitOnDisable();
            player.enabled = false;
        }
    }
}