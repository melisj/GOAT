using DG.Tweening;
using Goat.Pooling;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Goat.Saving 
{
    public class LoadMenu : MonoBehaviour
    {
        [SerializeField] private GameObject savePrefab;
        [SerializeField] private Transform saveParent;
        [SerializeField] private StartGame gameStarter;

        [SerializeField] private Button loadGameTrigger;
        [SerializeField] private TextMeshProUGUI selectedSaveText;

        [SerializeField] private TextMeshProUGUI infoObjectText;

        private string selectedSave;
        public string SelectedSave => selectedSave;

        private void Awake()
        {
            loadGameTrigger.onClick.AddListener(StartGameWithSave);
        }

        private void OnEnable()
        {
            ShowAllSaves();
        }

        private void OnDisable()
        {
            RemoveAllSaves();
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

        private void RemoveAllSaves()
        {
            foreach (Transform save in saveParent)
            {
                Destroy(save.gameObject);
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
                DataHandler.LevelLoaded += DataHandler_LevelLoaded;
                gameStarter.LoadGame(selectedSave);
            }
            else
            {
                Debug.Log("No save selected");
                ShowInfoAboutSave("No save selected!");
            }
        }

        private void DataHandler_LevelLoaded()
        {
            ShowInfoAboutSave("Save game loaded!");
        }

        private void ShowInfoAboutSave(string message)
        {
            infoObjectText.text = message;
            infoObjectText.DOColor(Color.white, 0);
            infoObjectText.DOColor(Color.clear, 5);
        }
    } 
}
