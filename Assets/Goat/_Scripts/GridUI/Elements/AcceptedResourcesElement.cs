using Goat.ScriptableObjects;
using Goat.Storage;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Goat.Grid.Interactions.UI
{
    public class AcceptedResourcesElement : MonoBehaviour
    {
        [SerializeField] private ResourceArray resArray;
        [SerializeField] private GameObject cellPrefab;
        [SerializeField] private RectTransform gridParent;
        [SerializeField, ReadOnly] private UICell[] cells;
        [SerializeField] private InteractablesInfo info;
        private bool createdCells;

        public void CreateCells()
        {
            if (createdCells) return;

            createdCells = true;
            cells = new UICell[resArray.Resources.Length];
            for (int i = 0; i < resArray.Resources.Length; i++)
            {
                GameObject cell = Instantiate(cellPrefab, gridParent);
                Resource resource = resArray.Resources[i];
                UICell uiCell = cell.GetComponent<UICell>();
                uiCell.Setup(resource);
                uiCell.OnClick(() => ChangeMainResource(resource, info));
                cells[i] = uiCell;
            }
        }

        public void SetActiveCells(StorageInteractable storage)
        {
            //if (info.CurrentSelected && info.CurrentSelected is StorageInteractable storage)
            //{
            for (int i = 0; i < cells.Length; i++)
            {
                if (storage.MainResource != null && storage.MainResource.Image == cells[i].Icon.sprite)
                    cells[i].OnSelect();
                else
                    cells[i].OnDeselect();
            }
            // }
        }

        private void ChangeMainResource(Resource res, InteractablesInfo info)
        {
            if (info.CurrentSelected && info.CurrentSelected is StorageInteractable storage)
            {
                storage.MainResource = res;
            }
        }
    }
}