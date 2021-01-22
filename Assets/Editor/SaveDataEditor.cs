using Goat.Saving;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SaveDataEditor : OdinMenuEditorWindow
{
    SaveHandler[] handlers = new SaveHandler[0];
    bool shouldRebuild = false;
    int amountOfHandlers;

    DrawSelectedObject<SaveHandler> saveHandlers = new DrawSelectedObject<SaveHandler>();

    [MenuItem("Tools/Save Data Info")]
    private static void OpenWindow()
    {
        GetWindow<SaveDataEditor>().Show();
    }

    protected override void OnGUI()
    {
        SirenixEditorGUI.Title("Save Manager", "", TextAlignment.Center, true);

        GetHandlers();

        if (shouldRebuild && Event.current.type == EventType.Layout)
        {
            ForceMenuTreeRebuild();
            shouldRebuild = false;
        }

        base.OnGUI();
    }

    protected override void DrawEditors()
    {
        saveHandlers?.SetSelected(MenuTree.Selection.SelectedValue);

        base.DrawEditors();
    }

    protected override IEnumerable<object> GetTargets()
    {
        List<object> targets = new List<object>();

        targets.Add(saveHandlers);
        targets.Add(base.GetTarget());

        return targets;
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree();

        if (handlers.Length > 0)
        {
            foreach (SaveHandler handler in handlers)
            {
                tree.Add(handler.data.className, handler);
            }
        }

        return tree;
    }

    protected override void DrawMenu()
    {
        if(handlers.Length > 0)
            base.DrawMenu();
    }

    protected void GetHandlers()
    {
        amountOfHandlers = handlers.Length;
        handlers = FindObjectsOfType<SaveHandler>();
        if (amountOfHandlers != handlers.Length)
            shouldRebuild = true;
    }
}


public class DrawSelectedObject<T> where T : SaveHandler
{
    [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
    public T selected;

    public void SetSelected(object item)
    {
        selected = item as T;
    }

    [ShowIf("@selected != null")]
    [GUIColor(.1f,.5f,.4f)]
    [ButtonGroup("Select GameObject in Scene", -1000)]
    private void SelectObjectInScene()
    {
        if (selected != null)
            Selection.activeGameObject = selected.gameObject;
    }
}