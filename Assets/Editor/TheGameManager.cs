using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System;
using System.Linq;
using Goat.AI.Parking;
using Goat.Farming;
using Goat.Storage;
using Goat.Grid;

public class TheGameManager : OdinMenuEditorWindow
{
    private const string ScriptableObjectPath = "Assets/Goat/_ScriptableObjects/";

    public enum ManagerState
    {
        Review,
        Buyables,
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
    private DrawSelected<Buyable> drawBuyables = new DrawSelected<Buyable>(0);
    private DrawSelected<HiredEmployee> drawEmployees = new DrawSelected<HiredEmployee>(1);
    private DrawSelected<FarmStation> drawFarms = new DrawSelected<FarmStation>(2);
    private DrawSelected<Floor> drawFloors = new DrawSelected<Floor>(3);
    private DrawSelected<Building> drawFurniture = new DrawSelected<Building>(4);
    private DrawSelected<Resource> drawResource = new DrawSelected<Resource>(5);
    private DrawSelected<ResourceTileData> drawResourceTiles = new DrawSelected<ResourceTileData>(6);
    private DrawSelected<Buyable> drawTubes = new DrawSelected<Buyable>(7);
    private DrawSelected<Wall> drawWalls = new DrawSelected<Wall>(8);
    private DrawSelected<Characters> drawCharacters = new DrawSelected<Characters>(9);
    private DrawSelected<AudioCueSO> drawAudio = new DrawSelected<AudioCueSO>(10);
    private DrawSelected<AudioCueSO> drawMusic = new DrawSelected<AudioCueSO>(11);
    private DrawSelected<AudioConfigurationSO> drawAudioConfig = new DrawSelected<AudioConfigurationSO>(12);
    private DrawSelected<AudioConfigurationSO> drawMusicConfig = new DrawSelected<AudioConfigurationSO>(13);
    private DrawSelected<ReviewData> drawReviews = new DrawSelected<ReviewData>(14);

    private string buyablePath = "Resources";
    private string employeesPath => $"{ScriptableObjectPath}{buyablePath}/{"Employees"}";
    private string farmingPath => $"{ScriptableObjectPath}{buyablePath}/{"Farming"}";
    private string floorsPath => $"{ScriptableObjectPath}{buyablePath}/{"Floors"}";
    private string furniturePath => $"{ScriptableObjectPath}{buyablePath}/{"Furniture"}";
    private string resourcePath => $"{ScriptableObjectPath}{buyablePath}/{"Resource"}";
    private string resourceTilesPath => $"{ScriptableObjectPath}{buyablePath}/{"ResourceTiles"}";
    private string tubesPath => $"{ScriptableObjectPath}{buyablePath}/{"Tubes"}";
    private string wallsPath => $"{ScriptableObjectPath}{buyablePath}/{"Walls"}";
    private string characterPath = "Data/RandomNPC";
    private string audioPath = "Audio";
    private string settingsPath = "Settings";
    private string soundsPath = "Sounds";
    private string reviewPath = "Data/Review";

    private bool guiInitialized;

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
        drawCharacters.SetPath(ScriptableObjectPath + characterPath);
        drawAudio.SetPath(ScriptableObjectPath + audioPath);
        drawAudioConfig.SetPath(ScriptableObjectPath + audioPath);
        drawReviews.SetPath(ScriptableObjectPath + reviewPath);

        drawEmployees.OnChanged += ItemChanged;
        drawFarms.OnChanged += ItemChanged;
        drawFloors.OnChanged += ItemChanged;
        drawFurniture.OnChanged += ItemChanged;
        drawResource.OnChanged += ItemChanged;
        drawResourceTiles.OnChanged += ItemChanged;
        drawTubes.OnChanged += ItemChanged;
        drawWalls.OnChanged += ItemChanged;
        drawCharacters.OnChanged += ItemChanged;
        drawAudio.OnChanged += ItemChanged;
        drawAudioConfig.OnChanged += ItemChanged;
        drawBuyables.OnChanged += ItemChanged;
        drawReviews.OnChanged += ItemChanged;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        drawEmployees.OnChanged -= ItemChanged;
        drawFarms.OnChanged -= ItemChanged;
        drawFloors.OnChanged -= ItemChanged;
        drawFurniture.OnChanged -= ItemChanged;
        drawResource.OnChanged -= ItemChanged;
        drawResourceTiles.OnChanged -= ItemChanged;
        drawTubes.OnChanged -= ItemChanged;
        drawWalls.OnChanged -= ItemChanged;
        drawCharacters.OnChanged -= ItemChanged;
        drawAudio.OnChanged -= ItemChanged;
        drawAudioConfig.OnChanged -= ItemChanged;
        drawBuyables.OnChanged -= ItemChanged;
        drawReviews.OnChanged -= ItemChanged;
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        OdinMenuTree tree = new OdinMenuTree();
        switch (managerState)
        {
            case ManagerState.Review:
                tree.AddAllAssetsAtPath("Review Data", ScriptableObjectPath + reviewPath, typeof(ReviewData));

                break;

            case ManagerState.Buyables:
                drawEmployees.SetPath(employeesPath);
                drawFarms.SetPath(farmingPath);
                drawFloors.SetPath(floorsPath);
                drawFurniture.SetPath(furniturePath);
                drawResource.SetPath(resourcePath);
                drawResourceTiles.SetPath(resourceTilesPath);
                drawTubes.SetPath(tubesPath);
                drawWalls.SetPath(wallsPath);
                tree.AddAllAssetsAtPath("Employees", employeesPath, typeof(Buyable));
                tree.AddAllAssetsAtPath("Farms", farmingPath, typeof(Buyable));
                tree.AddAllAssetsAtPath("Floors", floorsPath, typeof(Buyable));
                tree.AddAllAssetsAtPath("Furniture", furniturePath, typeof(Buyable));
                tree.AddAllAssetsAtPath("Resources", resourcePath, typeof(Buyable));
                tree.AddAllAssetsAtPath("ResourceTiles", resourceTilesPath, typeof(Buyable));
                tree.AddAllAssetsAtPath("Tubes", tubesPath, typeof(Buyable));
                tree.AddAllAssetsAtPath("Walls", wallsPath, typeof(Buyable));

                break;

            case ManagerState.Characters:
                tree.AddAllAssetsAtPath("Characters", ScriptableObjectPath + characterPath, typeof(Characters));

                break;

            case ManagerState.Sfx:
                drawAudio.SetPath($"{ScriptableObjectPath}{audioPath}/{managerState}/{soundsPath}");
                drawAudioConfig.SetPath($"{ScriptableObjectPath}{audioPath}/{managerState}/{settingsPath}");
                tree.AddAllAssetsAtPath(managerState.ToString(), $"{ScriptableObjectPath}{audioPath}/{managerState}/{soundsPath}", typeof(AudioCueSO));
                tree.AddAllAssetsAtPath(managerState.ToString() + " Settings", $"{ScriptableObjectPath}{audioPath}/{managerState}/{settingsPath}", typeof(AudioConfigurationSO));
                break;

            case ManagerState.Music:
                drawMusic.SetPath($"{ScriptableObjectPath}{audioPath}/{managerState}/{soundsPath}");
                drawMusicConfig.SetPath($"{ScriptableObjectPath}{audioPath}/{managerState}/{settingsPath}");
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
        EditorGUILayout.Space();

        if (guiInitialized)
            DrawEditor(enumIndex);
        EditorGUILayout.Space();
        base.OnGUI();
        guiInitialized = true;
    }

