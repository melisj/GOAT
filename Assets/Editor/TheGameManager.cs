using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System;

public class TheGameManager : OdinMenuEditorWindow
{
    private const string ScriptableObjectPath = "Assets/Goat/_ScriptableObjects/";

    public enum ManagerState
    {
        Review,
        Employees,
        Farming,
        Floors,
        Furniture,
        Resource,
        ResourceTiles,
        Tubes,
        Walls,
        Characters,
        Sfx,
        Music
    }

    [OnValueChanged("StateChanged")]
    [LabelText("Manager View")]
    [LabelWidth(100f)]
    [EnumToggleButtons()]
    [ShowInInspector]
    private ManagerState managerState;
    private int enumIndex = 0;
    private bool treeRebuild = true;
    private DrawSelected<Buyable> drawBuyables = new DrawSelected<Buyable>();
    private DrawSelected<Characters> drawCharacters = new DrawSelected<Characters>();
    private DrawSelected<AudioCueSO> drawAudio = new DrawSelected<AudioCueSO>();
    private DrawSelected<AudioConfigurationSO> drawAudioConfig = new DrawSelected<AudioConfigurationSO>();
    private DrawSelected<ReviewData> drawReviews = new DrawSelected<ReviewData>();

    private string buyablePath = "Resources";
    private string characterPath = "Data/RandomNPC";
    private string audioPath = "Audio";
    private string settingsPath = "Settings";
    private string soundsPath = "Sounds";
    private string reviewPath = "Data/Review";

    [MenuItem("Tools/The Game Manager")]
    public static void OpenWindow()
    {
        GetWindow<TheGameManager>().Show();
    }

    private void StateChanged()
    {
        treeRebuild = true;
    }

    private void ItemChanged(object sender, EventArgs e)
    {
        treeRebuild = true;
    }

    protected override void Initialize()
    {
        drawBuyables.SetPath(ScriptableObjectPath + buyablePath);
        drawBuyables.OnChanged += ItemChanged;
        drawCharacters.SetPath(ScriptableObjectPath + characterPath);
        drawCharacters.OnChanged += ItemChanged;
        drawAudio.SetPath(ScriptableObjectPath + audioPath);
        drawAudio.OnChanged += ItemChanged;
        drawAudioConfig.SetPath(ScriptableObjectPath + audioPath);
        drawAudioConfig.OnChanged += ItemChanged;
        drawReviews.SetPath(ScriptableObjectPath + reviewPath);
        drawReviews.OnChanged += ItemChanged;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        drawBuyables.OnChanged -= ItemChanged;
        drawCharacters.OnChanged -= ItemChanged;
        drawAudio.OnChanged -= ItemChanged;
        drawReviews.OnChanged -= ItemChanged;
        drawAudioConfig.OnChanged -= ItemChanged;
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        OdinMenuTree tree = new OdinMenuTree();
        switch (managerState)
        {
            case ManagerState.Review:
                tree.AddAllAssetsAtPath("Review Data", ScriptableObjectPath + reviewPath, typeof(ReviewData));

                break;

            case ManagerState.Employees:
            case ManagerState.Farming:
            case ManagerState.Floors:
            case ManagerState.Furniture:
            case ManagerState.Resource:
            case ManagerState.ResourceTiles:
            case ManagerState.Tubes:
            case ManagerState.Walls:
                drawBuyables.SetPath(ScriptableObjectPath + buyablePath + "/" + managerState.ToString());
                tree.AddAllAssetsAtPath(managerState.ToString(), ScriptableObjectPath + buyablePath + "/" + managerState.ToString(), typeof(Buyable));

                break;

            case ManagerState.Characters:
                tree.AddAllAssetsAtPath("Characters", ScriptableObjectPath + characterPath, typeof(Characters));

                break;

            case ManagerState.Sfx:
            case ManagerState.Music:
                drawAudio.SetPath($"{ScriptableObjectPath}{audioPath}/{managerState}/{soundsPath}");
                drawAudioConfig.SetPath($"{ScriptableObjectPath}{audioPath}/{managerState}/{settingsPath}");

                tree.AddAllAssetsAtPath(managerState.ToString(), $"{ScriptableObjectPath}{audioPath}/{managerState}/{soundsPath}", typeof(AudioCueSO));
                tree.AddAllAssetsAtPath(managerState.ToString() + " Settings", $"{ScriptableObjectPath}{audioPath}/{managerState}/{settingsPath}", typeof(AudioConfigurationSO));
                break;

            default:
                break;
        }

        return tree;
    }

