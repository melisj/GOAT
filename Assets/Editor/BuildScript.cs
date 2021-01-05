using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using System.IO;
using System;

public class BuildScript
{
    public static void PerformBuild()
    {
        string path = Path.GetFullPath(Path.Combine(Application.dataPath, "../"));
        if (Directory.Exists(path + "Build"))
            Directory.CreateDirectory(path + "Build");
        path += "Build/Application.exe";

        Console.WriteLine("Saving to: " + path);

        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, path, BuildTarget.StandaloneWindows, BuildOptions.None);
    }
}