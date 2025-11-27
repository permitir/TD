using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Plot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Color hoverColor;

    public GameObject towerObj;
    public BaseTurret turret;
    private Color startColor;

    private void Start()
    {
        startColor = sr.color;
    }

    private void OnMouseEnter()
    {

        if (!IsPointerOverUIElement())
        {
            sr.color = hoverColor;
        }
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
        if (IsPointerOverUIElement())
        {
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
            StartCoroutine(LevelManager.main.ShowErrorTemporarily(1.5f));
            Debug.Log("You do not have any money to buy this item currently.");
            return;
        }

        LevelManager.main.SpendCurrency(towerToBuild.cost);

        //gets the tower user wants to build
        towerObj = Instantiate(towerToBuild.prefab, transform.position, Quaternion.identity);
        turret = towerObj.GetComponent<BaseTurret>();
        //SaveSystem.instance.AddTurretPlaced(); - OLD 
        if (SaveSystem.instance != null) // Saves how many turrets the user has ever placed through their playtime - NEW
        {
            SaveSystem.instance.AddTurretPlaced();
        }
        UIManager.main.ClearCurrentTurret();
    }
    
    private bool IsPointerOverUIElement()
    {
        // Checker to see if the user's mouse is over an UI (blocks turret placement if mouse clicks are over an UI (TAG: MENU))
        if (EventSystem.current == null)
            return false;

        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult result in results)
        {
            GameObject uiObject = result.gameObject;

            if (uiObject.CompareTag("Menu"))
            {
                // Debug.Log("Blocked by UI"); - KEEP IT AS A NOTE UNLESS ISSUE WITH PLOTS! (This will flood player logs if not disabled!)
                return true;
            }
        }

        return false;
    }
}