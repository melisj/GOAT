﻿using Goat.Grid;
using Sirenix.OdinInspector;
using System.IO;
using UnityEditor;
using UnityEngine;

public class MeshToImage : MonoBehaviour
{
    private const string folderPath = "/Goat/Textures/UI/MeshImages/Resources/";
    private const string dataType = ".png";

    [SerializeField] private RenderTexture rt;
    [SerializeField, AssetList(Path = "/Goat/_ScriptableObjects/Resources")] private Buyable[] placeable;
    [SerializeField] private int width = 32;
    [SerializeField] private int height = 32;
    [SerializeField] private bool testCameraSize;
    [SerializeField] private MeshFilter[] mesh;

    //private void LateUpdate()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        CreateImage();
    //    }
    //}
    private void InitializeResTile(ResourceTileData resData)
    {
        ResourceTile resTile = mesh[0].transform.root.gameObject.GetComponent<ResourceTile>();
        resTile.Setup(resData);
    }

    private void CreateMesh(Buyable place)
    {
        for (int i = 0; i < mesh.Length; i++)
        {
            mesh[i].transform.localScale = Vector3.one;
            if (i >= place.Mesh.Length)
            {
                mesh[i].mesh = null;
                continue;
            }
            mesh[i].mesh = place.Mesh[i];
            if (place is ResourceTileData tileData)
            {
                InitializeResTile(tileData);
            }
        }
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
        for (int i = 0; i < placeable.Length; i++)
        {
            CreateMesh(placeable[i]);
            byte[] bytes = ToTexture2D(rt).EncodeToPNG();
            string path = Application.dataPath + folderPath + placeable[i].name + dataType;
            //FileStream stream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
            //BinaryWriter writer = new BinaryWriter(stream);
            //for (int i = 0; i < bytes.Length; i++)
            //{
            //    writer.Write(bytes[i]);
            //}
            //writer.Close();
            //stream.Close();
            File.WriteAllBytes(path, bytes);
            Debug.LogFormat("Created image of {0} at {1}", placeable[i].name, path);
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }
        //if (testCameraSize) return;
        //DeleteMesh();
    }

    [Button]
    private void CreateImageOfCurrentRender(string name)
    {
        byte[] bytes = ToTexture2D(rt).EncodeToPNG();
        string path = Application.dataPath + folderPath + name + dataType;
        //FileStream stream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
        //BinaryWriter writer = new BinaryWriter(stream);
        //for (int i = 0; i < bytes.Length; i++)
        //{
        //    writer.Write(bytes[i]);
        //}
        //writer.Close();
        //stream.Close();
        File.WriteAllBytes(path, bytes);
        Debug.LogFormat("Created image of {0} at {1}", name, path);
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif

        //if (testCameraSize) return;
        //DeleteMesh();
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