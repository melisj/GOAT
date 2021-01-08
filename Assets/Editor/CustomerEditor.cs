using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector.Editor;
using UnityEditor.AI;
using Goat.AI;

[CustomEditor(typeof(Customer))]
public class CustomerEditor : OdinEditor
{
    private SerializedProperty storeArea;

    protected override void OnEnable()
    {
        base.OnEnable();
        storeArea = serializedObject.FindProperty("storeArea");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        NavMeshComponentsGUIUtility.AreaPopup("Area Type", storeArea);
        serializedObject.ApplyModifiedProperties();
    }
}