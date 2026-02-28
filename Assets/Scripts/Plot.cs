using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Plot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Color hoverColour;

    public GameObject towerObj;
    public BaseTurret turret;
    private Color startColour;

    private void Start()
    {
        startColour = sr.color; // Default colours
    }

    private void OnMouseEnter()
    {

        if (!IsPointerOverUIElement())
        {
            sr.color = hoverColour;
        }
        //Doesn't show hover colour if UI in the way
        if (!UIManager.main.IsPlacementBlocked())
        {
            sr.color = hoverColour; // Placement isn't blocked, show hover colour
        }
    }

    private void OnMouseExit()
    {
        sr.color = startColour; // Whenever mouse exists a plot it returns it to its starter colour
    }

    private void OnMouseDown()
    {
        if (IsPointerOverUIElement())
        {
            return;
        }

        //if there's already a tower placed, open upgrade menu
        if (towerObj != null)
        {
            turret.OpenUpgradeUI();
            UIManager.main.SetHoveringState(this);
            return;
        }

        // Tower to build = selected tower
        Tower towerToBuild = BuildManager.main.GetSelectedTower();

        //if tower costs more than currency (player's balance), do nothing & show error
        if (towerToBuild.cost > LevelManager.main.currency)
        {
            StartCoroutine(LevelManager.main.ShowErrorTemporarily(1.5f)); // How long the error message will display for
            Debug.Log("You do not have any money to buy this item currently.");
            return;
        }

        LevelManager.main.SpendCurrency(towerToBuild.cost); // Spends currency if money > cost.

        // Gets the tower user wants to build
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
        if (EventSystem.current == null) // Checks if theres an event system on scene
            return false;

        PointerEventData eventData = new PointerEventData(EventSystem.current); // Stores information of where the mouse pointer is
        eventData.position = Input.mousePosition; // position of where the mouse actually is

        List<RaycastResult> results = new List<RaycastResult>(); // Empty list to store all UI elements the raycast hits & fills list with all the UI elements it hits
        EventSystem.current.RaycastAll(eventData, results); // loops every UI element the raycast hit

        foreach (RaycastResult result in results) // Get GameObject of current UI being checked
        {
            GameObject uiObject = result.gameObject; // Checks if it has "Menu" tag

            if (uiObject.CompareTag("Menu"))
            {
                // Debug.Log("Blocked by UI"); - KEEP IT AS A NOTE UNLESS ISSUE WITH PLOTS! (This will flood player logs if not disabled!)
                return true;
            }
        }

        return false; // if no tag hit, free to place turret
    }
}