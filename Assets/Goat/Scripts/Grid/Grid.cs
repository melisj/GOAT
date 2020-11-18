using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAT.Grid
{
    public class Grid : MonoBehaviour {
        [SerializeField] private Vector2Int gridSize = new Vector2Int(10, 10);
        [SerializeField] private float tileSize = 1.0f;
        public Tile[,] tiles;
        [SerializeField] private LayerMask gridMask;

        private Tile selectedTile;
        private Tile clickedTile;
        [SerializeField] private Transform selectionObject;


        [Space(20)]
        [SerializeField] private bool debugMouseRaycast = false;


        private void Start() {
            transform.localScale = new Vector3(gridSize.x, 0.1f, gridSize.y) * tileSize;
            transform.position = new Vector3(gridSize.x, 0, gridSize.y) * tileSize / 2;

            selectionObject.localScale = new Vector3(tileSize, 1, tileSize);

            InitializeTiles(gridSize, tileSize);
        }

        private void Update() {
            if (DoRaycastFromMouse(out RaycastHit hit)) { 
                Vector2Int tileIndex = CalculateTilePositionInArray(hit.point);
                selectedTile = ReturnTile(tileIndex);

                // Change selection tile when something was selected
                if (selectedTile != null)
                    selectionObject.position = new Vector3(tileIndex.x, 0, tileIndex.y) * tileSize;
            }

            CheckForClickOnTile();
        }

        private void CheckForClickOnTile() {
            if (Input.GetMouseButtonDown(0)) {
                clickedTile?.DeSelect();
                clickedTile = selectedTile;
                clickedTile?.Select();
            }
        }

        private void InitializeTiles(Vector2Int gridSize, float tileSize)
        {
            float tileOffset = tileSize / 2;
            tiles = new Tile[gridSize.x, gridSize.y];
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    tiles[x, y] = new Tile(new Vector3(x * tileSize + tileOffset, 0, y * tileSize + tileOffset));
                }
            }
        }

        private Vector2Int CalculateTilePositionInArray(Vector3 rayHitPosition)
        {
            //tile position gaat altijd X+ Y+ vanuit 
            Vector2 hitPosition = new Vector2(rayHitPosition.x, rayHitPosition.z);

            Vector2 relativeHitPos = hitPosition / tileSize;

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

        /// <summary>
        /// Raycast from the position of the mouse to the world
        /// </summary>
        /// <param name="hit"></param>
        /// <returns> Returns whether it hit something </returns>
        private bool DoRaycastFromMouse(out RaycastHit hit) {
            Vector3 mousePosition = Input.mousePosition + new Vector3(0, 0, Camera.main.nearClipPlane);
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            Vector3 cameraPerspective = mouseWorldPosition - Camera.main.transform.position;

            bool isHitting = Physics.Raycast(mouseWorldPosition, cameraPerspective, out RaycastHit mouseHit, Mathf.Infinity, gridMask);
            hit = mouseHit;
            return isHitting;
        }

        // Debug option for showing where mouse hits the collider
        private void OnDrawGizmos() {
            if (debugMouseRaycast) {
                DoRaycastFromMouse(out RaycastHit hit);

                Gizmos.color = Color.green;
                Gizmos.DrawSphere(hit.point, 1);
                Gizmos.DrawLine(hit.point, Camera.main.transform.position);
            }
        }
    }
}

