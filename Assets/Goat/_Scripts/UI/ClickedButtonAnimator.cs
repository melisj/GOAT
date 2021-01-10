using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using Goat.Events;
using Goat.Grid.UI;

namespace Goat.UI
{
    public class ClickedButtonAnimator : EventListenerVoid
    {
        //Initial sizes
        private const int BORDER_SIZE = 35;
        private const int BORDERCONTROL_SIZE = 100;
        [SerializeField] private GridUIInfo uiInfo;
        [SerializeField] private RectTransform clickedButton;
        [SerializeField] private RectTransform barButton;
        [SerializeField] private Image barButtonImage;
        [SerializeField] private RectTransform borders;
        [SerializeField] private RectTransform borderControl;
        [Title("Audio")]
        [SerializeField] private AudioCue openWindowAudio, closeWindoAudio;

        [Title("Animation Settings")]
        [SerializeField, Range(2, 5)] private int closingDurationMultiplier;
        [SerializeField] private float moveDuration;
        [SerializeField] private float bordersScaleDuration;
        [SerializeField] private float borderControlScaleDuration;
        [SerializeField, FoldoutGroup("Bar button scale punch")] private Vector3 scaleIncrease;
        [SerializeField, FoldoutGroup("Bar button scale punch"), Range(0f, 0.3f)] private float punchScaleDuration;
        [SerializeField, FoldoutGroup("Bar button scale punch"), Range(10, 30)] private int vibrato;
        [SerializeField, FoldoutGroup("Bar button scale punch"), Range(0f, 1f)] private float elasticity;

        private Sequence openWindow, closeWindow;
        private RectTransform currentWindow;
        private RectTransform currentClickedButton;
        private RectTransform currentBorderControl;
        private bool openedWindow;
        private Action OnOpen, OnClose;

        private void Awake()
        {
            openedWindow = false;
        }

        public override void OnEventRaised(UnityAtoms.Void value)
        {
            if (openedWindow)
            {
                AnimateCloseWindow();
            }
        }

        public void Play(RectTransform clickedButton, Sprite sprite, RectTransform targetWindow, RectTransform targetBorderControl, Action OnOpen, Action OnClose)
        {
            if (openedWindow)
            {
                AnimateCloseWindow(clickedButton, sprite, targetWindow, targetBorderControl, OnOpen, OnClose);
                return;
            }

            OpenWindow(clickedButton, sprite, targetWindow, targetBorderControl, OnOpen, OnClose);
        }

        private void OpenWindow(RectTransform clickedButton, Sprite sprite, RectTransform targetWindow, RectTransform targetBorderControl, Action OnOpen, Action OnClose)
        {
            openedWindow = true;

            this.clickedButton.position = clickedButton.position;
            this.OnOpen = OnOpen;
            this.OnClose = OnClose;

            barButtonImage.sprite = sprite;
            Reset();
            currentWindow = targetWindow;
            currentClickedButton = clickedButton;
            currentBorderControl = targetBorderControl;
            AnimateOpenWindow(currentWindow.anchoredPosition, OnOpen);
        }

        /// <summary>
        /// (1) Move button to window
        /// (2) Punch scale button
        /// (3) Show border
        /// (4) Scale border to window W&H, stop at size 100
        /// (5) If size > 100, hide border, show borderControl, scale to window W&H, update size
        /// (6) Show targetWindow
        /// </summary>
        /// <param name="pos"></param>
        private void AnimateOpenWindow(Vector2 pos, Action OnComplete)
        {
            bool sizeExceeds = currentBorderControl.sizeDelta.x > 100 || currentBorderControl.sizeDelta.y > 100;
            Vector2 bordersMaxSize = new Vector2(Mathf.Min(100, currentBorderControl.sizeDelta.x), Mathf.Min(100, currentBorderControl.sizeDelta.y));
            if (closeWindow.NotNull())
                closeWindow.Complete();
            if (openWindow.NotNull())
                openWindow.Complete();

            openWindow = DOTween.Sequence();
            openWindow.OnComplete(() => OnComplete());
            openWindow.Append(clickedButton.DOAnchorPos(pos, moveDuration));
            openWindow.Append(clickedButton.DOPunchAnchorPos(scaleIncrease, punchScaleDuration, vibrato, elasticity));
            openWindow.AppendCallback(() =>
            {
                barButton.gameObject.SetActive(false);
                borders.gameObject.SetActive(true);
            });

            openWindow.Append(borders.DOSizeDelta(bordersMaxSize, bordersScaleDuration));
            openWindow.AppendCallback(openWindowAudio.PlayAudioCue);
            if (sizeExceeds)
            {
                openWindow.AppendCallback(() =>
                {
                    borders.gameObject.SetActive(false);
                    borderControl.gameObject.SetActive(true);
                });
                SetMiddleSize setMiddleSize = borderControl.GetComponent<SetMiddleSize>();
                openWindow.Append(borderControl.DOSizeDelta(currentBorderControl.sizeDelta, borderControlScaleDuration).OnUpdate(() => setMiddleSize.ChangeSize()));
            }

            openWindow.AppendCallback(() =>
            {
                borders.gameObject.SetActive(false);
                borderControl.gameObject.SetActive(false);
                //currentWindow.gameObject.SetActive(true);
                uiInfo.CurrentUIElement = currentWindow.GetComponent<BasicGridUIElement>().Type;
                //TODO: Change to ShowUI from GridInfo
            });
        }

