using System.Collections.Generic;
using UnityEngine;

namespace GOAT.Grid
{
    public enum TileType
    {
        Empty,
        Building
    }

    public class Tile
    {
        private bool isSolid;
        private bool isUnlocked;
        private bool isSelected;
        private TileType tileType;
        
        public void Select() {

        }

        public void UnSelect() {

        }
    }
}