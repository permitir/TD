using UnityEngine;

public class FireTurret : BaseTurret
{
    private void Start()
    {
        bpsBase = BPS;
        targetingRangeBase = targetingRange;
        totalMoneySpent = turretCosts; //tracks initial turret costs

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
        //If no target then it waits till a target is spotted.
        if (target == null)
        {
            FindTarget();
            return;
        }

        RotateTowardsTarget();

        if (!CheckTargetIsInRange())
        {
            //If target not in range then do nothing 
            target = null;
        }
        else
        { //However if target in range, shoot and then reset the timer since last bullet shot so it doesn't become a minigun.
            timeUntilFire += Time.deltaTime;

            if (timeUntilFire >= 1f / BPS)
            {
                Shoot();
                timeUntilFire = 0f;
            }
        }
    }

    protected override void Shoot()
    {
        // Play shooting sound
        if (shootSound != null)
        {
            audioSource.PlayOneShot(shootSound, GetSFXVolume());
        }


        //gets bullet prefab and shoots it at the enemy from the turrets firing point.
        GameObject bulletObj = Instantiate(bulletPrefab, firingPoint.position, Quaternion.identity);
        FireBullet bulletScript = bulletObj.GetComponent<FireBullet>();
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
        //What happens after user buys a turret/tower upgrade
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

            CloseUpgradeUI();
        }
    }

    public void SellTurret()
    {
        int sellValue = Mathf.RoundToInt(totalMoneySpent * sellPercentage / level); //calculating how much the player gets back by doing all upgrades/tower value * x%
        LevelManager.main.IncreaseCurrency(sellValue); //give player money back
        CloseUpgradeUI();
        Destroy(gameObject);
    }
}
