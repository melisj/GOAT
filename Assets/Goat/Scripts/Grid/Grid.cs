using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAT.Grid
{
    public class Grid : MonoBehaviour
    {
        [SerializeField] private Vector2Int gridSize = new Vector2Int(10, 10);
        [SerializeField] private float tileSize = 1.0f;
        private float tileOffset;
        public Tile[,] tiles;
        [SerializeField] private LayerMask gridMask;
        
        private void Start()
        {
            transform.localScale = new Vector3(gridSize.x, 1, gridSize.y) * tileSize;
            transform.position = new Vector3(gridSize.x, 1, gridSize.y) * tileSize / 2;

            tileOffset = tileSize / 2;
        }

        private void Update() {
            Vector3 mouseHit = GetMousePosition();
            Vector2Int tileIndex = CalculateTilePositionInArray(mouseHit);
            Tile tileInfo = ReturnTile(tileIndex);
            print(tileIndex);
        }

        private void InitializeTiles(Vector2Int gridSize, float tileSize)
        {
            tiles = new Tile[gridSize.x, gridSize.y];
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    tiles[x, y] = new Tile();
                }
            }
        }

        private Vector2Int CalculateTilePositionInArray(Vector3 rayHitPosition)
        {
            //tile position gaat altijd X+ Y+ vanuit 
            Vector2 hitPosition = new Vector2(rayHitPosition.x, rayHitPosition.z);

            Vector2 relativeHitPos = (hitPosition) / tileSize;
            return new Vector2Int(Mathf.FloorToInt(relativeHitPos.x), Mathf.FloorToInt(relativeHitPos.y));
        }

        private Tile ReturnTile(Vector2Int tilePositionInArray)
        {
            if (tilePositionInArray.x < tiles.GetLength(0) && tilePositionInArray.y < tiles.GetLength(1))
            {
                return tiles[tilePositionInArray.x, tilePositionInArray.y];
            }
            else Debug.LogError("Grid Selection is outside of tile bounds");
            return null;
        }
    
        private Vector3 GetMousePosition() {
            Vector3 mousePosition = Input.mousePosition + new Vector3(0, 0, Camera.main.nearClipPlane);
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            Vector3 cameraPerspective = mouseWorldPosition - Camera.main.transform.position;

            if (Physics.Raycast(mouseWorldPosition, cameraPerspective, out RaycastHit hit, Mathf.Infinity)) {
                return hit.point;
            } 
            return Vector3.zero;
        }

        private void HighLightTile(Tile tile) {
            tile.Select();
        }
    }
}

