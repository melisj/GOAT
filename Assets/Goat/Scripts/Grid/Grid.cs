using UnityEngine;
using GOAT.Grid.UI;
using System;
using UnityEngine.EventSystems;

namespace GOAT.Grid
{
    public enum SelectionMode
    {
        Universal,
        Edit,
        Select
    }

    public class Grid : MonoBehaviour {
        [SerializeField] private Vector2Int gridSize = new Vector2Int(10, 10);
        [SerializeField] private float tileSize = 1.0f;
        private Vector3 startingPosition;
        public Tile[,] tiles;
        [SerializeField] private LayerMask gridMask;

        public float GetTileSize { get { return tileSize; } }

        private Tile clickedTile;
        [SerializeField] private Transform selectionObject;

        // Variables used for highlighting and placing object on grid when in edit mode
        private GameObject placingObject;
        private FloorType placingFloorType;
        private BuildingType placingBuildingType;
        bool editingFloor;
        private Tile previousTile = null;

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
            if(Input.GetKeyDown(KeyCode.C)) {
                if (interactionMode != SelectionMode.Universal)
                    interactionMode = SelectionMode.Universal;
                else
                    interactionMode = SelectionMode.Select;
            }

            if (interactionMode == SelectionMode.Universal)
            {
                UIManager.editModeUI.ToggleSwitchButton(false);

                //left mouse button
                if (Input.GetMouseButtonDown(0))
                {

                    if (!GridUIManager.IsElementSelected())
                    {
                        Tile tempTile = SelectTile();
                        if (tempTile != null)
                        {
                            TileInformation tileInfo = tempTile.GetTileInformation();
                            GridUIManager.ShowNewUI(UIManager.selectionModeUI);
                            UIManager.selectionModeUI.SetTileInfo(tileInfo);

                            selectionObject.gameObject.SetActive(false);
                        }
                    }
                    else if (!GridUIManager.IsSelectedSame(UIManager.tileEditUI))
                    {
                        GridUIManager.HideUI();
                    }
                }
                if (Input.GetMouseButtonDown(1))
                {
                    //right mouse button
                    selectionObject.gameObject.SetActive(false);

                    if (!GridUIManager.IsElementSelected())
                    {

                        Tile tempTile = SelectTile();
                        GridUIManager.ShowNewUI(UIManager.tileEditUI);
                        UIManager.tileEditUI.SetSelectedTile(tempTile);

                        selectionObject.gameObject.SetActive(true);
                        if (tempTile != null)
                            selectionObject.position = tempTile.GetTileInformation().TilePosition;
                    } else
                    {
                        GridUIManager.HideUI();
                    }
                }
            }
            else
            {
                UIManager.editModeUI.ToggleSwitchButton(true);
                selectionObject.gameObject.SetActive(false);

                if (interactionMode == SelectionMode.Select && Input.GetMouseButtonDown(0))
                {
                    Tile tempTile = SelectTile();
                    //left mouse button
                    if (!GridUIManager.IsElementSelected())
                    {
                        if (tempTile != null)
                        {
                            TileInformation tileInfo = tempTile.GetTileInformation();
                            GridUIManager.ShowNewUI(UIManager.selectionModeUI);
                            UIManager.selectionModeUI.SetTileInfo(tileInfo);

                            selectionObject.gameObject.SetActive(false);
                        }
                    }
                    else if (!GridUIManager.IsSelectedSame(UIManager.tileEditUI))
                    {
                        GridUIManager.HideUI();
                    }
                }
                else if (interactionMode == SelectionMode.Edit)
                {
                    // Highlight tile with selected building/floor
                    Tile tempTile = SelectTile();
                    HighlightTile(tempTile);
                    previousTile = tempTile;

                    if (Input.GetMouseButtonDown(0) && tempTile != null)
                    {
                        // Check new old tile type vs new tile type
                        if (editingFloor && tempTile.GetTileInformation().floorType != placingFloorType)
                        {
                            tempTile.EditFloor(placingFloorType);
                        }
                        else if (!editingFloor && tempTile.GetTileInformation().buildingType != placingBuildingType)
                        {
                            tempTile.EditBuilding(placingBuildingType);
                        }
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
                    tiles[x, y] = new Tile(new Vector3(x * tileSize + tileOffset, 0, y * tileSize + tileOffset) + startingPosition, this);
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
            //

            // Show/Hide objects on selected tile
            if (previousTile != null)
            {
                previousTile.ShowFloor(true);
                previousTile.ShowBuilding(true);
            }
            if (selectedTile != null)
            {
                if (editingFloor)
                    selectedTile.ShowFloor(false);
                else
                    selectedTile.ShowBuilding(false);
            }

            // Selected placingtile on position of tile hit by raycast
            if (placingObject != null && selectedTile != null)
            {
                placingObject.transform.position = selectedTile.GetTileInformation().TilePosition;
                placingObject.SetActive(true);
            }
            else if (selectedTile == null && placingObject != null)
            {
                placingObject.SetActive(false);
            }
        }

        //===========================================================================================================================================================================================================================================================================


        /// <summary>
        /// Instantiate a new gameobject al highlight object which is a preview of the object to be placed on the highlighted tile.
        /// Call with UI buttons in edit mode.
        /// </summary>
        /// <param name="type"> Int pointing to enum </param>
        public void SetSelectionBuilding(int type)
        {
            editingFloor = false;
            GameObject tempObject = null;

            if ((BuildingType)type == placingBuildingType) return;

            placingBuildingType = (BuildingType)type;
            if (placingObject != null) Destroy(placingObject);

            tempObject = TileAssets.FindAsset(placingBuildingType);

            if (tempObject != null)
            {
                placingObject = Instantiate(tempObject, new Vector3(0, 0, 200), Quaternion.identity);
                placingObject.transform.localScale = tileSize * Vector3.one;
            }
        }
        public void SetSelectionFloor(int type)
        {
            editingFloor = true;
            GameObject tempObject = null;

            if ((FloorType)type == placingFloorType) return;

            if (placingObject != null) Destroy(placingObject);
            placingFloorType = (FloorType)type;
            tempObject = TileAssets.FindAsset(placingFloorType);

            if (tempObject != null)
            {
                placingObject = Instantiate(tempObject, new Vector3(0, 0, 200), Quaternion.identity);
                placingObject.transform.localScale = tileSize * Vector3.one;
            }
        }
        public void EnterExitEditMode()
        {
            if (interactionMode != SelectionMode.Edit)
            {
                interactionMode = SelectionMode.Edit;
                GridUIManager.ShowNewUI(UIManager.editModeUI);
            }
            else
            {
                interactionMode = SelectionMode.Select;
                GridUIManager.HideUI();
            }
            SetSelectionFloor(0);
            SetSelectionBuilding(0);
        }

        //===========================================================================================================================================================================================================================================================================

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

            Vector2 relativeHitPos = (hitPosition - gridPositionOffset) / tileSize;

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

            bool isHitting = Physics.Raycast(mouseWorldPosition, cameraPerspective, out RaycastHit mouseHit, Mathf.Infinity);
            hit = mouseHit;

            if (EventSystem.current.IsPointerOverGameObject())
                return false;
            return isHitting;
        }

        //===========================================================================================================================================================================================================================================================================

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

