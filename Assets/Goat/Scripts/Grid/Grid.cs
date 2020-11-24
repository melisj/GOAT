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

    public enum TilePartEditing
    {
        None,
        Floor,
        Building,
        Wall
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
        private GameObject previewObject;                               // Preview object shown on grid
        private Quaternion previewObjectRotation;
        private FloorType previewFloorType;
        private BuildingType previewBuildingType;
        private WallType previewWallType;
        private TilePartEditing editing = TilePartEditing.None;
        bool editingFloor;
        private Tile previousTile = null;

        [Space(20)]
        [SerializeField] private bool debugMouseRaycast = false;
        public SelectionMode interactionMode = SelectionMode.Universal;

        // Managers
        [SerializeField] private GridUIManager UIManager;
        [SerializeField] private InteractableManager interactableManager;


        private void Start()
        {
            selectionObject.localScale = new Vector3(tileSize, 1, tileSize);

            InitializeTiles(gridSize, tileSize);

            TileAssets.InitializeAssetsDictionary();
        }

        private void Update()
        {
            // Change spaghet after test which spaghet is best for editing
            if(Input.GetKeyDown(KeyCode.C)) {
                if (interactionMode != SelectionMode.Universal)
                    interactionMode = SelectionMode.Universal;
                else
                    interactionMode = SelectionMode.Select;
            }

            if (interactionMode == SelectionMode.Universal)
            {
                //Universal Mode
                UIManager.editModeUI.ToggleSwitchButton(false);

                //left mouse button
                if (Input.GetMouseButtonDown(0))
                {

                    if (!GridUIManager.IsElementSelected())
                    {
                        interactableManager.CheckForInteractable();

                        Tile tempTile = SelectTile();
                        if (tempTile != null)
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
                //Editing/Seleting Mode
                UIManager.editModeUI.ToggleSwitchButton(true);
                selectionObject.gameObject.SetActive(false);

                if (interactionMode == SelectionMode.Select && Input.GetMouseButtonDown(0))
                {
                    Tile tempTile = SelectTile();
                    //left mouse button
                    if (!GridUIManager.IsElementSelected())
                    {
                        interactableManager.CheckForInteractable();

                        if (tempTile != null)
                            selectionObject.gameObject.SetActive(false);
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
                        if (editingFloor && tempTile.GetTileInformation().floorType != previewFloorType)
                        {
                            tempTile.EditFloor(previewFloorType);
                        }
                        else if (!editingFloor && tempTile.GetTileInformation().buildingType != previewBuildingType)
                        {
                            tempTile.EditBuilding(previewBuildingType, previewObjectRotation);
                        }
                    }
                    if(Input.GetMouseButtonDown(1) && previewObject)
                    {
                        previewObject.transform.Rotate(new Vector3(0, 45, 0), Space.World);
                        previewObjectRotation = previewObject.transform.rotation;
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
            if (previewObject != null && selectedTile != null)
            {
                previewObject.transform.position = selectedTile.GetTileInformation().TilePosition;
                previewObject.SetActive(true);
            }
            else if (selectedTile == null && previewObject != null)
            {
                previewObject.SetActive(false);
            }
        }

        //===========================================================================================================================================================================================================================================================================

        public void ChangePreviewObject(bool _editingFloor, int type)
        {
            GameObject tempObject = null;
            // If we go from edit floor to edit building destroy preview object
            if(editingFloor != _editingFloor)
            {
                editingFloor = _editingFloor;
                previewBuildingType = BuildingType.Empty;
                previewFloorType = FloorType.Empty;
                if (previewObject) Destroy(previewObject);
            }

            // If we are still editing the floor but select a different FloorType the preview object needs to be replaced.
            if (editingFloor && previewFloorType != (FloorType)type)
            {
                if (previewObject) Destroy(previewObject);
                previewFloorType = (FloorType)type;
                tempObject = TileAssets.FindAsset(previewFloorType);
            }
            // If we are still editing the building but select a different BuildingType the preview object needs to be replaced.
            else if (!editingFloor && previewBuildingType != (BuildingType)type)
            {
                if (previewObject) Destroy(previewObject);
                previewBuildingType = (BuildingType)type;
                tempObject = TileAssets.FindAsset(previewBuildingType);
            }

            // If the temp object we selected != null instantiate it as previewObject.
            if (tempObject)
            {
                previewObject = Instantiate(tempObject, new Vector3(0, 200, 0), Quaternion.identity);
                previewObject.transform.localScale = Vector3.one * tileSize;
            }
        }

        public void ChangePreviewObject(TilePartEditing _editing, int type)
        {
            GameObject tempObject = null;
            // If we start editing an other part of the tile.
            if(editing != _editing)
            {
                editing = _editing;
                if (previewObject) Destroy(previewObject);
                previewFloorType = FloorType.Empty;
                previewBuildingType = BuildingType.Empty;
                previewWallType = WallType.Empty;
            }

            // If we are still editing the floor but select a different FloorType the preview object needs to be replaced.
            if (editing == TilePartEditing.Floor && (FloorType)type != previewFloorType)
            {
                if (previewObject) Destroy(previewObject);
                previewFloorType = (FloorType)type;
                tempObject = TileAssets.FindAsset(previewFloorType);
            }
            //If we are still editing the building but select a different BuildingType the preview object needs to be replaced.
            else if(editing == TilePartEditing.Building && (BuildingType)type != previewBuildingType)
            {
                if (previewObject) Destroy(previewObject);
                previewBuildingType = (BuildingType)type;
                tempObject = TileAssets.FindAsset(previewBuildingType);
            }
            // If we are still editing the wall but select a different WallType the preview object needs to be replaced.
            else if (editing == TilePartEditing.Wall && (WallType)type != previewWallType)
            {
                if (previewObject) Destroy(previewObject);
                previewWallType = (WallType)type;
                tempObject = TileAssets.FindAsset(previewWallType);
            }

            // If the temp object we selected != null instantiate it as previewObject.
            if (tempObject)
            {
                previewObject = Instantiate(tempObject, new Vector3(0, 200, 0), Quaternion.identity);
                previewObject.transform.localScale = Vector3.one * tileSize;
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
            ChangePreviewObject(true, 0);
            ChangePreviewObject(false, 0);
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
        public static bool DoRaycastFromMouse(out RaycastHit hit)
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

