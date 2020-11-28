using Goat.Selling;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Goat.Storage
{
    public class ResourceGridFiller : MonoBehaviour
    {
        [SerializeField] private ResourceDictionary resData;
        [SerializeField] private GameObject sellingUIObject;
        [SerializeField] private GameObject cellPrefab;

        [Serializable] private class SelectedResourceChanged : UnityEvent<Resource> { }

        [SerializeField] private SelectedResourceChanged selectedResourceChangeEvt;

        private Resource currentRes;
        private GameObject cell;

        [ButtonGroup]
        private void FillUIGrid()
        {
            Resource[] resources = Resources.LoadAll<Resource>("Resource");
            for (int i = 0; i < resources.Length; i++)
            {
                SetupCell(resources[i], i);
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

        private void SetupCell(Resource resource, int index)
        {
            cell = Instantiate(cellPrefab, transform);
            ResourceUI resUI = cell.GetComponent<ResourceUI>();
            resUI.ImageButton.onClick.AddListener(() => sellingUIObject.SetActive(true));
            resUI.ImageButton.onClick.AddListener(() => selectedResourceChangeEvt?.Invoke(resource));
            //cell.GetComponent<Button>().onClick.AddListener(delegate { ActivateVerify(); });
            // cell.GetComponent<Button>().onClick.AddListener(delegate { SelectItem(); });
            cell.name = resource.ResourceType.ToString();
            resUI.SetupUI(resource, index);
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