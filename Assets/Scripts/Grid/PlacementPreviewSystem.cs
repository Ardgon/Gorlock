using UnityEngine;
using UnityEngine.AI;

public class PlacementPreviewSystem : MonoBehaviour
{
    [SerializeField]
    private float previewYOffset = 0.06f;

    [SerializeField]
    private GameObject cellIndicator;
    private GameObject previewObject;

    [SerializeField]
    private Material previewMaterialPrefab;
    private Material previewMaterialInstance;

    private Renderer cellIndicatorRenderer;
    private bool rotated;


    private void Start()
    {
        previewMaterialInstance = new Material(previewMaterialPrefab);
        cellIndicator.SetActive(false);
        cellIndicatorRenderer = cellIndicator.GetComponentInChildren<Renderer>();
    }

    public void StartShowingPlacementPreview(GameObject prefab, Vector2Int size, bool rotated)
    {
        previewObject = Instantiate(prefab);
        this.rotated = rotated;

        if (rotated)
        {
            previewObject.transform.Rotate(0f, -90f, 0f);
        }

        // Disable Collider and NavMeshObstacles
        Collider[] previewObjectColliders = GetComponentsInChildren<Collider>();
        foreach (Collider col in previewObjectColliders) { col.enabled = false; }
        var previewObjectObstacle = previewObject.GetComponentInChildren<NavMeshObstacle>();
        previewObjectObstacle.enabled = false;

        // Disable AI Controller
        previewObject.GetComponent<BaseTowerController>().enabled = false;
        Destroy(previewObject.GetComponent<FoodSource>());

        PreparePreview(previewObject);
        PrepareCursor(size);
        cellIndicator.SetActive(true);
    }

    private void PrepareCursor(Vector2Int size)
    {
        if (size.x > 0 || size.y > 0)
        {
            cellIndicator.transform.localScale = new Vector3(size.x, 1, size.y);
            cellIndicatorRenderer.material.mainTextureScale = size;
        }
    }

    private void PreparePreview(GameObject previewObject)
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = previewMaterialInstance;
            }
            renderer.materials = materials;
        }
    }

    public void StopShowingPreview()
    {
        cellIndicator.SetActive(false);
        Destroy(previewObject);
    }

    public void UpdatePosition(Vector3 position, bool validity, Vector2Int size)
    {
        MovePreview(position);
        MoveCursor(position, size);
        ApplyFeedback(validity);
    }

    private void ApplyFeedback(bool validity)
    {
        Color color = validity ? Color.white : Color.red;
        color.a = 0.5f;
        cellIndicatorRenderer.material.color = color;
        previewMaterialInstance.color = color;
    }

    private void MoveCursor(Vector3 position, Vector2Int size)
    {
        float xOffset = Mathf.Floor(size.x / 2);
        float zOffset = Mathf.Floor(size.y / 2);
        position.x += xOffset;
        position.z += zOffset;
        cellIndicator.transform.position = position;
    }

    private void MovePreview(Vector3 position)
    {
        Vector3 placementPosition = new Vector3(position.x + 0, position.y + previewYOffset, position.z);
        previewObject.transform.position = placementPosition;
    }
}
