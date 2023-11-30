using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBlocker : MonoBehaviour
{
    [SerializeField]
    private Vector2Int size;
    [SerializeField]
    private int id = -1;
    private GridPlacementSystem placementSystem;

    // Start is called before the first frame update
    void Start()
    {
        placementSystem = FindObjectOfType<GridPlacementSystem>();

        placementSystem.AddObject(gameObject, size, -1);
    }

}
