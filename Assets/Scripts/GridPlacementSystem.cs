using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPlacementSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject mouseIndicator;
    [SerializeField]
    private GameObject cellIndicator;
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private Grid grid;

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        mouseIndicator.transform.position = mousePosition;
        cellIndicator.transform.position = grid.CellToWorld(gridPosition);
    }
}
