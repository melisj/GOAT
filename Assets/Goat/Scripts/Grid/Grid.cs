using Goat.Storage;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.Grid
{
    [RequireComponent(typeof(GridDataHandler))]
    public class Grid : MonoBehaviour
    {
        [Header("Generation")]
        [SerializeField] private Wall defaultWall;
        [SerializeField] private Vector2Int gridSize = new Vector2Int(10, 10);
        [SerializeField] private float tileSize = 1.0f;
        private Vector3 startingPosition;
        public Tile[,] tiles;

        [Space(10), Header("Hit Detection")]
        [SerializeField] private LayerMask gridMask;

        // Variables used for highlighting and placing object on grid when in edit mode
        [Space(10), Header("Preview Object")]
        [SerializeField] private Material previewMaterial;
        [SerializeField] private GameObject previewPrefab;
        private GameObject previewObject;              // Preview object shown on grid
        private MeshFilter[] previewObjectMesh;
        private Placeable previewPlaceableInfo;
        private List<Vector2Int> checkedTiles = new List<Vector2Int>();
        private float objectRotationAngle;                              // Rotation of preview object

        private GridDataHandler dataHandler;

        private Tile currentTile;
        private Tile leftTile, rightTile, upTile, downTile;
        private Tile previousTile = null;
        private Vector2Int currentTileIndex;
        private bool autoWalls;

        public bool DestroyMode { get; set; }
        public float GetTileSize { get { return tileSize; } }
        public Vector2Int GetGridSize { get { return gridSize; } }

        private void Start()
        {
            InitializeTiles(gridSize, tileSize);
            InitializePreviewObject();

            dataHandler = GetComponent<GridDataHandler>();
            if(dataHandler)
                dataHandler.LoadGrid();

            InputManager.Instance.OnInputEvent += Instance_OnInputEvent;
            InputManager.Instance.InputModeChanged += Instance_InputModeChanged;
        }

        public void Reset()
        {
            if (tiles != null)
            {
                for (int x = 0; x < gridSize.x; x++)
                {
                    for (int y = 0; y < gridSize.y; y++)
                    {
                        tiles[x, y].ResetPooled();
                    }
                }
            }
        }

        #region Input

        private void Instance_InputModeChanged(object sender, InputMode mode)
        {
            DestroyMode = mode == InputMode.Destroy;
            if (previewObject)
            {
                bool inEditMode = mode == InputMode.Edit;
                if (!inEditMode)
                {
                    currentTile = SelectTile();
                    if (currentTile != null)
                    {
                        currentTile.ShowTile(true, objectRotationAngle);
                    }
                    previousTile = currentTile;
                }
                previewObject.SetActive(inEditMode);
            }
        }

        private void Instance_OnInputEvent(KeyCode keyCode, InputManager.KeyMode keyMode, InputMode inputMode)
        {
            if (inputMode == InputMode.Edit | inputMode == InputMode.Destroy)
            {
                if (keyCode == KeyCode.Mouse0 && keyMode.HasFlag(InputManager.KeyMode.Pressed))
                {
                    if (currentTile != null)
                    {
                        checkedTiles.Clear();
                        currentTile.EditAny(previewPlaceableInfo, objectRotationAngle, DestroyMode);
                        if (autoWalls)
                            SetupNeighborTiles(currentTileIndex);
                    }
                }
                if (keyCode == KeyCode.R && keyMode.HasFlag(InputManager.KeyMode.Down))
                {
                    // Always has to rotate a 90 degrees
                    objectRotationAngle = (objectRotationAngle + 90) % 360;
                    if (previewObject) previewObject.transform.rotation = Quaternion.Euler(0, objectRotationAngle, 0);
                }
                if (keyCode == KeyCode.T && keyMode.HasFlag(InputManager.KeyMode.Down))
                {
                    // Always has to rotate a 90 degrees
                    autoWalls = !autoWalls;
                    if (autoWalls)
                        SetupNeighborTiles(currentTileIndex);
                    Debug.Log("Automode is " + (autoWalls ? "On" : "Off"));
                }
            }
        }

        #endregion Input

        private void ChangeMaterialColor(bool canPlace)
        {
            Color newColor = canPlace ? Color.green : Color.red;
            newColor.a = 0.5f;
            previewMaterial.color = newColor;
        }

        private void Update()
        {
            EditTile();
        }

        private void SetupNeighborTiles(Vector2Int index)
        {
            Tile tileToSet = tiles[index.x, index.y];
            checkedTiles.Add(index);
            CheckNeighbourTiles(tileToSet, index);
        }

        private void CheckNeighbourTiles(Tile tile, Vector2Int index2D)
        {
            int rotation = -90;
            CheckTile(tile, ref rotation, index2D, Vector2Int.down);
            CheckTile(tile, ref rotation, index2D, Vector2Int.left);
            CheckTile(tile, ref rotation, index2D, Vector2Int.up);
            CheckTile(tile, ref rotation, index2D, Vector2Int.right);
        }

        private void CheckTile(Tile tile, ref int rotation, Vector2Int index2D, Vector2Int offset)
        {
            Tile neighbourTile = GetNeighbourTile(index2D + offset);
            Placeable wallPlace = defaultWall;
            rotation += 90;
            if (neighbourTile != null && neighbourTile.FloorObj != null)
            {
                if (!checkedTiles.Contains(neighbourTile.SaveData.gridPosition))
                {
                    SetupNeighborTiles(neighbourTile.SaveData.gridPosition);
                }

                tile.EditAnyWall(wallPlace, rotation, true);

                return;
            }

            if (tile.FloorObj == null)
            {
                tile.EditAnyWall(wallPlace, rotation, true);
            }
            else
            {
                tile.EditAnyWall(wallPlace, rotation, false);
            }

            //Placewalls
        }

        private Tile GetNeighbourTile(Vector2Int index)
        {
            Tile tile = null;
            if (index.x < tiles.GetLength(0) && index.x >= 0 &&
               index.y < tiles.GetLength(0) && index.y >= 0)
                tile = tiles[index.x, index.y];
            return tile;
        }

        private void EditTile()
        {
            if (InputManager.Instance.InputMode == InputMode.Edit | InputManager.Instance.InputMode == InputMode.Destroy)
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
            Material material = GetComponent<Renderer>().material;
            material.mainTextureScale = gridSize;
            startingPosition = transform.parent.position;

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    tiles[x, y] = new Tile(
                        new Vector3(x * tileSize + tileOffset, 0, y * tileSize + tileOffset) + startingPosition,
                        new Vector2Int(x, y),
                        this);
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
            }

            // Hide target object on selected tile
            if (selectedTile != null)
            {
                if (InputManager.Instance.InputMode == InputMode.Edit)
                {
                    ChangeMaterialColor(!selectedTile.CheckForFloor(previewPlaceableInfo));
                    selectedTile.ShowTile(false, objectRotationAngle, previewPlaceableInfo);
                }
            }

            // Selected placingtile on position of tile hit by raycast
            if (previewObject && selectedTile != null)
            {
                EnablePreview(selectedTile.Position);
            }
            else if (previewObject && selectedTile == null)
            {
                DisablePreview();
            }
        }

        #region Preview Functions

        private void InitializePreviewObject()
        {
            //TODO: Prefab
            //previewObject = new GameObject("Preview Object", typeof(MeshFilter), typeof(MeshRenderer));
            previewObject = Instantiate(previewPrefab);
            previewObject.transform.SetParent(transform.parent);
            previewObject.transform.localScale = Vector3.one * tileSize;

            previewObjectMesh = previewObject.GetComponentsInChildren<MeshFilter>();
            for (int i = 0; i < previewObjectMesh.Length; i++)
            {
                previewObjectMesh[i].GetComponent<MeshRenderer>().material = previewMaterial;
            }
        }

        public void EnablePreview(Vector3 position)
        {
            previewObject.transform.position = position;
            previewObject.SetActive(true);
        }

        public void DisablePreview()
        {
            previewObject.SetActive(false);
        }

        public void SetPreviewActiveMesh(Placeable placeable)
        {
            previewObject.SetActive(true);
            for (int i = 0; i < previewObjectMesh.Length; i++)
            {
                if (i >= placeable.Mesh.Length)
                {
                    previewObjectMesh[i].mesh = null;
                    continue;
                }
                previewObjectMesh[i].mesh = placeable.Mesh[i];
            }
        }

        public void ChangePreviewObject(Placeable placeable)
        {
            //Change to pooling if destroy is really destroying
            if (previewPlaceableInfo != placeable)
                previewPlaceableInfo = placeable;

            if (!DestroyMode)
                SetPreviewActiveMesh(placeable);
        }

        #endregion Preview Functions

        #region Tile Functions

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
                currentTileIndex = tileIndex;
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

        #endregion Tile Functions
    }
}