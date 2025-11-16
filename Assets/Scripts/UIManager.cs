using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public static UIManager main;

    private bool isHoveringShopUI; //check if hovering over shop ui
    private bool isHoveringUI; // check if hovering over upgrade ui
    private Turret currentTurret;
    public bool isMenuOpen; // check if menu is open

    private void Awake()
    {
        main = this;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!isHoveringUI && !IsPointerOverUI() && currentTurret != null)
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Collider2D hit = Physics2D.OverlapPoint(mousePos);

                 if (hit == null || !hit.GetComponent<Plot>())
                {
                    currentTurret.CloseUpgradeUI();
                    currentTurret = null;
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

    public void SetShopHoverState(bool state)
    {
        isHoveringShopUI = state;
    }

    public void SetHoveringState(bool state)
    {
        isHoveringUI = state;
    }

    public bool isHovering()
    {
        return isHoveringUI || isHoveringShopUI;
    }

    public void SetCurrentTurret(Turret turret)
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
