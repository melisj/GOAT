using Goat.Storage;
using Goat.Selling;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Goat.Storage
{
    public class ResourceGridFiller : MonoBehaviour
    {
        [SerializeField] private ResourceDictionary resData;
        [SerializeField] private GameObject sellingUIObject;
        [SerializeField] private GameObject cellPrefab;
        [SerializeField] private SellingUI sellingUI;
        private Resource currentRes;
        private GameObject cell;

        [ButtonGroup]
        private void FillUIGrid()
        {
            foreach (KeyValuePair<ResourceType, Resource> res in resData.Resources)
            {
                SetupCell(res.Value);
            }
        }

        [ButtonGroup]
        private void DestroyPreviousGrid()
        {
            for (int i = transform.childCount; i > 0; i--)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
        }

        private void SetupCell(Resource resource)
        {
            cell = Instantiate(cellPrefab, transform);
            Button imageButton = cell.GetComponent<Button>();
            imageButton.onClick.AddListener(() => sellingUIObject.SetActive(true));
            imageButton.onClick.AddListener(() => sellingUI.Resource = resource);
            //cell.GetComponent<Button>().onClick.AddListener(delegate { ActivateVerify(); });
            // cell.GetComponent<Button>().onClick.AddListener(delegate { SelectItem(); });
            cell.name = resource.ResourceType.ToString();
            ResourceUI resUI = cell.GetComponent<ResourceUI>();
            Debug.Log(imageButton);
            resUI.SetupUI(resource);
        }

        private void Start()
        {
            if (transform.childCount <= 0)
            {
                FillUIGrid();
            }
        }

        private void ActivateVerify()
        {
            sellingUIObject.SetActive(true);
        }
    }
}