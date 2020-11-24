using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GOAT.Grid
{
    public static class TileAssets
    {
        private static Dictionary<FloorType, GameObject> floorAssets = new Dictionary<FloorType, GameObject>();
        private static Dictionary<BuildingType, GameObject> buildingAssets = new Dictionary<BuildingType, GameObject>();
        private static Dictionary<WallType, GameObject> wallAssets = new Dictionary<WallType, GameObject>();
        //private static Dictionary<Enum, string> testassets = new Dictionary<Enum, string>() { };

        public static void InitializeAssetsDictionary()
        {
            Array floorValues = Enum.GetValues(typeof(FloorType));
            for (int i = 0; i < floorValues.Length; i++)
            {
                FloorType type = ((FloorType)i);
                string typeName = type.ToString();
                floorAssets.Add(type, (GameObject)UnityEngine.Resources.Load("Floor-" + typeName));
            }

            Array buildingValues = Enum.GetValues(typeof(BuildingType));
            for (int i = 0; i < buildingValues.Length; i++)
            {
                BuildingType type = ((BuildingType)i);
                string typeName = type.ToString();
                buildingAssets.Add(type, (GameObject)UnityEngine.Resources.Load("Building-" + typeName));
            }
            Array wallValues = Enum.GetValues(typeof(WallType));
            for (int i = 0; i < wallValues.Length; i++)
            {
                WallType type = ((WallType)i);
                string typename = type.ToString();
                wallAssets.Add(type, (GameObject)UnityEngine.Resources.Load("Wall-" + typename));
            }
        }

        public static GameObject FindAsset(FloorType type)
        {
            if (floorAssets.ContainsKey(type))
                return floorAssets[type];
            else
                return null;
        }
        public static GameObject FindAsset(BuildingType type)
        {
            if (buildingAssets.ContainsKey(type))
                return buildingAssets[type];
            else
                return null;
        }
        public static GameObject FindAsset(WallType type)
        {
            if (wallAssets.ContainsKey(type))
                return wallAssets[type];
            else
                return null;
        }

    }
}