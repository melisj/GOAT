﻿using Goat.Grid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Goat.AI
{
    public class NavManager : MonoBehaviour
    {
        [SerializeField] private NavMeshSurface surfaceAI;
        [SerializeField] private NavMeshSurface surfacePlayer;

        // Start is called before the first frame update
        private void Awake()
        {
            InputManager.Instance.OnInputEvent += Instance_OnInputEvent;
            InputManager.Instance.InputModeChanged += Instance_InputModeChanged;
            GridDataHandler.LevelLoaded += GridDataHandler_LevelLoaded;
        }

        private void GridDataHandler_LevelLoaded()
        {
            RebakeMesh();
        }

        private void Instance_InputModeChanged(object sender, InputMode e)
        {
            if (e != InputMode.Edit)
            {
                surfacePlayer?.BuildNavMesh();
                surfaceAI?.BuildNavMesh();
            }
        }

        private void Instance_OnInputEvent(KeyCode code, InputManager.KeyMode keyMode, InputMode inputMode)
        {
            if (code == KeyCode.N && keyMode == InputManager.KeyMode.Down)
            {
                surfaceAI.BuildNavMesh();
                surfacePlayer.BuildNavMesh();
            }
        }

        private void RebakeMesh()
        {
            surfaceAI.BuildNavMesh();
            surfacePlayer.BuildNavMesh();
        }
    }
}