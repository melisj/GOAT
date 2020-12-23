using UnityEngine;
using Goat.Events;

namespace Goat.CameraControls
{
    public class ResetCameraPositionToPlayer : EventListenerKeyCodeModeEvent
    {
        [SerializeField] private KeyCode key;
        [SerializeField] private GameObject player;
        [SerializeField] private GameObject clickToMoveObject;

        public override void OnEventRaised(KeyCodeMode value)
        {
            KeyCode code = KeyCode.None;
            KeyMode mode = KeyMode.None;

            value.Deconstruct(out code, out mode);
            OnInput(code, mode);
        }

        private void Update()
        {
            if (Input.GetKeyDown(key))
            {
                Vector3 newPos = player.transform.position;
                clickToMoveObject.transform.position = new Vector3(newPos.x, clickToMoveObject.transform.position.y, newPos.z);
            }
        }

        private void OnInput(KeyCode code, KeyMode mode)
        {
            if (code == key && mode.HasFlag(KeyMode.Down))
            {
                Vector3 newPos = player.transform.position;
                clickToMoveObject.transform.position = new Vector3(newPos.x, clickToMoveObject.transform.position.y, newPos.z);
            }
        }
    }
}