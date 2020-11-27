using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;

public class MeshToImage : MonoBehaviour
{
    private const string folderPath = "/Goat/Textures/UI/MeshImages/";
    private const string dataType = ".png";

    [SerializeField] private RenderTexture rt;
    [SerializeField, AssetList(Path = "Goat/Prefabs/Grid/Resources")] private GameObject meshPrefab;
    [SerializeField] private int width = 32;
    [SerializeField] private int height = 32;
    [SerializeField] private bool testCameraSize;
    private GameObject mesh;

    //private void LateUpdate()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        CreateImage();
    //    }
    //}

    private void CreateMesh()
    {
        mesh = Instantiate(meshPrefab, Vector3.zero, Quaternion.identity, transform);
    }

    private void DeleteMesh()
    {
        for (int i = transform.childCount; i > 0; i--)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }

    [Button]
    private void CreateImage()
    {
        CreateMesh();
        byte[] bytes = ToTexture2D(rt).EncodeToPNG();
        string path = Application.dataPath + folderPath + meshPrefab.name + dataType;
        //FileStream stream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
        //BinaryWriter writer = new BinaryWriter(stream);
        //for (int i = 0; i < bytes.Length; i++)
        //{
        //    writer.Write(bytes[i]);
        //}
        //writer.Close();
        //stream.Close();
        File.WriteAllBytes(path, bytes);
        Debug.LogFormat("Created image of {0} at {1}", meshPrefab.name, path);
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
        if (testCameraSize) return;
        DeleteMesh();
    }

    private Texture2D ToTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.ARGB32, false);
        Camera.main.Render();
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }
}