#if UNITY_EDITOR
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
            scencesToLoad = Resources.LoadAll<SceneAsset>("SceneLoader");
            LoadAllScenes();
        }
    }

    [Button("Store paths for build")]
    public void StoreScenesForBuild()
    {
        scencesToLoad = Resources.LoadAll<SceneAsset>("SceneLoader");
        StoreScenes();
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
        foreach (SceneAsset scene in scencesToLoad)
        {
            if (Application.isPlaying)
                SceneManager.LoadScene(AssetDatabase.GetAssetPath(scene), LoadSceneMode.Additive);
            else
                EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(scene), OpenSceneMode.Additive);
        }
    }
}
#endif
