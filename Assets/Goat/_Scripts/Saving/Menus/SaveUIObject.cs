using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Goat.Saving
{
    public class SaveUIObject : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI fileNameText;
        [SerializeField] private Button button;
        private SaveMenu saveMenu;

        private void Awake()
        {
            button.onClick.AddListener(SelectThis);
        }

        public void SetName(SaveMenu saveMenu, string fileName)
        {
            this.saveMenu = saveMenu;
            fileNameText.text = fileName;
        }

        private void SelectThis()
        {
            if (saveMenu.SelectedSave == fileNameText.text)
                saveMenu.StartGameWithSave();
            saveMenu.ChangeSelectedSave(fileNameText.text);
        }
    }
}
