using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor.Rendering;
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

    private GridData gridObjectData;

    private List<GameObject> placedGameObjects = new();
    private Vector3Int lastDetectedPosition = Vector3Int.zero;
    private bool rotated;

    private void Awake()
    {
        StopPlacement();
        gridObjectData = new();
    }

    public GameObject SpawnRandomlyOnGrid(int id, int attempts = 5)
    {
        GameObject prefabToSpawn = database.objectData[id].Prefab;
        Vector2Int size = database.objectData[id].Size;

        for (int i = 0; i < attempts; i++)
        {
            Vector3 playingGridSize = gridVisualization.transform.localScale * 10;
            playingGridSize.y = 0;
            Vector3 playingGridBottomLeft = gridVisualization.transform.position - (playingGridSize / 2);

            // Generate random position within the grid
            Vector3Int randomGrid = new Vector3Int(
                UnityEngine.Random.Range(0, (int)playingGridSize.x),
                0,
                UnityEngine.Random.Range(0, (int)playingGridSize.z));

            // Spawn prefab with random rotation
            Vector3 worldPosition = grid.GetCellCenterWorld(randomGrid) + playingGridBottomLeft;
            Vector3Int gridPosition = grid.WorldToCell(worldPosition);

            // Check if the position is valid for placement
            if (CheckPlacementValidity(gridPosition, size))
            {
                float randomRotationY = UnityEngine.Random.Range(0f, 360f);

                GameObject spawnedObject = Instantiate(prefabToSpawn, worldPosition, Quaternion.identity);
                spawnedObject.transform.rotation = Quaternion.Euler(0f, randomRotationY, 0f);

                AddObject(spawnedObject, size, id);

                return spawnedObject;
            }
        }

        return null;
    }

    public void AddObject(GameObject spawnedObject, Vector2Int size, int id)
    {
        Vector3Int gridPosition = grid.WorldToCell(spawnedObject.transform.position);
        placedGameObjects.Add(spawnedObject);
        gridObjectData.AddObjectAt(gridPosition, size, id, placedGameObjects.Count - 1);
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

        if (!CheckPlacementValidity(gridPosition, GetSize()))
            return;

        GameObject newObject = Instantiate(database.objectData[selectedObjectIndex].Prefab);
        newObject.transform.position = grid.CellToWorld(gridPosition);

        if (rotated)
        {
            newObject.transform.Rotate(0f, -90f, 0f);
            newObject.transform.position = new Vector3(
                newObject.transform.position.x + 1,
                newObject.transform.position.y,
                newObject.transform.position.z);
        }

        AddObject(newObject, GetSize(), database.objectData[selectedObjectIndex].ID);
        preview.UpdatePosition(grid.CellToWorld(gridPosition), false); 
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, Vector2Int size)
    {
        return gridObjectData.CanPlaceObjectAt(gridPosition, size);
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

        bool placementValidity = CheckPlacementValidity(gridPosition, GetSize());
        preview.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
        lastDetectedPosition = gridPosition;
    }
}
