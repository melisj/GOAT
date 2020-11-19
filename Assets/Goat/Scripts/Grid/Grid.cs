using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAT.Grid
{
    public enum SelectionMode
    {
        Universal,
        Edit,
        Select
    }

    public class Grid : MonoBehaviour
    {
        [SerializeField] private Vector2Int gridSize = new Vector2Int(10, 10);
        [SerializeField] private float tileSize = 1.0f;
        private Vector3 startingPosition;
        public Tile[,] tiles;
        [SerializeField] private LayerMask gridMask;

        private Tile clickedTile;
        [SerializeField] private Transform selectionObject;

        [Space(20)]
        [SerializeField] private bool debugMouseRaycast = false;
        public SelectionMode interactionMode = SelectionMode.Universal;

        public FloorType floorType;

        // UI
        [SerializeField] private TileEditUI tileEditUI;
        [SerializeField] private SelectionModeUI selectionModeUI;


        private void Start()
        {
            selectionObject.localScale = new Vector3(tileSize, 1, tileSize);

            InitializeTiles(gridSize, tileSize);

            TileAssets.InitializeAssetsDictionary();
        }

        private void Update()
        {
            // Check if the selection object is needed
            //bool isSelectionObjectVisible = (interactionMode == InteractionMode.UI) && (interactionMode == SelectionMode.Edit);
            //selectionObject.gameObject.SetActive(isSelectionObjectVisible);

            if (interactionMode == SelectionMode.Universal)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    //left mouse button
                    Tile tempTile = SelectTile();
                    selectionModeUI.ShowTileInfo(tempTile.GetTileInformation());

                    tileEditUI.HideUI();
                }
                if (Input.GetMouseButtonDown(1))
                {
                    //right mouse button
                    Tile tempTile = SelectTile();
                    tileEditUI.ShowUI(tempTile);

                    selectionModeUI.HideTileInfo();
                }
            }
            else
            {
                if (interactionMode == SelectionMode.Select && Input.GetMouseButtonDown(0))
                {
                    Tile tempTile = SelectTile();
                    TileInformation tempTileInformation = tempTile.GetTileInformation();
                    if(tempTileInformation.floorType != FloorType.Empty)
                    {
                        if(tempTileInformation.buildingType == BuildingType.Empty)
                        {
                            // Navigate to tile
                        }
                        else
                        {
                            // Show tile information
                            Debug.Log(tempTileInformation);
                        }
                    }
                }
                else if (interactionMode == SelectionMode.Edit)
                {
                    // Highlight tile with selected building/floor
                    if (Input.GetMouseButtonDown(0))
                    {

                    }
                }
            }


            /*
            if (interactionMode == InteractionMode.Mouse) {
                if (Input.GetMouseButtonDown(0)) {
                    //left mouse button
                    Tile tempTile = SelectTile();
                    tempTile?.Select();
                }
                if (Input.GetMouseButtonDown(1)) {
                    //right mouse button
                    Tile tempTile = SelectTile();
                    tempTile?.EditFloor(floorType);
                }
            } else if (interactionMode == InteractionMode.UI) {
                if (Input.GetMouseButtonDown(0)) {
                    //left mouse button
                    Tile tempTile = SelectTile();

                    if (selectionMode == SelectionMode.Edit) {
                        tempTile?.EditFloor(floorType);
                    } else if (selectionMode == SelectionMode.Select) {
                        tempTile?.Select();
                    }
                }

                // Highlight tiles when in edit mode.
                // This is useful when selecting neighboring tiles without a visible border
                if (selectionMode == SelectionMode.Edit)
                    HighlightTile();
            }*/

            /*// Highlight tiles when in edit mode.
            // This is useful when selecting neighboring tiles without a visible border
            if (selectionMode == SelectionMode.Edit) HighlightTile();

            if (Input.GetMouseButtonDown(0))
            {
                //left mouse button
                Tile tempTile = SelectTile();
            }
            if (Input.GetMouseButtonDown(1) && selectionMode != SelectionMode.Edit)
            {
                //right mouse button
                Tile tempTile = SelectTile();
                tempTile?.EditFloor(floorType);
            }*/
        }
        private void InitializeTiles(Vector2Int gridSize, float tileSize)
        {
            float tileOffset = tileSize / 2;
            tiles = new Tile[gridSize.x, gridSize.y];
            startingPosition = transform.parent.position;

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    tiles[x, y] = new Tile(new Vector3(x * tileSize + tileOffset, 0, y * tileSize + tileOffset) + startingPosition);
                }
            }

            transform.localScale = new Vector3(gridSize.x, 0.1f, gridSize.y) * tileSize;
            transform.localPosition = new Vector3(gridSize.x, 0, gridSize.y) * tileSize / 2;
        }

        private void HighlightTile()
        {
            Tile selectedTile = SelectTile();

            // Change selection tile when something was selected
            if (selectedTile != null)
            {
                selectionObject.position = selectedTile.GetTileInformation().TilePosition;
            }
        }

        private Tile SelectTile()
        {
            Tile selectedTile = null;
            if (DoRaycastFromMouse(out RaycastHit hit))
            {
                Vector2Int tileIndex = CalculateTilePositionInArray(hit.point);
                selectedTile = ReturnTile(tileIndex);
            }
            return selectedTile;
        }

        private Vector2Int CalculateTilePositionInArray(Vector3 rayHitPosition)
        {
            Vector2 gridPositionOffset = new Vector2(startingPosition.x, startingPosition.z);
            Vector2 hitPosition = new Vector2(rayHitPosition.x, rayHitPosition.z);

            Vector2 relativeHitPos = (hitPosition / tileSize) - gridPositionOffset;

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
        private bool DoRaycastFromMouse(out RaycastHit hit)
        {
            Vector3 mousePosition = Input.mousePosition + new Vector3(0, 0, Camera.main.nearClipPlane);
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            Vector3 cameraPerspective = mouseWorldPosition - Camera.main.transform.position;

            bool isHitting = Physics.Raycast(mouseWorldPosition, cameraPerspective, out RaycastHit mouseHit, Mathf.Infinity, gridMask);
            hit = mouseHit;
            return isHitting;
        }

        // Debug option for showing where mouse hits the collider
        private void OnDrawGizmos()
        {
            if (debugMouseRaycast)
            {
                DoRaycastFromMouse(out RaycastHit hit);

                Gizmos.color = Color.green;
                Gizmos.DrawSphere(hit.point, 1);
                Gizmos.DrawLine(hit.point, Camera.main.transform.position);
            }
        }
    }
}

