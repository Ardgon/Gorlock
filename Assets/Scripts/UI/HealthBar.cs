using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private RectTransform healthBar;
    private Health healthComponent;
    private Camera mainCamera;
    private Canvas canvas;

    private void Start()
    {
        healthComponent = GetComponentInParent<Health>();
        mainCamera = Camera.main;
        canvas = GetComponent<Canvas>();
    }

    private void Update()
    {
        float healthPerentage = healthComponent.GetHitPointPercentage();
        healthBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, healthPerentage);

        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.back, mainCamera.transform.rotation * Vector3.up);

        if (healthPerentage < 0.95f)
        {
            canvas.enabled = true;
        }
        else
        {
            canvas.enabled = false;
        }
    }
}
