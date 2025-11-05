using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager main;

    private bool isHoveringUI;
    private Turret currentTurret;

    private void Awake()
    {
        main = this;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
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
