using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Goat.Saving
{
    public class SaveMenu : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI fileNameText;
        [SerializeField] private Button saveButton;

        [SerializeField] private TextMeshProUGUI infoObjectText;

        private char[] charsToTrim = { ' ', '.' };

        private void Awake()
        {
            saveButton.onClick.AddListener(TrySaveGame);
        }

        public void TrySaveGame()
        {

            string fileName = fileNameText.text.Trim(charsToTrim);
            if (fileName.Length > 3)
            {
                SaveGame(fileName);
                ShowInfoAboutSave("Saved successfully!");
            }
            else
            {
                ShowInfoAboutSave("Save name should be at least three characters long!");
                Debug.LogWarning("File could not be saved, save requires a valid name!");
            }
        }

        private void SaveGame(string saveFile)
        {
            DataHandler dataHandler = FindObjectOfType<DataHandler>();
            dataHandler.SaveGame(saveFile);
        }

        private void ShowInfoAboutSave(string message)
        {
            infoObjectText.text = message;
            Sequence sequence = DOTween.Sequence();
            sequence.SetUpdate(true);
            sequence.Append(infoObjectText.DOColor(Color.white, 0));
            sequence.Append(infoObjectText.DOColor(Color.clear, 5));
        }
    }
}