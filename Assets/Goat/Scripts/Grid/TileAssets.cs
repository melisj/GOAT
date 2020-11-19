using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GOAT.Grid {
    public static class TileAssets
    {
        private static Dictionary<Enum, GameObject> tileAssets = new Dictionary<Enum, GameObject>() { };
        //private static Dictionary<BuildingType, GameObject> buildingAssets = new Dictionary<BuildingType, GameObject>();

        public static void InitializeAssetsDictionary() {
            tileAssets.Add(FloorType.Empty, new GameObject());
            
            //UnityEngine.Resources.Load("");
        }

        public static GameObject FindAsset(Enum enumValue) {
            return tileAssets[enumValue];
        }
    }
}