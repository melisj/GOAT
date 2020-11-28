using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;

namespace Goat
{
    public enum InputMode
    {
        Select,
        Edit,
        Destroy
    }

    public class InputManager : SerializedMonoBehaviour
    {
        private static InputManager instance;

        public static InputManager Instance
        {
            get
            {
                if (!instance)
                {
                    instance = FindObjectOfType<InputManager>();
                }
                return instance;
            }
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

        public InputMode InputMode
        {
            get => inputMode;
            set
            {
                if (value != inputMode)
                    InputModeChanged.Invoke(this, value);
                inputMode = value;
            }
        }

        [SerializeField] private InputMode inputMode;

        public delegate void OnInput(KeyCode code, KeyMode keyMode, InputMode inputMode);

        public event OnInput OnInputEvent;

        public event EventHandler<InputMode> InputModeChanged;

        [SerializeField] private InputData data;

        private Dictionary<KeyCode, KeyMode> inputKeys => data.InputKeys;

        public bool InputFieldSelected { get; set; }

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

        private void Update()
        {
            CheckKeys();
        }

        private void CheckKeys()
        {
            var enumerator = inputKeys.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyCode currentCode = enumerator.Current.Key;
                KeyMode toCheckMode = enumerator.Current.Value;
                KeyMode currentMode = KeyMode.None;
                if (toCheckMode.HasFlag(KeyMode.Up))
                {
                    if (Input.GetKeyUp(currentCode))
                    {
                        currentMode |= KeyMode.Up;
                        OnInputEvent.Invoke(currentCode, currentMode, InputMode);
                        currentMode &= ~KeyMode.Up;
                    }
                }
                if (toCheckMode.HasFlag(KeyMode.Down))
                {
                    if (Input.GetKeyDown(currentCode))
                    {
                        currentMode |= KeyMode.Down;

                        OnInputEvent.Invoke(currentCode, currentMode, InputMode);
                        currentMode &= ~KeyMode.Down;
                    }
                }
                if (toCheckMode.HasFlag(KeyMode.Pressed))
                {
                    if (Input.GetKey(currentCode))
                    {
                        currentMode |= KeyMode.Pressed;

                        OnInputEvent.Invoke(currentCode, currentMode, InputMode);
                        currentMode &= ~KeyMode.Pressed;
                    }
                }
            }
        }
    }
}