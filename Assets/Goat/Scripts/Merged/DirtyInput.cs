using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grid = Goat.Grid.Grid;
using DG.Tweening;
using System;

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

    public static bool Swap<T>(this T[] objectArray, int x, int y)
    {
        // check for out of range
        if (objectArray.Length <= y || objectArray.Length <= x) return false;

        // swap index x and y
        T buffer = objectArray[x];
        objectArray[x] = objectArray[y];
        objectArray[y] = buffer;

        return true;
    }

    public static bool NotNull(this Sequence seq)
    {
        return seq?.IsActive() ?? false;
    }

    public static void Rotate<T>(this T[] array, int count)
    {
        if (array == null || array.Length < 2) return;
        count %= array.Length;
        if (count == 0) return;
        int left = count < 0 ? -count : array.Length + count;
        int right = count > 0 ? count : array.Length - count;
        if (left <= right)
        {
            for (int i = 0; i < left; i++)
            {
                var temp = array[0];
                Array.Copy(array, 1, array, 0, array.Length - 1);
                array[array.Length - 1] = temp;
            }
        }
        else
        {
            for (int i = 0; i < right; i++)
            {
                var temp = array[array.Length - 1];
                Array.Copy(array, 0, array, 1, array.Length - 1);
                array[0] = temp;
            }
        }
    }

    public static void Rotate<T>(this List<T> list, int count)
    {
        for (int i = 0; i < count; i++)
        {
            T first = list[0];
            list.RemoveAt(0);
            list.Add(first);
        }
    }
}