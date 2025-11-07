using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.UI;

public class Plot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Color hoverColor;

    public GameObject towerObj;
    public Turret turret;
    private Color startColor;

    private void Start()
    {
        startColor = sr.color;
    }

    private void OnMouseEnter()
    {
        //Doesn't show hover colour if UI in the way
        if (!UIManager.main.IsPlacementBlocked())
        {
            sr.color = hoverColor;
        }
    }

    private void OnMouseExit()
    {
        sr.color = startColor;
    }

    private void OnMouseDown()
    {
        //Checks if placement is blocked
        if (UIManager.main.IsPlacementBlocked())
        {
            Debug.Log("Placement blocked");
            return;
        }

    //if there's already a tower placed, do nothing
        if (towerObj != null)
        {
            turret.OpenUpgradeUI();
            UIManager.main.SetHoveringState(this);
            return;
        }

        //tower to build = selected tower
        Tower towerToBuild = BuildManager.main.GetSelectedTower();

        //if tower costs more than currency (player's balance), do nothing
        if (towerToBuild.cost > LevelManager.main.currency)
        {
            Debug.Log("You do not have any money to buy this item currently.");
            return;
        }

        LevelManager.main.SpendCurrency(towerToBuild.cost);

        //gets the tower user wants to build
        towerObj = Instantiate(towerToBuild.prefab, transform.position, Quaternion.identity);
        turret = towerObj.GetComponent<Turret>();
        UIManager.main.ClearCurrentTurret();
    }
}