    protected override void OnGUI()
    {
        if (treeRebuild && Event.current.type == EventType.Layout)
        {
            ForceMenuTreeRebuild();
            treeRebuild = false;
        }

        SirenixEditorGUI.Title("The Game Manager", "", TextAlignment.Center, true);
        switch (managerState)
        {
            case ManagerState.Employees:
            case ManagerState.Farming:
            case ManagerState.Floors:
            case ManagerState.Furniture:
            case ManagerState.Resource:
            case ManagerState.ResourceTiles:
            case ManagerState.Tubes:
            case ManagerState.Walls:
            case ManagerState.Characters:
            case ManagerState.Sfx:
            case ManagerState.Music:
                DrawEditor(enumIndex);

                break;

            default:
                break;
        }

        base.OnGUI();
    }

    protected override void DrawEditors()
    {
        switch (managerState)
        {
            case ManagerState.Review:
                DrawEditor(enumIndex);

                drawReviews.SetSelected(this.MenuTree.Selection.SelectedValue);
                break;

            case ManagerState.Employees:
            case ManagerState.Farming:
            case ManagerState.Floors:
            case ManagerState.Furniture:
            case ManagerState.Resource:
            case ManagerState.ResourceTiles:
            case ManagerState.Tubes:
            case ManagerState.Walls:
                drawBuyables.SetSelected(this.MenuTree.Selection.SelectedValue);

                break;

            case ManagerState.Characters:
                drawCharacters.SetSelected(this.MenuTree.Selection.SelectedValue);

                break;

            case ManagerState.Sfx:
            case ManagerState.Music:
                if (this.MenuTree.Selection.SelectedValue is AudioCueSO)
                {
                    drawAudio.SetSelected(this.MenuTree.Selection.SelectedValue);
                    DrawEditor((int)managerState);
                    return;
                }
                else if (this.MenuTree.Selection.SelectedValue is AudioConfigurationSO)
                {
                    drawAudioConfig.SetSelected(this.MenuTree.Selection.SelectedValue);
                    DrawEditor((int)managerState + 2);
                    return;
                }
                break;

            default:
                break;
        }

        DrawEditor((int)managerState);
    }

    protected override IEnumerable<object> GetTargets()
    {
        List<object> targets = new List<object>();
        targets.Add(drawReviews);
        targets.Add(drawBuyables);
        targets.Add(drawBuyables);
        targets.Add(drawBuyables);
        targets.Add(drawBuyables);
        targets.Add(drawBuyables);
        targets.Add(drawBuyables);
        targets.Add(drawBuyables);
        targets.Add(drawBuyables);
        targets.Add(drawCharacters);
        targets.Add(drawAudio);
        targets.Add(drawAudio);
        targets.Add(drawAudioConfig);
        targets.Add(drawAudioConfig);
        targets.Add(base.GetTarget());
        enumIndex = targets.Count - 1;
        return targets;
    }

    protected override void DrawMenu()
    {
        base.DrawMenu();
    }
}

public class DrawSelected<T> where T : ScriptableObject
{
    [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
    public T selected;

    [LabelWidth(100)]
    [PropertyOrder(-1)]
    [BoxGroup("CreateNew", showLabel: false)]
    [HorizontalGroup("CreateNew/Horizontal")]
    public string nameForNew;
    private string path;

    public event EventHandler OnChanged;

    [HorizontalGroup("CreateNew/Horizontal")]
    [GUIColor(0.7f, 0.7f, 1f)]
    [PropertyOrder(-1)]
    [Button]
    public void CreateNew()
    {
        if (nameForNew == "")
            return;

        T newItem = ScriptableObject.CreateInstance<T>();
        newItem.name = "New " + typeof(T).ToString();

        if (path == "")
            path = "Assets/";

        AssetDatabase.CreateAsset(newItem, path + "\\" + nameForNew + ".asset");
        AssetDatabase.SaveAssets();

        nameForNew = "";
        OnChanged?.Invoke(this, null);
    }

    [HorizontalGroup("CreateNew/Horizontal")]
    [GUIColor(1f, 0.7f, 1f)]
    [PropertyOrder(-1)]
    [Button]
    public void DeleteSelected()
    {
        if (selected != null)
        {
            string selectedPath = AssetDatabase.GetAssetPath(selected);
            AssetDatabase.DeleteAsset(selectedPath);
            AssetDatabase.SaveAssets();
        }
        OnChanged?.Invoke(this, null);
    }

    public void SetSelected(object item)
    {
        T newSelected = item as T;
        if (newSelected != null)
            this.selected = newSelected;
    }

    public void SetPath(string path)
    {
        this.path = path;
    }
}