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
            GetTileInformation();
        }

        public void DeSelect() {

        }

        public TileInformation GetTileInformation()
        {
            // Initialize tile information
            return new TileInformation(centerPosition, tileType);
        }
    }

    public struct TileInformation
    {
        public readonly Vector3 TilePosition;
        public readonly TileType TileType;

        public TileInformation(Vector3 TilePosition, TileType TileType)
        {
            this.TilePosition = TilePosition;
            this.TileType = TileType;
        }
    }
}