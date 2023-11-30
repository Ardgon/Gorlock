using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    private void Start()
    {
        StopPlacement();
        gridObjectData = new();
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

        if (!CheckPlacementValidity(gridPosition))
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

        placedGameObjects.Add(newObject);
        gridObjectData.AddObjectAt(gridPosition, GetSize(), database.objectData[selectedObjectIndex].ID, placedGameObjects.Count - 1);
        preview.UpdatePosition(grid.CellToWorld(gridPosition), false);
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition)
    {
        return gridObjectData.CanPlaceObjectAt(gridPosition, GetSize());
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

        bool placementValidity = CheckPlacementValidity(gridPosition);
        preview.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
        lastDetectedPosition = gridPosition;
    }
}
