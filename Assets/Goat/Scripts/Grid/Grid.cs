﻿using UnityEngine;
using Goat.Grid.UI;
using System;
using Goat.Pooling;
using UnityEngine.EventSystems;
using Goat.Grid.Interactions;

namespace Goat.Grid
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

    public class Grid : MonoBehaviour
    {
        [SerializeField] private Vector2Int gridSize = new Vector2Int(10, 10);
        [SerializeField] private float tileSize = 1.0f;
        private Vector3 startingPosition;
        public Tile[,] tiles;
        [SerializeField] private LayerMask gridMask;

        public float GetTileSize { get { return tileSize; } }

        [SerializeField] private Transform selectionObject;

        // Variables used for highlighting and placing object on grid when in edit mode
        private GameObject previewObject;                               // Preview object shown on grid
        private float objectRotationAngle;                              // Rotation of preview object
        private FloorType previewFloorType;
        private BuildingType previewBuildingType;
        private WallType previewWallType;
        private Placeable previewPlaceable;
        private TilePartEditing editing = TilePartEditing.None;
        public bool IsEditing;
        private Tile previousTile = null;

        public bool DestroyMode { get; set; }

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
            if (Input.GetKeyDown(KeyCode.C))
            {
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
                    }
                    else
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
                    else if (!GridUIManager.IsSelectedSame(UIManager.tileEditUI) && !EventSystem.current.IsPointerOverGameObject())
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
                        TileInformation tileInfo = tempTile.GetTileInformation();
                        // Check new old tile type vs new tile type

                        if (IsEditing)
                        {
                            tempTile.EditAny(previewPlaceable, objectRotationAngle, DestroyMode);
                        }

                        if (editing == TilePartEditing.Floor)
                        {
                            tempTile.EditFloor(previewFloorType, objectRotationAngle);
                        }
                        else if (editing == TilePartEditing.Building)
                        {
                            tempTile.EditBuilding(previewBuildingType, objectRotationAngle);
                        }
                        else if (editing == TilePartEditing.Wall)
                        {
                            tempTile.EditWall(previewWallType, (WallPosition)objectRotationAngle);
                        }
                    }
                    if (Input.GetMouseButtonDown(1))
                    {
                        // Always has to rotate a 90 degrees
                        objectRotationAngle = (objectRotationAngle + 90) % 360;
                        if (previewObject) previewObject.transform.rotation = Quaternion.Euler(0, objectRotationAngle, 0);
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
            // Show al objects on previous tile
            if (previousTile != null)
            {
                previousTile.ShowTile(true, objectRotationAngle);
                previousTile.ShowFloor(true);
                previousTile.ShowBuilding(true);
                previousTile.ShowWall(true, (WallPosition)objectRotationAngle);
                //previousTile.ShowAnyWall(true, objectRotationAngle);
            }

            // Hide target object on selected tile
            if (selectedTile != null)
            {
                if (IsEditing)
                {
                    selectedTile.ShowTile(false, objectRotationAngle);
                }
                if (editing == TilePartEditing.Floor)
                    selectedTile.ShowFloor(false);
                else if (editing == TilePartEditing.Building)
                    selectedTile.ShowBuilding(false);
                else if (editing == TilePartEditing.Wall)
                    selectedTile.ShowWall(false, (WallPosition)objectRotationAngle);
                else
                    Debug.LogWarning("Trying to highlight Nothing");
            }

            // Selected placingtile on position of tile hit by raycast
            if (previewObject && selectedTile != null)
            {
                previewObject.transform.position = selectedTile.GetTileInformation().TilePosition;
                previewObject.SetActive(true);
            }
            else if (selectedTile == null && previewObject)
            {
                previewObject.SetActive(false);
            }
        }

        //===========================================================================================================================================================================================================================================================================
        public void ChangePreviewObject(TilePartEditing _editing, int type)
        {
            GameObject tempObject = null;
            // If we start editing an other part of the tile.
            if (editing != _editing)
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
            else if (editing == TilePartEditing.Building && (BuildingType)type != previewBuildingType)
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
                previewObject = Instantiate(tempObject, new Vector3(0, 200, 0), Quaternion.Euler(0, objectRotationAngle, 0));
                previewObject.transform.localScale = Vector3.one * tileSize;
            }
        }

        public void DestroyPreview()
        {
            if (previewObject)
            {
                Destroy(previewObject);
            }
        }

        public void ChangePreviewObject(Placeable placeable)
        {
            //Change to pooling if destroy is really destroying
            IsEditing = true;
            if (previewPlaceable != placeable)
            {
                previewPlaceable = placeable;

                if (previewObject) Destroy(previewObject);
            }
            if (previewObject) Destroy(previewObject);

            if (!DestroyMode)
            {
                previewObject = Instantiate(placeable.Prefab, new Vector3(0, 200, 0), Quaternion.Euler(0, objectRotationAngle, 0));
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
            ChangePreviewObject(TilePartEditing.None, 0);
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

        //
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