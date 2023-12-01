using System;
using UnityEngine;

public class TowerSelectionSystem : MonoBehaviour
{
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private LayerMask targetLayer;
    [SerializeField]
    private GameObject selectedTowerPanel;
    [SerializeField]
    private TowerSelectionPanelPopulator towerSelectionPanelPopulator;

    private BaseTowerController selectedTower;

    // Start is called before the first frame update
    void Start()
    {
        ActivateSelection();
        inputManager.OnExit += ActivateSelection;
    }

    private void ActivateSelection()
    {
        DeselectTower();
        inputManager.OnClicked += SelectTower;
    }

    public void DeactivateSelect()
    {
        inputManager.OnClicked -= SelectTower;
    }

    private void DeselectTower()
    {
        if (selectedTower != null)
            selectedTower.GetComponent<LineRenderer>().enabled = false;
        selectedTower = null;
        selectedTowerPanel.SetActive(false);
    }

    private void SelectTower()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayer))
        {
            // Check if the clicked object is a tower
            BaseTowerController tower = hit.collider.gameObject.GetComponent<BaseTowerController>();

            if (tower != null || !tower.enabled)
            {
                if (selectedTower != null)
                {
                    DeselectTower();
                }

                // Tower is clicked, perform selection logic
                selectedTower = tower;
                towerSelectionPanelPopulator.PopulatePanel(tower.baseStats);
                selectedTowerPanel.SetActive(true);
                selectedTower.GetComponent<LineRenderer>().enabled = true;
            }
        } else
        {
            DeselectTower();
        }
    }
}
