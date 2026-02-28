using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public static UIManager main;

    private bool isHoveringShopUI; //check if hovering over shop ui
    private bool isHoveringUI; // check if hovering over upgrade ui
    private BaseTurret currentTurret;
    public bool isMenuOpen; // check if menu is open

    private void Awake()
    {
        main = this;
    }

    private void Update()
    {

        // Click away to deselect turret
        if (Input.GetMouseButtonDown(0))
        {
            if (!isHoveringUI && !IsPointerOverUI() && currentTurret != null)
            {
                // checks if mouse is not hovering UI (isHoveringUI tag)
                // Mouse is not over a UI element with "Menu" tag (isPointerOverUI) 
                // There is a turret currently selected (currentTurret != null)
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Collider2D hit = Physics2D.OverlapPoint(mousePos); // Returns collider if something was hit or null if not

                 if (hit == null || !hit.GetComponent<Plot>()) // check if player click empty space or not a plot [[]] hit == null = click on something [[]] (!hit.GetComponent<Plot>()) = clicked on something, but it's not a Plot
                {
                    // Player clicked away from the turret/plot, so close the upgrade UI
                    currentTurret.CloseUpgradeUI();
                    currentTurret = null; // Clears the currently selected turret (deselects it)
                }
            }
        }
    }

    //checks if pointer is over any UI
    public bool IsPointerOverUI()
    {
        if (EventSystem.current == null)
            return false;

        return EventSystem.current.IsPointerOverGameObject();
    }

    //Checks if turret placement should be blocked
    public bool IsPlacementBlocked()
    {
        return isHoveringUI || isHoveringShopUI || IsPointerOverUI();
    }

    public void SetShopHoverState(bool state) // Setting state as hover
    {
        isHoveringShopUI = state;
    }

    public void SetHoveringState(bool state) // Setting state as hover
    {
        isHoveringUI = state;
    }

    public bool isHovering() // Check if hovering
    {
        return isHoveringUI || isHoveringShopUI;
    }

    public void SetCurrentTurret(BaseTurret turret)
    {
        if (currentTurret != null && currentTurret != turret)
        {
            currentTurret.CloseUpgradeUI();
        }

    currentTurret = turret;
    }

    public void ClearCurrentTurret()
    {
        currentTurret = null;
    }

    public void MenuOpen(bool open)
    {
        isMenuOpen = open;
    }
}