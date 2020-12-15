using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "GridObjectsList", menuName = "ScriptableObjects/GlobalVariables/GridObjectsList")]
public class GridObjectsList : SerializedScriptableObject
{
    [SerializeField] private List<Buyable> buyables = new List<Buyable>();

    private void OnValidate()
    {
        SetID();
    }

    [Button("Set ID's", ButtonSizes.Large)]
    private void SetInteractables()
    {
        Object[] list = Resources.LoadAll("", typeof(Buyable));
        for (int i = 0; i < list.Length; i++)
        {
            if (!buyables.Contains((Buyable)list[i]))
                buyables.Add((Buyable)list[i]);
        }

        EditorUtility.SetDirty(this);

        SetID();
    }

    private void SetID()
    {
        // Assign new id's to the interactables
        bool changedList = false;
        for (int i = 0; i < buyables.Count; i++)
        {
            if (buyables[i].ID != i)
            {
                buyables[i].ID = i;
                EditorUtility.SetDirty(buyables[i]);
                changedList = true;
            }
        }

        // Save changes
        if (changedList)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    public Buyable GetObject(int index)
    {
        return buyables[index];
    }
}
