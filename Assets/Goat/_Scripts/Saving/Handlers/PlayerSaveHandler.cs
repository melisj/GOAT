﻿using Goat.Saving;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Goat.Player
{
    public class PlayerSaveHandler : SaveHandler
    {
        public PlayerInventory inventory;
        public GridObjectsList objectList;
        public NavMeshAgent agent;

        public void Awake()
        {
            data = new PlayerSaveData();
        }
    }

    [Serializable]
    public class PlayerSaveData : DataContainer, ISaveable
    {
        // Saved data
        public string inventoryString;
        public Vector3 position;

        public override IEnumerator Load(SaveHandler handler)
        {
            PlayerSaveHandler playerHandler = (PlayerSaveHandler)handler;

            playerHandler.inventory.Inventory.Load(inventoryString, playerHandler.objectList);

            playerHandler.transform.position = position;

            try { 
                playerHandler.agent.isStopped = true;
                playerHandler.agent.velocity = Vector3.zero;
                playerHandler.agent.ResetPath();
            }
            catch { }

            DoneLoading(handler, DataHandler.ContainerExitCode.Success);
            yield break;
        }

        public override void Save(SaveHandler handler)
        {
            PlayerSaveHandler playerHandler = (PlayerSaveHandler)handler;

            inventoryString = playerHandler.inventory.Inventory.Save();

            position = playerHandler.transform.position;
        }
    }
}