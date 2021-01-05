using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SceneLoaderForBuild))]
public class SceneLoaderForEditor : MonoBehaviour
{
    [SerializeField, ReadOnly]private SceneAsset[] scencesToLoad;
    
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
        if (EditorUtility.DisplayDialog("Load Scenes", "This will load in new scenes listed in the inspector!", "Load all", "Nope!"))
        {
#if UNITY_EDITOR
            scencesToLoad = Resources.LoadAll<SceneAsset>("SceneLoader");
            LoadAllScenes();
#endif
        }
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
        string[] scenePaths = new string[scencesToLoad.Length];
        for(int i = 0; i < scencesToLoad.Length; i++) 
        {
            scenePaths[i] = AssetDatabase.GetAssetPath(scencesToLoad[i]);
        }
        SceneLoaderBuild.SetPaths(scenePaths);
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
