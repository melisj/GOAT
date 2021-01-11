using Goat.Events;
using Goat.Saving;
using System.Collections.Generic;
using System.IO;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Goat.Grid
{
    [RequireComponent(typeof(DataHandler))]
    public class Grid : EventListenerKeyCodeModeEvent
    {
        [Header("Generation")]
        [SerializeField] private Wall defaultWall;
        [SerializeField] private GameObject gridPlane;
        [SerializeField] private Vector2Int gridSize = new Vector2Int(10, 10);
        [SerializeField] private float tileSize = 1.0f;
        private Vector3 startingPosition;
        [HideInInspector] public Tile[,] tiles;

        [Space(10), Header("Hit Detection")]
        [SerializeField] private LayerMask gridMask;

        // Variables used for highlighting and placing object on grid when in edit mode
        [Space(10), Header("Preview Object")]
        [SerializeField] private Placeable defaultPlaceable;
        [SerializeField] private Material previewMaterial;
        [SerializeField] private GameObject previewPrefab;
        [Header("Event")]
        [SerializeField] private InputModeVariable currentMode;
        [SerializeField] private GridRayCaster gridRayCaster;
        [SerializeField] private VoidEvent onTileDestroyed, onTileCreated, onGridReset;
        private GameObject previewObject;              // Preview object shown on grid
        private MeshFilter[] previewObjectMesh;
        private Placeable previewPlaceableInfo;
        private List<Vector2Int> checkedTiles = new List<Vector2Int>();
        private float objectRotationAngle;                              // Rotation of preview object

        private Tile currentTile;
        private Tile leftTile, rightTile, upTile, downTile;
        private Tile previousTile = null;
        private Placeable previousAutoPlaceable = null;

        private Vector2Int currentTileIndex;
        private bool autoWalls;
        private bool fill;
        public bool DestroyMode { get; set; }
        public float GetTileSize { get { return tileSize; } }
        public Vector2Int GetGridSize { get { return gridSize; } }

        private void Awake()
        {
            InitializeTiles(gridSize, tileSize);
            InitializePreviewObject();

            //InputManager.Instance.OnInputEvent += Instance_OnInputEvent;
            //InputManager.Instance.InputModeChanged += Instance_InputModeChanged;
        }

        public void Reset()
        {
            Debug.Log("Reset");
            if (tiles != null)
            {
                for (int x = 0; x < gridSize.x; x++)
                {
                    for (int y = 0; y < gridSize.y; y++)
                    {
                        tiles[x, y].ResetPooled();
                    }
                }
                onGridReset.Raise();
            }
        }

        #region Input

        public override void OnEventRaised(KeyCodeMode value)
        {
            KeyCode code = KeyCode.None;
            KeyMode mode = KeyMode.None;

            value.Deconstruct(out code, out mode);

            OnInput(code, mode);
        }

        public void DestroyModeChange(InputMode mode)
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

        private void OnInput(KeyCode keyCode, KeyMode keyMode)
        {
            if (currentMode.InputMode == InputMode.Edit | currentMode.InputMode == InputMode.Destroy)
            {
                if (keyCode == KeyCode.Mouse0 && (DestroyMode ? keyMode.HasFlag(KeyMode.Down) : keyMode.HasFlag(KeyMode.Pressed)))
                {
                    if (currentTile != null)
                    {
                        checkedTiles.Clear();
                        if (currentTile.EditAny(previewPlaceableInfo, objectRotationAngle, DestroyMode)
                            && !(previewPlaceableInfo is Wall))
                        {
                            //if (previewPlaceableInfo != previousAutoPlaceable)
                            SetupNeighborTiles(currentTileIndex);
                        }

                        previousAutoPlaceable = previewPlaceableInfo;
                    }
                }
                if (keyCode == KeyCode.R && keyMode.HasFlag(KeyMode.Down))
                {
                    // Always has to rotate a 90 degrees
                    objectRotationAngle = (objectRotationAngle + 90) % 360;
                    if (previewObject) previewObject.transform.rotation = Quaternion.Euler(0, objectRotationAngle, 0);
                }
            }
        }

        private void OnInput()
        {
            if (currentMode.InputMode == InputMode.Edit | currentMode.InputMode == InputMode.Destroy)
            {
                //#if UNITY_EDITOR || DEVELOPMENT_BUILD
                //                if (Input.GetMouseButtonDown(0))
                //                {
                //                    if (fill)
                //                    {
                //                        FillGrid();
                //                        return;
                //                    }
                //                }
                //#endif
                bool createdTile = false;
                if ((DestroyMode ? Input.GetMouseButtonDown(0) : Input.GetMouseButton(0)))
                {
                    if (currentTile != null)
                    {
                        checkedTiles.Clear();
                        PlaceableInfo placeableInfo = currentTile.GetPlaceableInfo();
                        Placeable currentPlaceable = null;
                        if (placeableInfo)
                        {
                            currentPlaceable = placeableInfo.Placeable;
                        }
                        if (currentTile.EditAny(previewPlaceableInfo, objectRotationAngle, DestroyMode))
                        {
                            if (DestroyMode)
                                onTileDestroyed.Raise();
                            else if (!createdTile)
                                onTileCreated.Raise();
                            if (previewPlaceableInfo != null && (!previewPlaceableInfo.CreatesWallsAround & !DestroyMode)) return;
                            if (previewPlaceableInfo is Wall) return;
                            if (currentPlaceable != null)
                            {
                                Debug.Log($"{currentPlaceable} -- {currentPlaceable.CreatesWallsAround} && {DestroyMode}");
                                if (!currentPlaceable.CreatesWallsAround && DestroyMode) return;
                            }
                            //if (previewPlaceableInfo != previousAutoPlaceable)
                            SetupNeighborTiles(currentTileIndex);
                        }
                        previousAutoPlaceable = previewPlaceableInfo;
                        createdTile = true;
                    }
                }
                if (Input.GetKeyDown(KeyCode.R))
                {
                    // Always has to rotate a 90 degrees
                    objectRotationAngle = (objectRotationAngle + 90) % 360;
                    if (previewObject) previewObject.transform.rotation = Quaternion.Euler(0, objectRotationAngle, 0);
                }
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                if (Input.GetKeyDown(KeyCode.F))
                {
                    //fill = !fill;
                    FillGrid();
                    Debug.LogWarning($"FILL MODE = {fill}");
                }
#endif
            }
        }

        private void FillGrid()
        {
            Debug.LogError("you shouldn't be here");
            ResourceTileData[] datas = Resources.LoadAll<ResourceTileData>("ResourceTiles");
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    int random = Random.Range(0, 4);
                    int setTileRandom = Random.Range(0, 101);
                    int chanceToSet = Random.Range(0, 101);
                    int chanceToSpawn = 30;
                    int rotation = 90 * random;
                    if (!DestroyMode)
                    {
                        if (chanceToSet > chanceToSpawn) continue;
                        if (tiles[x, y].FloorObj) continue;
                    }
                    ResourceTileData data = GetNearestData(datas, setTileRandom);
                    if (tiles[x, y].EditAny(data, rotation, DestroyMode))
                    {
                        if (!data.CreatesWallsAround) continue;
                        SetupNeighborTiles(new Vector2Int(x, y));
                    }
                }
            }
        }

        private ResourceTileData GetNearestData(ResourceTileData[] data, int value)
        {
            int currentNearest = 0;
            float currentDiff = Mathf.Abs(data[0].ChanceToSpawn - value);
            for (int i = 0; i < data.Length; i++)
            {
                float diff = Mathf.Abs(data[i].ChanceToSpawn - value);
                if (diff < currentDiff)
                {
                    currentDiff = diff;
                    currentNearest = i;
                }
            }

            return data[currentNearest];
        }

        #endregion Input

        private void ChangeMaterialColor(bool canPlace, bool destroyMode)
        {
            Color newColor = canPlace ? destroyMode ? Color.white : Color.green : Color.red;
            newColor.a = 0.5f;
            previewMaterial.color = newColor;
        }

        private void Update()
        {
            EditTile();
            OnInput();
        }

        private void SetupNeighborTiles(Vector2Int index)
        {
            Tile tileToSet = tiles[index.x, index.y];
            checkedTiles.Add(index);
            CheckNeighbourTiles(tileToSet, index);
        }

        private bool CanPayAuto(Vector2Int index)
        {
            checkedTiles.Clear();
            SetupNeighborTilesWithoutEditing(index);
            Placeable wallPlace = previewPlaceableInfo is Wall ? previewPlaceableInfo : defaultWall;
            float total = wallPlace.Price * (checkedTiles.Count * 2);
            return wallPlace.Money.Amount > total;
        }

        private void SetupNeighborTilesWithoutEditing(Vector2Int index)
        {
            Tile tileToSet = tiles[index.x, index.y];
            checkedTiles.Add(index);
            CheckNeighbourTilesWithoutEditing(tileToSet, index);
        }

        private void AutoPayment()
        {
            if (checkedTiles.Count > 0)
            {
                Placeable wallPlace = previewPlaceableInfo is Wall ? previewPlaceableInfo : defaultWall;

                wallPlace.Buy(checkedTiles.Count * 2);
            }
        }

        private void CheckNeighbourTiles(Tile tile, Vector2Int index2D)
        {
            int rotation = -90;
            CheckTile(tile, ref rotation, index2D, Vector2Int.down);
            CheckTile(tile, ref rotation, index2D, Vector2Int.left);
            CheckTile(tile, ref rotation, index2D, Vector2Int.up);
            CheckTile(tile, ref rotation, index2D, Vector2Int.right);
        }

        private void CheckNeighbourTilesWithoutEditing(Tile tile, Vector2Int index2D)
        {
            int rotation = -90;
            CheckTileWithoutEditing(tile, ref rotation, index2D, Vector2Int.down);
            CheckTileWithoutEditing(tile, ref rotation, index2D, Vector2Int.left);
            CheckTileWithoutEditing(tile, ref rotation, index2D, Vector2Int.up);
            CheckTileWithoutEditing(tile, ref rotation, index2D, Vector2Int.right);
        }

        private void CheckTile(Tile tile, ref int rotation, Vector2Int index2D, Vector2Int offset)
        {
            Tile neighbourTile = GetNeighbourTile(index2D + offset);
            Placeable wallPlace = previewPlaceableInfo is Wall ? previewPlaceableInfo : defaultWall;
            rotation += 90;
            if (neighbourTile != null && neighbourTile.FloorObj != null)
            {
                if (!(neighbourTile.GetPlaceableInfo().Placeable is ResourceTileData))
                {
                    //if (!checkedTiles.Contains(neighbourTile.SaveData.gridPosition))
                    //{
                    //    SetupNeighborTiles(neighbourTile.SaveData.gridPosition);
                    //}

                    tile.EditAnyWall(wallPlace, rotation, true, true);

                    neighbourTile.EditAnyWall(wallPlace, InverseRotation(rotation), true, true);
                    if (tile.FloorObj == null)
                    {
                        neighbourTile.EditAnyWall(wallPlace, InverseRotation(rotation), false, true);
                    }
                    return;
                }
            }
            PlaceableInfo placeableInfo = tile.GetPlaceableInfo();
            if (tile.FloorObj == null)
            {
                if (placeableInfo && !(placeableInfo.Placeable is ResourceTileData))
                    return;
                tile.EditAnyWall(wallPlace, rotation, true, true);
            }
            else
            {
                tile.EditAnyWall(wallPlace, rotation, false, true);
            }

            //Placewalls
        }

        private int InverseRotation(int rotation)
        {
            Debug.Log($"{rotation} <> {(rotation + 180) % 360}");
            Debug.Log($"{GetWallIndex(rotation)} <> {GetWallIndex((rotation + 180) % 360)}");
            return (rotation + 180) % 360;
        }

        private int GetWallIndex(float rotationAngle)
        {
            int index = 0;
            if (rotationAngle > 0)
            {
                index = (int)(rotationAngle / 90);
            }
            return index;
        }

        private void CheckTileWithoutEditing(Tile tile, ref int rotation, Vector2Int index2D, Vector2Int offset)
        {
            Tile neighbourTile = GetNeighbourTile(index2D + offset);
            if (neighbourTile != null && neighbourTile.FloorObj != null)
            {
                if (!checkedTiles.Contains(neighbourTile.SaveData.gridPosition))
                {
                    SetupNeighborTilesWithoutEditing(neighbourTile.SaveData.gridPosition);
                }

                return;
            }
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
            if (currentMode.InputMode == InputMode.Edit | currentMode.InputMode == InputMode.Destroy)
            {
                // Highlight tile with selected building/floor
                currentTile = SelectTile();
                HighlightTile(currentTile);
                previousTile = currentTile;
            }
        }

        private void InitializeTiles(Vector2Int gridSize, float tileSize)
        {
            Debug.Log("Init");

            float tileOffset = tileSize / 2;
            tiles = new Tile[gridSize.x, gridSize.y];
            Material material = gridPlane.GetComponent<Renderer>().material;
            material.mainTextureScale = gridSize;
            startingPosition = gridPlane.transform.position;

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

            gridPlane.transform.localScale = new Vector3(gridSize.x, 0.1f, gridSize.y) * tileSize;
            gridPlane.transform.localPosition = new Vector3(gridSize.x, 0.1f, gridSize.y) * tileSize / 2;
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
                ChangeMaterialColor(!selectedTile.CheckForFloor(previewPlaceableInfo, objectRotationAngle, DestroyMode), DestroyMode);
                if (currentMode.InputMode != InputMode.Select)
                {
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

            previewObject.SetActive(false);

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
            if (gridRayCaster.DoRaycastFromMouse(out RaycastHit hit, gridMask))
            {
                Vector2Int tileIndex = CalculateTilePositionInArray(hit.point);
                currentTileIndex = tileIndex;
                selectedTile = ReturnTile(tileIndex);
            }
            return selectedTile;
        }

        /// <summary>
        /// Returns tile in grid that is being selected by the mouse
        /// </summary>
        /// <returns></returns>
        public Tile GetRandomEmptyTile()
        {
            Tile selectedTile = null;
            int maxIter = 10, iter = 0;
            while (selectedTile == null)
            {
                selectedTile = tiles[Random.Range(0, gridSize.x), Random.Range(0, gridSize.y)];
                if (!selectedTile.HasNoObjects)
                    selectedTile = null;

                if (iter > maxIter) { Debug.LogWarning("No empty space found!"); break; }
                iter++;
            }
            return selectedTile;
        }

        /// <summary>
        /// Returns tile in grid that is being selected by the mouse
        /// </summary>
        /// <returns></returns>
        public List<Tile> GetTilesInRadius(Tile startTile, int radius)
        {
            List<Tile> selectedTiles = new List<Tile>();
            int middleX = startTile.TilePosition.x,
                middleY = startTile.TilePosition.y;
            int startX = Mathf.Clamp(middleX - radius, 0, gridSize.x),
                startY = Mathf.Clamp(middleY - radius, 0, gridSize.y);
            int endX = Mathf.Clamp(middleX + radius, 0, gridSize.x),
                endY = Mathf.Clamp(middleY + radius, 0, gridSize.y);

            for (int x = startX; x < endX; x++)
            {
                for (int y = startY; y < endY; y++)
                {
                    Tile tileBeingChecked = tiles[x, y];
                    if (Vector3.Distance(startTile.Position, tileBeingChecked.Position) <= radius)
                    {
                        if (tileBeingChecked.HasNoObjects)
                            selectedTiles.Add(tileBeingChecked);
                    }
                }
            }
            return selectedTiles;
        }

        /// <summary>
        /// Returns a Vector2 int which points to a position in the 2D Array of tiles.
        /// Casts position.x and position.y to int taking into account the tile size and grid origin.
        /// </summary>
        /// <param name="rayHitPosition"> Hit point of ray on collider of the grid.</param>
        /// <returns></returns>
        public Vector2Int CalculateTilePositionInArray(Vector3 rayHitPosition)
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
        public Tile ReturnTile(Vector2Int tilePositionInArray)
        {
            if (tilePositionInArray.x < tiles.GetLength(0) && tilePositionInArray.y < tiles.GetLength(1) &&
                tilePositionInArray.x >= 0 && tilePositionInArray.y >= 0)
            {
                return tiles[tilePositionInArray.x, tilePositionInArray.y];
            }
            else Debug.LogWarning("Grid Selection is outside of tile bounds");
            return null;
        }

        #endregion Tile Functions
    }
}