        /// <summary>
        /// (1) Hide targetWindow
        /// (2) If size > 100, hide border, show borderControl, scale to window W&H, update size
        /// (3) Scale border to window W&H, stop at size 100
        /// (4) Show border
        /// (5) Punch scale button
        /// (6) Move button to window
        /// </summary>
        private void AnimateCloseWindow(RectTransform clickedButton, Sprite sprite, RectTransform targetWindow, RectTransform targetBorderControl, Action OnOpen, Action OnClose)
        {
            AnimateCloseWindow();
            if (currentWindow != targetWindow)
                closeWindow.AppendCallback(() => OpenWindow(clickedButton, sprite, targetWindow, targetBorderControl, OnOpen, OnClose));
        }

        private void AnimateCloseWindow()
        {
            openedWindow = false;
            bool sizeExceeds = currentWindow.sizeDelta.x > 100 || currentWindow.sizeDelta.y > 100;
            Vector2 bordersMaxSize = new Vector2(Mathf.Min(100, currentWindow.sizeDelta.x), Mathf.Min(100, currentWindow.sizeDelta.y));
            if (openWindow.NotNull())
                openWindow.Complete();
            if (closeWindow.NotNull())
                closeWindow.Complete();

            closeWindow = DOTween.Sequence();
            closeWindow.OnStart(() => this.OnClose());
            closeWindow.AppendCallback(() =>
            {
                borders.gameObject.SetActive(!sizeExceeds);
                borderControl.gameObject.SetActive(sizeExceeds);
            });

            if (sizeExceeds)
            {
                closeWindow.AppendCallback(() =>
                {
                    borders.gameObject.SetActive(false);
                    borderControl.gameObject.SetActive(true);
                });
                SetMiddleSize setMiddleSize = borderControl.GetComponent<SetMiddleSize>();
                closeWindow.Append(borderControl.DOSizeDelta(new Vector2(BORDERCONTROL_SIZE, BORDERCONTROL_SIZE), borderControlScaleDuration / closingDurationMultiplier).OnUpdate(() => setMiddleSize.ChangeSize()));
            }

            closeWindow.Append(borders.DOSizeDelta(new Vector2(BORDER_SIZE, BORDER_SIZE), bordersScaleDuration / closingDurationMultiplier));
            closeWindow.AppendCallback(closeWindoAudio.PlayAudioCue);
            closeWindow.AppendCallback(() =>
            {
                barButton.gameObject.SetActive(true);
                borders.gameObject.SetActive(false);
            });

            closeWindow.Append(transform.DOMove(currentClickedButton.position, moveDuration / closingDurationMultiplier));
            closeWindow.Append(barButton.DOPunchPosition(scaleIncrease, punchScaleDuration / closingDurationMultiplier, vibrato, elasticity));
            closeWindow.AppendCallback(() =>
            {
                //TODO: Change to HideUI from GridInfo
                barButton.gameObject.SetActive(false);
                uiInfo.CurrentUIElement = UIElement.None;
                //currentWindow.gameObject.SetActive(false);
            });
        }

        private void Reset()
        {
            borders.sizeDelta = new Vector2(BORDER_SIZE, BORDER_SIZE);
            borderControl.sizeDelta = new Vector2(BORDERCONTROL_SIZE, BORDERCONTROL_SIZE);

            barButton.gameObject.SetActive(true);
            borders.gameObject.SetActive(false);
            borderControl.gameObject.SetActive(false);
        }
    }
}