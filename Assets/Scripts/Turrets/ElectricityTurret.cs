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
    [SerializeField] private float stunDuration = 0.4f; // How long enemies will be stunned for

    [Header("Upgrade Stats")]
    [SerializeField] private TextMeshProUGUI ElectricityDamageText;
    [SerializeField] private TextMeshProUGUI stunDurationText;

    private float electricityTimer = 0f;
    private int baseElectricityDamage;
    private float baseStunDuration;

    private void Start()
    {
        bpsBase = BPS; // storing base bullet value
        targetingRangeBase = targetingRange; // storing value
        baseElectricityDamage = electricityDamage; // stroing value
        baseStunDuration = stunDuration; // storing value
        totalMoneySpent = turretCosts; // storing value

        upgradeButton.onClick.AddListener(Upgrade); // linking upgrade button to Upgrade function
        sellButton.onClick.AddListener(SellTurret); // linking sell button to SellTurret function

        if (upgradeUI != null)
        {
            upgradeUI.SetActive(false); // hide upgrade UI at start
        }

        UpdateUpgradeCost(); // update displayed upgrade cost and sell value

        // Audio source
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;

        // Setup lightning line renderer
        if (electricityLine != null)
        {
            electricityLine.enabled = false; // hiding lightning effect at start
        }
    }

    private void Update()
    {
        if (electricityTimer > 0) // if lightning effect is currently shwoing:
        {
            electricityTimer -= Time.deltaTime; // count down the timer
            if (electricityTimer <= 0 && electricityLine != null) // when timer reaches 0:
            {
                electricityLine.enabled = false; // hides the lightning effect
            }
        }

        if (Input.GetMouseButtonDown(0)) // when left mouse is clicked
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // convert mouse pos to world pos
            Collider2D hitCollider = Physics2D.OverlapPoint(mousePos); // check what was clicked

            if (hitCollider != null && hitCollider.gameObject == gameObject) // if this turret was clicked
            {
                if (!UIManager.main.isHovering()) // make sure it isnt hovering over UI
                {
                    OpenUpgradeUI(); // open upgrade menu
                }
            }
        }

        // If no target then wait for one
        if (target == null)
        {
            FindTarget(); // searching for enemy
            return; // exit until enemy found
        }

        RotateTowardsTarget(); // rotate towards target

        if (!CheckTargetIsInRange()) // if target moved out of range
        {
            target = null; // clear the target so turret find a new one
        }
        else
        {
            timeUntilFire += Time.deltaTime; // increase firing timer

            if (timeUntilFire >= 1f / BPS) // if enough time has passed:
            {
                Shoot(); // fire at the target
                timeUntilFire = 0f; // reset firing timer
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
        if (initialTarget == null) return; // Exit if no target found

        // Stores all targets hit by the chain
        Transform[] targets = new Transform[chainTarget]; // Array to hold all enemies to stun
        int targetsHit = 0; // how many enemies have been hit
        targets[targetsHit] = initialTarget; // add first target as targets[0]
        targetsHit++; // Increase counter

        // Damage and stun the first target
        ApplyElectricityEffect(initialTarget);

        // Find and damage chained targets
        Transform currentTarget = initialTarget;

        for (int i = 1; i < chainTarget; i++) // Loop through remaining chain slots
        {
            Transform nextTarget = FindNearestEnemy(currentTarget, targets, targetsHit); // Find closests enemy that has NOT been hit

            if (nextTarget != null) // If more extra targets
            {
                targets[targetsHit] = nextTarget; // add to array
                targetsHit++;

                ApplyElectricityEffect(nextTarget);
                currentTarget = nextTarget; // Update current position for next chain search
            } 
            else
            {
                break; // No more targets in range
            }
        }

        // Draws the electricity effect
        if (electricityLine != null)
        {
            DrawElectricity(targets, targetsHit); // Shows the lightning chain from enemy to enemy
        }
    }

    private Transform FindNearestEnemy(Transform form, Transform[] excludeTargets, int excludeCount)
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(form.position, chainRange, Vector2.zero, 0f, enemyMask); // Finds all enemies within the chain range

        Transform nearest = null; // Stores the closest enemy
        float nearestDistance = Mathf.Infinity; // start with infinite distance

        foreach (RaycastHit2D hit in hits) // check each enemy found
        {
            // Skip if alraedy targetted
            bool alreadyHit = false; // flag to check if the enemy actually has been hit
            for (int i = 0; i < excludeCount; i++) // loop through all PREVIOUS targets
            {
                if (excludeTargets[i] == hit.transform) // check if enemies match
                {
                    alreadyHit = true; // mark as alr hit
                    break; // Stop check
                }
            }

            if (alreadyHit) continue; // skip if enemy alr been hit

            float distance = Vector2.Distance(form.position, hit.transform.position); // calculate distance to next enemy
            if (distance < nearestDistance)
            {
                nearestDistance = distance; // Update closest distance
                nearest = hit.transform; // Update closest enemy
            }
        }

        return nearest; // nearestEnemy
    }

    private void ApplyElectricityEffect(Transform enemy)
    {
        // Deals damage
        Health enemyHealth = enemy.GetComponent<Health>(); 
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(electricityDamage); // Apply the electricity damage
        }

        // Applies stun effect
        StunEffect stunEffect = enemy.GetComponent<StunEffect>(); // Check if enemy has stun effect
        if (stunEffect == null)
        {
            stunEffect = enemy.gameObject.AddComponent<StunEffect>(); // If null add stun effect
        }

        stunEffect.ApplyStun(stunDuration); // Stun enemy for the duration
    }

    private void DrawElectricity(Transform[] targets, int count)
    {
        if (electricityLine == null || count == 0) return; // stop if no line renderer or no targets

        electricityLine.positionCount = count + 1; // Number of points
        electricityLine.SetPosition(0, firingPoint.position);

        // Draws line to each chained target
        for (int i = 0; i < count; i++)
        {
            if (targets[i] != null)
            {
                electricityLine.SetPosition(i + 1, targets[i].position); // Draw line to enemy pos(x,y)
            }
        }

        electricityLine.enabled = true;
        electricityTimer = electricityDuration; // Set timer how long to show the effect
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
            electricityDamage = baseElectricityDamage + (level - 1);
            stunDuration = baseStunDuration + ((level - 1) * 0.1f);

            // Increase chain targets every 2 levels
            if (level % 2 == 0)
            {
                chainTarget++;
            }

            CloseUpgradeUI();
        }
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
