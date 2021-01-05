using Sirenix.OdinInspector;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SceneLoaderForBuild))]
public class SceneLoaderForEditor : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField, ReadOnly]private SceneAsset[] scencesToLoad;
#endif
    private SceneLoaderForBuild sceneLoaderBuild;
    public SceneLoaderForBuild SceneLoaderBuild { get {
            if (sceneLoaderBuild == null)
                sceneLoaderBuild = GetComponent<SceneLoaderForBuild>();
            return sceneLoaderBuild;
        }
    }

    [Button("Load necessary scenes")]
    public void LoadScenesInEditor()
    {
#if UNITY_EDITOR
        if (EditorUtility.DisplayDialog("Load Scenes", "This will load in new scenes listed in the inspector!", "Load all", "Nope!"))
        {
            scencesToLoad = Resources.LoadAll<SceneAsset>("SceneLoader");
            LoadAllScenes();
        }
#endif
    }

    [Button("Store paths for build")]
    public void StoreScenesForBuild()
    {
#if UNITY_EDITOR
        scencesToLoad = Resources.LoadAll<SceneAsset>("SceneLoader");
        StoreScenes();
#endif
    }

    private void StoreScenes()
    {
#if UNITY_EDITOR
        string[] scenePaths = new string[scencesToLoad.Length];
        for(int i = 0; i < scencesToLoad.Length; i++) 
        {
            scenePaths[i] = AssetDatabase.GetAssetPath(scencesToLoad[i]);
        }
        SceneLoaderBuild.SetPaths(scenePaths);
#endif
    }

    private void LoadAllScenes()
    {
#if UNITY_EDITOR
        foreach (SceneAsset scene in scencesToLoad)
        {
            EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(scene), OpenSceneMode.Additive);
        }
#endif
    }
}
