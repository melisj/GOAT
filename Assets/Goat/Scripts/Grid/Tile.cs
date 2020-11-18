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
        private Vector3 centerPosition;
        private bool isSolid;
        private bool isUnlocked;
        private bool isSelected;
        private TileType tileType;
        
        public Tile(Vector3 centerPosition) {
            this.centerPosition = centerPosition;
        }

        public void Select() {
            Debug.Log("You've selected me at: " + centerPosition);
        }

        public void DeSelect() {

        }
    }
}