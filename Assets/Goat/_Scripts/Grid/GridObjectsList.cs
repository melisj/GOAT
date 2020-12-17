using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "GridObjectsList", menuName = "ScriptableObjects/GlobalVariables/GridObjectsList")]
public class GridObjectsList : SerializedScriptableObject
{
    [SerializeField] private List<Buyable> buyables = new List<Buyable>();

    [Button("Set ID's", ButtonSizes.Large)]
    private void SetInteractables()
    {
        Object[] list = Resources.LoadAll("", typeof(Buyable));
        for (int i = 0; i < list.Length; i++)
        {
            if (!buyables.Contains((Buyable)list[i]))
                buyables.Add((Buyable)list[i]);
        }
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
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
#if UNITY_EDITOR
                EditorUtility.SetDirty(buyables[i]);
#endif
                changedList = true;
            }
        }

        // Save changes
        if (changedList)
        {
#if UNITY_EDITOR
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }
    }

    public Buyable GetObject(int index)
    {
        return buyables[index];
    }
}