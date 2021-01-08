﻿using Goat.Pooling;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Goat.Saving 
{
    public class SaveMenu : MonoBehaviour
    {
        [SerializeField] private GameObject savePrefab;
        [SerializeField] private Transform saveParent;
        [SerializeField] private StartGame gameStarter;

        [SerializeField] private Button loadGameTrigger;
        [SerializeField] private TextMeshProUGUI selectedSaveText;

        private string selectedSave;
        public string SelectedSave => selectedSave;

        private void Awake()
        {
            loadGameTrigger.onClick.AddListener(StartGameWithSave);
            ShowAllSaves();
        }

        private void ShowAllSaves()
        {
            string[] saves = DataHandler.GetAllSaves();

            foreach(string save in saves)
            {
                int firstIndex = save.LastIndexOf("/") + 1;
                int secondIndex = save.LastIndexOf(".");

                string saveName = save.Substring(firstIndex, secondIndex - firstIndex);

                GameObject saveObject = Instantiate(savePrefab, Vector3.zero, Quaternion.identity, saveParent); //PoolManager.Instance.GetFromPool(savePrefab, Vector3.zero, Quaternion.identity, transform);
                saveObject.GetComponent<SaveUIObject>().SetName(this, saveName);
            }
        }

        public void ChangeSelectedSave(string selectedSave)
        {
            this.selectedSave = selectedSave;
            selectedSaveText.text = selectedSave;
        }

        public void StartGameWithSave()
        {
            if (selectedSave != null && selectedSave != "")
            {
                gameStarter.LoadGame(selectedSave);
            }
            else
            {
                Debug.Log("No save selected");
            }
        }
    } 
}
