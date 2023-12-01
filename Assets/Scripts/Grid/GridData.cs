using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridData 
{
    Dictionary<Vector3Int, PlacementData> placedObjects = new();

    public int GetObjectIdAtCell(Vector3Int gridPosition)
    {
        if (!placedObjects.ContainsKey(gridPosition))
        {
            Debug.LogError($"No object placed on grid pos {gridPosition} when getting id");
            return -1;
        }
            
        return placedObjects[gridPosition].ID;
    }

    public void RemoveObjectAt(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> positionToOccupy = CalculatePosition(gridPosition, objectSize);

        foreach (var pos in positionToOccupy)
        {
            if (!placedObjects.ContainsKey(pos))
                throw new Exception($"Dictionary does contains this cell position: {pos}");
            placedObjects.Remove(pos);
        }
    }

    public void AddObjectAt(Vector3Int gridPosition, Vector2Int objectSize, int ID)
    {
        List<Vector3Int> positionToOccupy = CalculatePosition(gridPosition, objectSize);
        PlacementData data = new PlacementData(positionToOccupy, ID);

        foreach(var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
                throw new Exception($"Dictionary already contains this cell position: {pos}");
            placedObjects[pos] = data;
        }
    }

    private List<Vector3Int> CalculatePosition(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> returnVal = new();
        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                returnVal.Add(gridPosition + new Vector3Int(x, 0, y));
            }
        }
        return returnVal;
    }

    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> positionsToOccupy = CalculatePosition(gridPosition, objectSize);

        foreach (var pos in positionsToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
                return false;
        }

        return true;
    }
}

public class PlacementData
{
    public List<Vector3Int> OccupiedPositions;
    public int ID { get; private set; }

    public PlacementData(List<Vector3Int> occupiedPositions, int iD)
    {
        this.OccupiedPositions = occupiedPositions;
        this.ID = iD;
    }
}