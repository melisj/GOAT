using UnityEngine;
using Goat.Grid.UI;
using System;
using Goat.Pooling;
using UnityEngine.EventSystems;
using Goat.Grid.Interactions;

namespace Goat.Grid
{
    public enum SelectionMode
    {
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
        private Tile currentTile;

        public float GetTileSize { get { return tileSize; } }

        // Variables used for highlighting and placing object on grid when in edit mode
        private GameObject previewObject;                               // Preview object shown on grid
        private float objectRotationAngle;                              // Rotation of preview object
                                                                        // private FloorType previewFloorType;
                                                                        // private BuildingType previewBuildingType;
                                                                        // private WallType previewWallType;
        private Placeable previewPlaceable;
        //private TilePartEditing editing = TilePartEditing.None;
        public bool IsEditing;
        private Tile previousTile = null;

        public bool DestroyMode { get; set; }

        [Space(20)]
        public SelectionMode interactionMode = SelectionMode.Edit;

        private void Start()
        {
            InitializeTiles(gridSize, tileSize);

            TileAssets.InitializeAssetsDictionary();

            InputManager.Instance.OnInputEvent += Instance_OnInputEvent;
            InputManager.Instance.InputModeChanged += Instance_InputModeChanged;
        }

        private void Instance_InputModeChanged(object sender, InputMode mode)
        {
            if (previewObject)
            {
                bool inEditMode = mode == InputMode.Edit;
                previewObject.SetActive(inEditMode);
            }
        }

        private void Instance_OnInputEvent(KeyCode keyCode, InputManager.KeyMode keyMode, InputMode inputMode)
        {
            if (inputMode == InputMode.Edit)
            {
                if (keyCode == KeyCode.Mouse0 && keyMode == InputManager.KeyMode.Down)
                {
                    if (currentTile != null)
                    {
                        //TileInformation tileInfo = currentTile.GetTileInformation();
                        if (IsEditing)
                            currentTile.EditAny(previewPlaceable, objectRotationAngle, DestroyMode);
                    }
                }
                if (keyCode == KeyCode.Mouse1 && keyMode == InputManager.KeyMode.Down)
                {
                    // Always has to rotate a 90 degrees
                    objectRotationAngle = (objectRotationAngle + 90) % 360;
                    if (previewObject) previewObject.transform.rotation = Quaternion.Euler(0, objectRotationAngle, 0);
                }
            }
        }

        private void Update()
        {
            EditTile();
        }

        private void EditTile()
        {
            if (InputManager.Instance.InputMode == InputMode.Edit)
            {
                // Highlight tile with selected building/floor
                currentTile = SelectTile();
                HighlightTile(currentTile);
                previousTile = currentTile;
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
                // previousTile.ShowBuilding(true);
                //previousTile.ShowWall(true, (WallPosition)objectRotationAngle);
                //previousTile.ShowAnyWall(true, objectRotationAngle);
            }

            // Hide target object on selected tile
            if (selectedTile != null)
            {
                if (IsEditing)
                {
                    selectedTile.ShowTile(false, objectRotationAngle);
                }
                //if (editing == TilePartEditing.Floor)
                //selectedTile.ShowFloor(false);
                //else if (editing == TilePartEditing.Building)
                //    selectedTile.ShowBuilding(false);
                //else if (editing == TilePartEditing.Wall)
                //    selectedTile.ShowWall(false, (WallPosition)objectRotationAngle);
                // else
                // Debug.LogWarning("Trying to highlight Nothing");
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

        public void DestroyPreview()
        {
            if (previewObject)
            {
                previewObject.SetActive(false);
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

        //===========================================================================================================================================================================================================================================================================

        /// <summary>
        /// Returns tile in grid that is being selected by the mouse
        /// </summary>
        /// <returns></returns>
        private Tile SelectTile()
        {
            Tile selectedTile = null;
            if (InputManager.Instance.DoRaycastFromMouse(out RaycastHit hit, gridMask))
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
    }
}