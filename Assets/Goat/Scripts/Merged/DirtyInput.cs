using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grid = Goat.Grid.Grid;
using DG.Tweening;

public class DirtyInput : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private GameObject buyingUI;
    [SerializeField] private GameObject buildingUI;

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ChangeUI(buildingUI);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            //   grid.interactionMode = Goat.Grid.SelectionMode.Edit;
            grid.DestroyMode = true;
            ChangeUI();
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            ChangeUI(buyingUI);
        }
    }

    private void ChangeUI(GameObject newUI = null)
    {
        if (newUI && newUI.activeInHierarchy)
        {
            //Toggles
            newUI.SetActive(false);
            return;
        }

        //Opens the correct one and closes the other
        buildingUI.SetActiveOnCompare(newUI);
        buyingUI.SetActiveOnCompare(newUI);

        if (newUI)
        {
            grid.DestroyMode = false;
            //grid.DestroyPreview();
        }
    }
}

public static class Extensions
{
    public static GameObject SetActiveOnCompare(this GameObject obj, GameObject comparedObj)
    {
        obj.SetActive(obj == comparedObj);
        return obj;
    }

    public static bool NotNull(this Sequence seq)
    {
        return seq?.IsActive() ?? false;
    }
}