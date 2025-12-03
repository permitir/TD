using TMPro;
using UnityEngine;

public class ElectricityTurret : BaseTurret
{
    
    [Header("Attributes")]
    [SerializeField] private int chainTarget = 2; // This is the number of enemies it will chain to
    [SerializeField] private float chainRange = 2f; // The range between enemies
    [SerializeField] private LineRenderer electricityLine; // Use this instead of bullet prefab
    [SerializeField] private float electricityDuration = 0.25f;
    [SerializeField] private int electricityDamage = 1;
    [SerializeField] private float stunDuration = 1f; // How long enemies will be stunned for

    [Header("Upgrade Stats")]
    [SerializeField] private TextMeshProUGUI ElectricityDamageText;
    [SerializeField] private TextMeshProUGUI stunDurationText;

    private float electricityTimer = 0f;
    private int baseElectricityDamage;
    private float baseStunDuration;

    private void Start()
    {
        bpsBase = BPS;
        targetingRangeBase = targetingRange;
        baseElectricityDamage = electricityDamage;
        baseStunDuration = stunDuration;
        totalMoneySpent = turretCosts;

        upgradeButton.onClick.AddListener(Upgrade);
        sellButton.onClick.AddListener(SellTurret);

        if (upgradeUI != null)
        {
            upgradeUI.SetActive(false);
        }

        UpdateUpgradeCost();

        // Audio source
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;

        // Setup lightning line renderer
        if (electricityLine != null)
        {
            electricityLine.enabled = false;
        }
    }

    private void Update()
    {
        if (electricityTimer > 0)
        {
            electricityTimer -= Time.deltaTime;
            if (electricityTimer <= 0 && electricityLine != null)
            {
                electricityLine.enabled = false;
            }
        }

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

        // If no target then wait for one
        if (target == null)
        {
            FindTarget();
            return;
        }

        RotateTowardsTarget();

        if (!CheckTargetIsInRange())
        {
            target = null;
        }
        else
        {
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

        ElectricityChain(target);
    }

    private void ElectricityChain(Transform initialTarget)
    {
        if (initialTarget == null) return;

        // Stores all targets hit by the chain
        Transform[] targets = new Transform[chainTarget];
        int targetsHit = 0;
        targets[targetsHit] = initialTarget;
        targetsHit++;

        // Damage and stun the first target
        ApplyElectricityEffect(initialTarget);

        // Find and damage chained targets
        Transform currentTarget = initialTarget;

        for (int i = 1; i < chainTarget; i++)
        {
            Transform nextTarget = FindNearestEnemy(currentTarget, targets, targetsHit);

            if (nextTarget != null)
            {
                targets[targetsHit] = nextTarget;
                targetsHit++;

                ApplyElectricityEffect(nextTarget);
                currentTarget = nextTarget;
            } 
            else
            {
                break; // No more targets in range
            }
        }

        // Draws the electricity effect
        if (electricityLine != null)
        {
            DrawElectricity(targets, targetsHit);
        }
    }

    private Transform FindNearestEnemy(Transform form, Transform[] excludeTargets, int excludeCount)
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(form.position, chainRange, Vector2.zero, 0f, enemyMask);

        Transform nearest = null;
        float nearestDistance = Mathf.Infinity;

        foreach (RaycastHit2D hit in hits)
        {
            // Skip if alraedy targetted
            bool alreadyHit = false;
            for (int i = 0; i < excludeCount; i++)
            {
                if (excludeTargets[i] == hit.transform)
                {
                    alreadyHit = true;
                    break;
                }
            }

            if (alreadyHit) continue;

            float distance = Vector2.Distance(form.position, hit.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = hit.transform;
            }
        }

        return nearest;
    }

    private void ApplyElectricityEffect(Transform enemy)
    {
        // Deals damage
        Health enemyHealth = enemy.GetComponent<Health>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(electricityDamage);
        }

        // Applies stun effect
        StunEffect stunEffect = enemy.GetComponent<StunEffect>();
        if (stunEffect == null)
        {
            stunEffect = enemy.gameObject.AddComponent<StunEffect>();
        }

        stunEffect.ApplyStun(stunDuration);
    }

    private void DrawElectricity(Transform[] targets, int count)
    {
        if (electricityLine == null || count == 0) return;

        electricityLine.positionCount = count + 1;
        electricityLine.SetPosition(0, firingPoint.position);

        // Draws line to each chained target
        for (int i = 0; i < count; i++)
        {
            if (targets[i] != null)
            {
                electricityLine.SetPosition(i + 1, targets[i].position);
            }
        }

        electricityLine.enabled = true;
        electricityTimer = electricityDuration;
    }

    public override void OpenUpgradeUI()
    {
        UIManager.main.ClearCurrentTurret();
        upgradeUI.SetActive(true);
        upgradeStatsUI.SetActive(true);
        UpdateUpgradeCost();
        UpdateUpgradeUI();
        UpdateElectricityUpgradeUI();

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
        if (UpgradeCalculator() > LevelManager.main.currency) return;

        LevelManager.main.SpendCurrency(UpgradeCalculator());
        totalMoneySpent += UpgradeCalculator();
        UIManager.main.ClearCurrentTurret();

        level++;
        BPS = BPSCalculator();
        targetingRange = RangeCalculator();
        electricityDamage = baseElectricityDamage + (level - 1);
        stunDuration = baseStunDuration + ((level - 1) * 0.1f);

        // Increase chain targets every 2 levels
        if (level % 2 == 0)
        {
            chainTarget++;
        }

        CloseUpgradeUI();
    }

    private void UpdateElectricityUpgradeUI()
    {
        if (ElectricityDamageText != null)
        {
            ElectricityDamageText.text = "E-Damage: " + electricityDamage;
        }
        
        if (stunDurationText != null)
        {
            stunDurationText.text = "Stun: " + stunDuration.ToString("F1") + "s";
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
