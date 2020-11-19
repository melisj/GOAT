using UnityEngine;
using System;

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
        private GameObject placingObject;
        private FloorType placingFloor;
        private BuildingType placingBuilding;

        [Space(20)]
        [SerializeField] private bool debugMouseRaycast = false;
        public SelectionMode interactionMode = SelectionMode.Universal;

        // UI
        [SerializeField] private GridUIManager UIManager;


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
                    if (!GridUIManager.IsElementSelected())
                    {
                        TileInformation tileInfo = SelectTile().GetTileInformation();
                        GridUIManager.ShowNewUI(UIManager.selectionModeUI);
                        UIManager.selectionModeUI.SetTileInfo(tileInfo);

                        selectionObject.gameObject.SetActive(false);
                    }
                    else if (!GridUIManager.IsSelectedSame(UIManager.tileEditUI))
                    {
                        GridUIManager.HideUI();
                    }
                }
                if (Input.GetMouseButtonDown(1))
                {
                    //right mouse button
                    if (!GridUIManager.IsElementSelected())
                    {

                        Tile tempTile = SelectTile();
                        GridUIManager.ShowNewUI(UIManager.tileEditUI);
                        UIManager.tileEditUI.SetSelectedTile(tempTile);

                        selectionObject.gameObject.SetActive(true);
                        HighlightTile(tempTile);
                    }
                    else
                    {
                        GridUIManager.HideUI();
                    }
                }
            }
            else
            {
                if (interactionMode == SelectionMode.Select && Input.GetMouseButtonDown(0))
                {
                    Tile tempTile = SelectTile();
                    TileInformation tempTileInformation = tempTile.GetTileInformation();
                    if (tempTileInformation.floorType != FloorType.Empty)
                    {
                        if (tempTileInformation.buildingType == BuildingType.Empty)
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
                    Tile tempTile = SelectTile();
                    HighlightTile(tempTile);

                    if (Input.GetMouseButtonDown(0))
                    {
                        // Check new old tile type vs new tile type

                        // Check if floor or building should be changed
                    }
                }
            }
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

        /// <summary>
        /// Visualises which tile is being raycast.
        /// </summary>
        /// <param name="selectedTile"> Tile being raycast.</param>
        private void HighlightTile(Tile selectedTile)
        {
            // Change selection tile when something was selected
            if (selectedTile != null)
            {
                selectionObject.position = selectedTile.GetTileInformation().TilePosition;
            }

            if (selectedTile != null && placingObject != null)
            {
                placingObject.transform.position = selectedTile.GetTileInformation().TilePosition;
            }
            else if(selectedTile == null && placingObject != null)
            {
                placingObject.transform.position = new Vector3(0,200,0);
            }
        }

        /// <summary>
        /// Instantiate a new gameobject al highlight object which is a preview of the object to be placed on the highlighted tile.
        /// Call with UI buttons in edit mode.
        /// </summary>
        /// <param name="isFloor"> Is tile to be placed a floor or building</param>
        /// <param name="type"> Int pointing to enum </param>
        public void SetSelectionObject(bool isFloor, int type)
        {
            if (placingObject != null) Destroy(placingObject);
            GameObject tempObject = null;
            if (isFloor)
            {
                placingFloor = (FloorType)type;
                tempObject = TileAssets.FindAsset(placingFloor);
            }
            else
            {
                placingBuilding = (BuildingType)type;
                tempObject = TileAssets.FindAsset(placingBuilding);             
            }
            placingObject = tempObject != null ? Instantiate(tempObject, new Vector3(0, 0, 200), Quaternion.identity) : null;
        }

        /// <summary>
        /// Returns tile in grid that is being selected by the mouse
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Returns a Vector2 int which points to a position in the 2D Array of tiles.
        /// Casts position.x and position.y to int taking into account the tile size and grid origin.
        /// </summary>
        /// <param name="rayHitPosition"> Hit point of ray on collider of the grid.</param>
        /// <returns></returns>
        private Vector2Int CalculateTilePositionInArray(Vector3 rayHitPosition)
        {
            Vector2 gridPositionOffset = new Vector2(startingPosition.x, startingPosition.z);
            Vector2 hitPosition = new Vector2(rayHitPosition.x, rayHitPosition.z);

            Vector2 relativeHitPos = (hitPosition / tileSize) - gridPositionOffset;

            return new Vector2Int(Mathf.FloorToInt(relativeHitPos.x), Mathf.FloorToInt(relativeHitPos.y));
        }

        /// <summary>
        /// Looks up tile in array and returns it.
        /// </summary>
        /// <param name="tilePositionInArray"> Vector2Int which points to location in 2D Array of tiles.</param>
        /// <returns></returns>
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

