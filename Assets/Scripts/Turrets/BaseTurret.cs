using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public abstract class BaseTurret : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected Transform turretRotationPoint;
    [SerializeField] protected LayerMask enemyMask;
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] protected Transform firingPoint;

    [Header("Upgrades")]
    [SerializeField] protected GameObject upgradeUI;
    [SerializeField] protected GameObject upgradeStatsUI;
    [SerializeField] protected Button upgradeButton;
    [SerializeField] protected Button sellButton;
    [SerializeField] protected TextMeshProUGUI upgradeCostText;
    [SerializeField] protected TextMeshProUGUI sellValueText;

    [Header("Upgrade Stats")]
    [SerializeField] protected TextMeshProUGUI levelText;
    [SerializeField] protected TextMeshProUGUI rangeText;
    [SerializeField] protected TextMeshProUGUI bpsText;

    [Header("Audio")]
    [SerializeField] protected AudioClip shootSound;
    [SerializeField] protected AudioMixer mainAudioMixer;
    protected AudioSource audioSource;
    
    [Header("Attributes")]
    [SerializeField] protected float targetingRange = 5f;
    [SerializeField] protected float rotationSpeed = 5f;
    [SerializeField] protected float BPS = 1f;
    [SerializeField] protected int baseUpgradeCost = 100;
    [SerializeField] protected int maxLevel = 6;
    [SerializeField] protected int turretCosts = 100;
    [SerializeField] protected float sellPercentage = 0.55f;

    protected float bpsBase;
    protected float targetingRangeBase;
    protected Transform target;
    protected float timeUntilFire;
    protected int level = 1;
    protected int totalMoneySpent = 0;

    // Abstract methods that child classes must implement
    public abstract void OpenUpgradeUI();
    public abstract void CloseUpgradeUI();
    protected abstract void Shoot();

    // ALl the shared methods

    protected float GetSFXVolume()
    {
        float vol = 0f; // Storing volume value
        mainAudioMixer.GetFloat("SFX", out vol);
        return Mathf.Pow(10f, vol / 20f);
    }

    protected void FindTarget()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, targetingRange, (Vector2)transform.position, 0f, enemyMask); // Detecting all enemies in turret range

        if (hits.Length > 0) // If atleast 1 enemy was found:
        {
            target = hits[0].transform; // Set the first target as detected
        }
    }

    protected bool CheckTargetIsInRange()
    {
        return Vector2.Distance(target.position, transform.position) <= targetingRange; // return true if target is within range, false if not
    }

    protected void RotateTowardsTarget()
    {
        float angle = Mathf.Atan2(target.position.y - transform.position.y, target.position.x - transform.position.x) * Mathf.Rad2Deg - 90f; // Calculate the rotation of turret
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angle)); // Coverting the angle
        turretRotationPoint.rotation = Quaternion.RotateTowards(turretRotationPoint.rotation, targetRotation, rotationSpeed * Time.deltaTime); // Smooth rotations at X speed
    }

    protected int UpgradeCalculator()
    {
        return Mathf.RoundToInt(baseUpgradeCost * Mathf.Pow(level, 0.8f)); // Calculate upgrade cost based on current level
    }

    protected float BPSCalculator()
    {
        return bpsBase * Mathf.Pow(level, 0.25f); // Calculate bullets per second based on level
    }

    protected float RangeCalculator()
    {
        return targetingRangeBase * Mathf.Pow(level, 0.15f); // Calculate targeting range based on level
    }

    protected void UpdateUpgradeCost()
    {
        if (upgradeCostText != null)
        {
            upgradeCostText.text = "$" + UpgradeCalculator(); // Display cost to upgrade
        }

        if (level >= maxLevel) // If at max level:
        {
            upgradeButton.interactable = false; // Disable button to upgrade
            upgradeCostText.text = "MAX"; // change text to show max level reached
            upgradeCostText.color = Color.red; // change colour of text
        }

        if (sellValueText != null)
        {
            int sellValue = Mathf.RoundToInt(totalMoneySpent * sellPercentage / level); // Calculate sell value (% of money spent)
            sellValueText.text = "$" + sellValue; // Display sell value
        }
    }

    protected void UpdateUpgradeUI()
    {
        levelText.text = "Level: " + level + "/" + maxLevel; // Display current level out of max level (1/5 2/5 etc)
        rangeText.text = "Range: " + targetingRange.ToString("F1"); // Display targeting range to 1 decimal place
        bpsText.text = "Fire Rate: " + BPS.ToString("F1"); // Display bullets per second to 1 decimal place
    }
}