using Goat.Grid.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goat
{
    public class ChangeCycleListener : MonoBehaviour
    {
        [SerializeField] private DayNightCycle dayNightCycle;
        [SerializeField] private SpawnNPC npcSpawner;
        [SerializeField] private GameObject[] UItoHide;
        [SerializeField] private ChangeMode changeMode;

        private void Awake()
        {
            dayNightCycle.OnChangeCycle += DayNightCycle_OnChangeCycle;
        }

        private void HideUI(bool activate)
        {
            for (int i = 0; i < UItoHide.Length; i++)
            {
                UItoHide[i].SetActive(!activate);
            }
            GridUIManager.Instance.HideUI();
        }

        private void DisableModeChanging(bool enable)
        {
            changeMode.AllowedToChange = !enable;
        }

        private void SwitchMode(bool toSelectMode)
        {
            InputManager.Instance.InputMode = toSelectMode ? InputMode.Select : InputMode.Select;
        }

        private void SpawnNpc(bool spawn)
        {
            if (spawn)
            {
                npcSpawner.SpawnRepeat(1, 5);
            }
            else
            {
                npcSpawner.KillSequence();
            }
        }

        private void DayNightCycle_OnChangeCycle(object sender, bool isDay)
        {
            HideUI(isDay);
            DisableModeChanging(isDay);
            SwitchMode(isDay);
            SpawnNpc(isDay);
        }
    }
}