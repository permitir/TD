using System;
using Unity.Mathematics;
using UnityEngine;

public class MinigunTurret : BaseTurret
{
    [Header("Attributes")]
    [SerializeField] private float maxBPS = 10f; // Max fire ratewhen fully spun up
    [SerializeField] private float minBPS = 1f; // The starting firing rate
    [SerializeField] private float spinUpTime = 2f; // Time to get to max firing rate
    [SerializeField] private float spinDownTime = 1f; // Time taken till the spin to slow down when not firing

    private float currentSpinLevel = 0f; // How much/fast the barrel is spinning
    private float currentBPS; // The current amount of bullets per second based on the spin
    private bool isFiring = false; // Checks if the minigun is firing

    private void Start()
    {
        bpsBase = BPS;
        targetingRangeBase = targetingRange;
        totalMoneySpent = turretCosts;

        upgradeButton.onClick.AddListener(Upgrade); //Button Listener
        sellButton.onClick.AddListener(SellTurret); //Button Listener

        if (upgradeUI != null)
        {
            upgradeUI.SetActive(false);
        }

        UpdateUpgradeCost();

        //Audio source
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;

        currentBPS = minBPS;
    }

    private void Update()
    {
        // Checker to see if the user's mouse is over an UI (blocks turret placement if mouse clicks are over an UI (TAG: MENU))
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hitCollider = Physics2D.OverlapPoint(mousePos);

            if (hitCollider != null && hitCollider.gameObject == gameObject)
            {
                if (!UIManager.main.isHovering())
                {
                    OpenUpgradeUI();
                }
            }
        }

        // If no target then it waits till a target is spotted.
        if (target == null)
        {
            FindTarget();
            isFiring = false;
            return;
        }

        if (!CheckTargetIsInRange())
        {
            // If target not in range then do nothing 
            target = null;
            isFiring = false;
        }
        else
        { // However if target in range, shoot and then reset the timer since last bullet shot so it doesn't become a minigun.
            RotateTowardsTarget();
            isFiring = true;
            timeUntilFire += Time.deltaTime;

            if (timeUntilFire >= 1f / BPS)
            {
                Shoot();
                timeUntilFire = 0f;
            }
        }

        if (isFiring)
        {
            // Spin up - increases the spin level of turret
            Mathf.Clamp01(currentSpinLevel + (Time.deltaTime / spinUpTime));
            // Safeguard to keep the value locked between 0 and 1
        } 
        else
        {
            // Spin down
            Mathf.Clamp01(currentSpinLevel - (Time.deltaTime / spinDownTime));
        }

        //
        currentBPS = Mathf.Lerp(minBPS, maxBPS, currentSpinLevel);
        // min, max, %
        // 1, 10, 0/100
        // Examples:
        //          25% spun = Mathf.Lerp(1, 8, 0.25) = 2.75
        //                      25% between 1 & 8
        //          50% spin = Mathf.Lerp(1, 8, 0.5) = 4.5
        //                      50% between 1 & 8
    }

    protected override void Shoot()
    {
        // Play shooting sound
        if (shootSound != null)
        {
            audioSource.PlayOneShot(shootSound, GetSFXVolume());
        }

        // This gets bullet prefab and shoots it at the enemy from the turrets firing point.
        GameObject bulletObj = Instantiate(bulletPrefab, firingPoint.position, Quaternion.identity);
        Bullet bulletScript = bulletObj.GetComponent<Bullet>();
        bulletScript.SetTarget(target);
    }

    public override void OpenUpgradeUI()
    {
        UIManager.main.ClearCurrentTurret();
        upgradeUI.SetActive(true);
        upgradeStatsUI.SetActive(true);
        UpdateUpgradeCost();
        UpdateUpgradeUI();

        UIManager.main.SetCurrentTurret(this);
    }

    public override void CloseUpgradeUI()
    {
        upgradeUI.SetActive(false);
        upgradeStatsUI.SetActive(false);
        UIManager.main.SetHoveringState(false);
        UIManager.main.ClearCurrentTurret();
    }

    public void Upgrade()
    {
        // What happens after user buys a turret/tower upgrade
        if (UpgradeCalculator() > LevelManager.main.currency)
        {
            StartCoroutine(LevelManager.main.ShowErrorTemporarily(1.5f));
        } else
        {
            LevelManager.main.SpendCurrency(UpgradeCalculator());
            totalMoneySpent += UpgradeCalculator();
            UIManager.main.ClearCurrentTurret();

            level++;

            BPS = BPSCalculator();
            targetingRange = RangeCalculator();

            // Calculation to increase the max fire rate with upgrades
            maxBPS = bpsBase * 8f * Mathf.Pow(level, 0.2f);
            minBPS = bpsBase * Mathf.Pow(level, 0.2f);

            // Decrease the spin up time
            spinUpTime = MathF.Max(0.5f, 2f - (level * 0.2f));

            CloseUpgradeUI();
        }
    }

     public void SellTurret()
    {
        int sellValue = Mathf.RoundToInt(totalMoneySpent * sellPercentage / level); // This calculates how much the player gets back by doing all upgrades/tower value * x%
        LevelManager.main.IncreaseCurrency(sellValue); // This give player money back
        CloseUpgradeUI();
        Destroy(gameObject);
    }
}
