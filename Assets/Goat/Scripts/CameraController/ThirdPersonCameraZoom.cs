﻿using Cinemachine;
using UnityEngine;

namespace Goat.CameraControls
{
    public class ThirdPersonCameraZoom : CameraZoom
    {
        private Cinemachine3rdPersonFollow follow;
        private float latestZoom = -999;

        private void OnEnable()
        {
            if (follow && latestZoom > -999)
            {
                follow.CameraDistance = latestZoom;
            }
        }

        private void OnDisable()
        {
            if (follow && latestZoom > -999)
            {
                follow.CameraDistance = latestZoom;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            if (baseComp is Cinemachine3rdPersonFollow)
            {
                follow = (Cinemachine3rdPersonFollow)baseComp;
            }
            latestZoom = minMaxZoom.y / 2;
            follow.CameraDistance = latestZoom;
        }

        private void Update()
        {
            latestZoom += Zoom(follow.CameraDistance);
            follow.CameraDistance = latestZoom;
        }
    }
}