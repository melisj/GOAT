using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;
using Goat.Events;
using UnityAtoms.BaseAtoms;

namespace Goat
{
    public enum InputMode
    {
        Select,
        Edit,
        Destroy
    }

    [Flags]
    public enum KeyMode
    {
        None = 0,
        Up = 1,
        Down = 2,
        Pressed = 4,
        All = 7
    }

    public class InputManager : SerializedMonoBehaviour
    {
        [SerializeField] private InputMode inputMode;
        [SerializeField] private InputModeEvent onInputModeChanged;
        [SerializeField] private KeyCodeModeEvent onNewInput;

        [SerializeField] private BoolVariable inputFieldSelected;
        [SerializeField] private InputData data;

        public delegate void OnInput(KeyCode code, KeyMode keyMode, InputMode inputMode);

        public event OnInput OnInputEvent;

        public event EventHandler<InputMode> InputModeChanged;

        private Dictionary<KeyCode, KeyMode> inputKeys => data.InputKeys;

        /// <summary>
        /// Raycast from the position of the mouse to the world
        /// </summary>
        /// <param name="hit"></param>
        /// <returns> Returns whether it hit something </returns>
        public bool DoRaycastFromMouse(out RaycastHit hit, LayerMask mask)
        {
            Vector3 mousePosition = Input.mousePosition + new Vector3(0, 0, Camera.main.nearClipPlane);
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            Vector3 cameraPerspective = mouseWorldPosition - Camera.main.transform.position;

            bool isHitting = Physics.Raycast(mouseWorldPosition, cameraPerspective, out RaycastHit mouseHit, Mathf.Infinity, mask);
            hit = mouseHit;

            if (EventSystem.current.IsPointerOverGameObject())
                return false;
            return isHitting;
        }

        //private void Update()
        //{
        //    if (!inputFieldSelected.Value)
        //    {
        //        CheckKeys();
        //    }
        //}

        private void CheckKeys()
        {
            var enumerator = inputKeys.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyCode currentCode = enumerator.Current.Key;
                KeyMode toCheckMode = enumerator.Current.Value;
                KeyMode currentMode = KeyMode.None;
                KeyCodeMode keyCodeMode = new KeyCodeMode();
                keyCodeMode.Item1 = currentCode;
                keyCodeMode.Item2 = toCheckMode;
                if (toCheckMode.HasFlag(KeyMode.Up))
                {
                    if (Input.GetKeyUp(currentCode))
                    {
                        currentMode |= KeyMode.Up;
                        // OnInputEvent.Invoke(currentCode, currentMode, InputMode);
                        keyCodeMode.Item2 = currentMode;
                        onNewInput.Raise(keyCodeMode);
                        currentMode &= ~KeyMode.Up;
                    }
                }
                if (toCheckMode.HasFlag(KeyMode.Down))
                {
                    if (Input.GetKeyDown(currentCode))
                    {
                        currentMode |= KeyMode.Down;
                        keyCodeMode.Item2 = currentMode;

                        // OnInputEvent.Invoke(currentCode, currentMode, InputMode);
                        onNewInput.Raise(keyCodeMode);

                        currentMode &= ~KeyMode.Down;
                    }
                }
                if (toCheckMode.HasFlag(KeyMode.Pressed))
                {
                    if (Input.GetKey(currentCode))
                    {
                        currentMode |= KeyMode.Pressed;
                        keyCodeMode.Item2 = currentMode;

                        //   OnInputEvent.Invoke(currentCode, currentMode, InputMode);
                        onNewInput.Raise(keyCodeMode);

                        currentMode &= ~KeyMode.Pressed;
                    }
                }
            }
        }
    }
}