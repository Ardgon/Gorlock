using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class GridPlacementSystem : MonoBehaviour
{
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private Grid grid;

    [SerializeField]
    private TowersDatabase_SO database;
    private int selectedObjectIndex = -1;

    [SerializeField]
    private GameObject gridVisualization;

    [SerializeField]
    private TowerSelectionSystem selectionSystem;

    [SerializeField]
    private PlacementPreviewSystem preview;

    private GridData gridObjectData = new();
    private Vector3Int lastDetectedPosition = Vector3Int.zero;
    private bool rotated;

    private void Awake()
    {
        StopPlacement();
    }

    public int GetObjectIdAtCell(Vector3Int gridPosition)
    {
        return gridObjectData.GetObjectIdAtCell(gridPosition);
    }

    public void RemoveTower(BaseTowerController tower)
    {
        var gridPos = WorldToCellPosition(tower.transform.position);
        var id = GetObjectIdAtCell(gridPos);
        var size = database.objectData[id].Size;
        if (rotated)
            size = new Vector2Int(size.y, size.x);

        gridObjectData.RemoveObjectAt(gridPos, size);
    }

    public Vector3Int WorldToCellPosition(Vector3 worldPosition)
    {
        return grid.WorldToCell(worldPosition);
    }

    public Vector3 CellToWorldPosition(Vector3Int gridPosition)
    {
        return grid.CellToWorld(gridPosition);
    }

    public GameObject SpawnRandomlyOnGrid(int id, int cost = 0, int attempts = 5)
    {
        GameObject prefabToSpawn = database.objectData[id].Prefab;
        Vector2Int size = database.objectData[id].Size;

        for (int i = 0; i < attempts; i++)
        {
            Vector3 playingGridSize = gridVisualization.transform.localScale * 10;
            playingGridSize.y = 0;
            Vector3 playingGridBottomLeft = gridVisualization.transform.position - (playingGridSize / 2);
            Debug.DrawLine(playingGridBottomLeft, playingGridBottomLeft + Vector3.up * 10f, UnityEngine.Color.red, 100f);


            // Generate random position within the grid
            Vector3Int randomGrid = new Vector3Int(
                UnityEngine.Random.Range(0, (int)playingGridSize.x),
                0,
                UnityEngine.Random.Range(0, (int)playingGridSize.z));

            // Spawn prefab with random rotation
            Vector3 worldPosition = randomGrid + playingGridBottomLeft;
            Vector3Int gridPosition = grid.WorldToCell(worldPosition);

            // Check if the position is valid for placement
            if (CheckPlacementValidity(gridPosition, size, cost))
            {
                float randomRotationY = UnityEngine.Random.Range(0f, 360f);

                Vector3 placementPosition = OffestPositionToCellCenter(worldPosition);
                GameObject spawnedObject = Instantiate(prefabToSpawn, placementPosition, Quaternion.identity);
                spawnedObject.transform.rotation = Quaternion.Euler(0f, randomRotationY, 0f);

                AddObject(spawnedObject, size, id);

                return spawnedObject;
            }
        }

        return null;
    }

    public void RemoveObject(GameObject obj, Vector2Int size)
    {
        Vector3Int gridPosition = grid.WorldToCell(obj.transform.position);
        gridObjectData.RemoveObjectAt(gridPosition, size);
    }

    public void AddObject(GameObject spawnedObject, Vector2Int size, int id)
    {
        Vector3Int gridPosition = grid.WorldToCell(spawnedObject.transform.position);
        gridObjectData.AddObjectAt(gridPosition, size, id);
    }

    // Called From UI Button
    public void StartPlacement(int ID)
    {
        // Deactivate Selection mode when placing
        selectionSystem.DeactivateSelect();

        StopPlacement();
        selectedObjectIndex = database.objectData.FindIndex(data => data.ID == ID);
        if (selectedObjectIndex < 0)
        {
            Debug.LogError($"No ID {ID}");
            return;
        }
        gridVisualization.SetActive(true);
        preview.StartShowingPlacementPreview(database.objectData[selectedObjectIndex].Prefab, GetSize(), rotated);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    private void PlaceStructure()
    {
        if (inputManager.IsPointerOverUI())
        {
            return;
        }

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        if (!CheckPlacementValidity(gridPosition, GetSize(), GetCost()))
            return;

        GameObject newObject = Instantiate(database.objectData[selectedObjectIndex].Prefab);
        Vector3 placementPosition = OffestPositionToCellCenter(grid.CellToWorld(gridPosition));
        newObject.transform.position = placementPosition;

        if (rotated)
        {
            newObject.transform.Rotate(0f, -90f, 0f);
        }

        AddObject(newObject, GetSize(), database.objectData[selectedObjectIndex].ID);

        GameMode.Instance.RemoveCoins(GetCost());

        preview.UpdatePosition(placementPosition, false, GetSize()); 
    }

    public Vector3 OffestPositionToCellCenter(Vector3 worldPosition)
    {
        return new Vector3(worldPosition.x + 0.5f, worldPosition.y, worldPosition.z + 0.5f);
    }

    public bool CheckPlacementValidity(Vector3Int gridPosition, Vector2Int size, int cost)
    {
        return CanAffordToPlace(cost) && gridObjectData.CanPlaceObjectAt(gridPosition, size);
    }

    private bool CanAffordToPlace(int cost)
    {
        return GameMode.Instance.HasEnoughCoins(cost);
    }

    private Vector2Int GetSize()
    {
        if (rotated)
        {
            return new Vector2Int(database.objectData[selectedObjectIndex].Size.y,
                database.objectData[selectedObjectIndex].Size.x);
        }

        return database.objectData[selectedObjectIndex].Size;
    }

    private int GetCost()
    {
        return database.objectData[selectedObjectIndex].Cost;
    }

    private void StopPlacement()
    {
        selectedObjectIndex = -1;
        gridVisualization.SetActive(false);
        preview.StopShowingPreview();
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        lastDetectedPosition = Vector3Int.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (selectedObjectIndex < 0)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            rotated = !rotated;
            preview.StopShowingPreview();
            preview.StartShowingPlacementPreview(database.objectData[selectedObjectIndex].Prefab, GetSize(), rotated);
        }

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        bool placementValidity = CheckPlacementValidity(gridPosition, GetSize(), GetCost());
        Vector3 placementPosition = OffestPositionToCellCenter(grid.CellToWorld(gridPosition));
        preview.UpdatePosition(placementPosition, placementValidity, GetSize());
        lastDetectedPosition = gridPosition;
    }
}
