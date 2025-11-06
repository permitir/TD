using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    [Header("Blocker")]
    [SerializeField] private GameObject uiBlocker;

    public static UIManager main;

    private bool isHoveringUI;
    private Turret currentTurret;

    public bool isMenuOpen;

    private void Awake()
    {
        main = this;
    }

    public bool IsInputBlocked()
    {
        return isMenuOpen || (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject());
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Pointer over uI?" + EventSystem.current.IsPointerOverGameObject());
            if (!isHoveringUI && currentTurret != null)
            {
                currentTurret.CloseUpgradeUI();
                currentTurret = null;
            }
        }
    }

    public void SetHoveringState(bool state)
    {
        isHoveringUI = state;
    }

    public bool isHovering()
    {
        return isHoveringUI;
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

    internal void SetCurrentTurret(Plot plot)
    {
        throw new NotImplementedException();
    }
}
