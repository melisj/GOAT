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
            tileOffset = tileSize / 2;
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
            Vector2 gridOrigin = new Vector2(transform.position.x, transform.position.z);
            Vector2 hitPosition = new Vector2(rayHitPosition.x, rayHitPosition.z);

            Vector2 relativeHitPos = (hitPosition - gridOrigin) / tileSize;
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
    
        private Vector2Int GetMouseGridPosition() {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 cameraDirection = Camera.main.transform.eulerAngles;            

            if (Physics.Raycast(mousePosition, cameraDirection, out RaycastHit hit, Mathf.Infinity, gridMask)) {
                return CalculateTilePositionInArray(hit.point);
            } else {
                return Vector2Int.zero;
            }
        }

        private void HighLightTile(Tile tile) {
            tile.Select();
        }
    }
}