    protected override void DrawEditors()
    {
        switch (managerState)
        {
            case ManagerState.Review:
                if (DrawAndSetSelected<ReviewData>(drawReviews))
                    return;
                break;

            case ManagerState.Buyables:
                if (DrawAndSetSelected<HiredEmployee>(drawEmployees))
                    return;
                if (DrawAndSetSelected<FarmStation>(drawFarms))
                    return;
                if (DrawAndSetSelected<Floor>(drawFloors))
                    return;
                if (DrawAndSetSelected<Building>(drawFurniture))
                    return;
                if (DrawAndSetSelected<Resource>(drawResource))
                    return;
                if (DrawAndSetSelected<ResourceTileData>(drawResourceTiles))
                    return;
                if (DrawAndSetSelected<Buyable>(drawTubes))
                    return;
                if (DrawAndSetSelected<Wall>(drawWalls))
                    return;
                break;

            case ManagerState.Characters:
                if (DrawAndSetSelected<Characters>(drawCharacters))
                    return;
                break;

            case ManagerState.Sfx:
            case ManagerState.Music:
                if (this.MenuTree.Selection.SelectedValue is AudioCueSO)
                {
                    drawAudio.SetSelected(this.MenuTree.Selection.SelectedValue);
                    DrawEditor(drawAudio.Index);
                    return;
                }
                else if (this.MenuTree.Selection.SelectedValue is AudioConfigurationSO)
                {
                    drawAudioConfig.SetSelected(this.MenuTree.Selection.SelectedValue);
                    DrawEditor(drawAudioConfig.Index);
                    return;
                }
                break;

            default:
                break;
        }
        if (this.MenuTree.Selection.SelectedValue != null)
            DrawEditor(enumIndex);
    }

    private bool DrawAndSetSelected<T>(DrawSelected<T> selected) where T : ScriptableObject
    {
        bool isTypeOf = this.MenuTree.Selection.SelectedValue is T;
        if (isTypeOf)
        {
            selected.SetSelected(this.MenuTree.Selection.SelectedValue);
            DrawEditor(selected.Index);
        }
        return isTypeOf;
    }

    protected override IEnumerable<object> GetTargets()
    {
        object[] targets = new object[15];

        targets[drawReviews.Index] = drawReviews;
        targets[drawEmployees.Index] = drawEmployees;
        targets[drawFarms.Index] = drawFarms;
        targets[drawFloors.Index] = drawFloors;
        targets[drawFurniture.Index] = drawFurniture;
        targets[drawResource.Index] = drawResource;
        targets[drawResourceTiles.Index] = drawResourceTiles;
        targets[drawTubes.Index] = drawTubes;
        targets[drawWalls.Index] = drawWalls;
        targets[drawCharacters.Index] = drawCharacters;
        targets[drawAudio.Index] = drawAudio;
        targets[drawMusic.Index] = drawMusic;
        targets[drawMusicConfig.Index] = drawMusicConfig;
        targets[drawAudioConfig.Index] = drawAudioConfig;

        List<object> allTargets = targets.ToList<object>();
        allTargets.Add(base.GetTarget());
        enumIndex = allTargets.Count - 1;
        return allTargets;
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
    [ShowInInspector, VerticalGroup("CreateNew/Vertical")] private string defaultPath => path + "\\" + nameForNew + ".asset";
    [SerializeField, VerticalGroup("CreateNew/Vertical")] private bool overridePath;
    [SerializeField, ShowIf("overridePath"), VerticalGroup("CreateNew/Vertical")]
    private string path;
    private int index;
    public int Index => index;

    public DrawSelected(int index)
    {
        this.index = index;
    }

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