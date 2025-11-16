using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Timeline;
using TMPro;
using UnityEngine.Audio;

public class Turret : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform turretRotationPoint;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firingPoint;

    [Header("Upgrades")]
    [SerializeField] private GameObject upgradeUI;
    [SerializeField] private GameObject upgradeStatsUI;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button sellButton;
    [SerializeField] private TextMeshProUGUI upgradeCostText;
    [SerializeField] private TextMeshProUGUI sellValueText;

    [Header("Upgrade Stats")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI rangeText;
    [SerializeField] private TextMeshProUGUI bpsText;

    [Header("Audio")]
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioMixer mainAudioMixer;
    private AudioSource audioSource;
    
    [Header("Attributes")]
    [SerializeField] private float targetingRange = 5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float BPS = 1; // Bullet Per Second
    [SerializeField] private int baseUpgradeCost = 100;
    [SerializeField] private int maxLevel = 6; //Max level a turret can be upgraded to
    [SerializeField] int turretCosts = 100;
    [SerializeField] float sellPercentage = 0.55f;

    private float bpsBase;
    private float targetingRangeBase;

    private Transform target;
    private float timeUntilFire;

    private int level = 1;
    private int totalMoneySpent = 0;

    private void Start()
    {
        //Turrets' basic needs
        bpsBase = BPS;
        targetingRangeBase = targetingRange;
        totalMoneySpent = turretCosts; //tracks initial turret costs

        upgradeButton.onClick.AddListener(Upgrade); //Button Listener
        sellButton.onClick.AddListener(SellTurret); //Button Listener

        if (upgradeUI != null)
        {
            upgradeUI.SetActive(false);
        }

        UpgradeCost();

        //Audio source
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
    }

    private void Update()
    {

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

    private void Shoot()
    {
        //Play shooting sound
        if (shootSound != null)
        {
            audioSource.PlayOneShot(shootSound, GetSFXVolume());
        }


        //gets bullet prefab and shoots it at the enemy from the turrets firing point.
        GameObject bulletObj = Instantiate(bulletPrefab, firingPoint.position, Quaternion.identity);
        Bullet bulletScript = bulletObj.GetComponent<Bullet>();
        bulletScript.SetTarget(target);
    }

    private float GetSFXVolume()
    {
        float vol = 0f;
        mainAudioMixer.GetFloat("SFX", out vol);
        return Mathf.Pow(10f, vol / 20f);
    }

    private void FindTarget()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, targetingRange, (Vector2)transform.position, 0f, enemyMask);

        if (hits.Length > 0)
        {
            target = hits[0].transform;
        }
    }

    private bool CheckTargetIsInRange()
    {
        //Calculation on how to see if target in range
        return Vector2.Distance(target.position, transform.position) <= targetingRange;
    }

    private void RotateTowardsTarget()
    {
        //Rotating the top of the gun(barel) towards the target
        float angle = Mathf.Atan2(target.position.y - transform.position.y, target.position.x - transform.position.x) * Mathf.Rad2Deg - 90f;

        Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        turretRotationPoint.rotation = Quaternion.RotateTowards(turretRotationPoint.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void OpenUpgradeUI()
    {
        UIManager.main.ClearCurrentTurret();
        upgradeUI.SetActive(true);
        upgradeStatsUI.SetActive(true);
        UpgradeCost();
        UpdateUpgradeUI();

        UIManager.main.SetCurrentTurret(this);
    }

    public void CloseUpgradeUI()
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

        CloseUpgradeUI();
    }

    public void SellTurret()
    {
        int sellValue = Mathf.RoundToInt(totalMoneySpent * sellPercentage / level); //calculating how much the player gets back by doing all upgrades/tower value * x%
        LevelManager.main.IncreaseCurrency(sellValue); //give player money back
        CloseUpgradeUI();
        Destroy(gameObject);
    }

    private int UpgradeCalculator()
    {
        //Updates the new value of how much its going to cost the next upgrade for the newly upgraded turret/tower
        return Mathf.RoundToInt(baseUpgradeCost * Mathf.Pow(level, 0.8f));
    }

    private float BPSCalculator() {
        //Calculates the new BulletPerSecond speed if upgraded again
        return bpsBase * Mathf.Pow(level, 0.25f);
    }

    private float RangeCalculator()
    {
        //Calculates the new targeting range if upgraded again
        return targetingRangeBase * Mathf.Pow(level, 0.15f);
    }

    private void UpgradeCost()
    {
        if (upgradeCostText != null)
        {
            upgradeCostText.text = "$" + UpgradeCalculator();
        }

        if (level >= maxLevel)
        {
            upgradeButton.interactable = false;
            upgradeCostText.text = "MAX";
            upgradeCostText.color = Color.red;
        }

        if (sellValueText != null)
        {
            int sellValue = Mathf.RoundToInt(totalMoneySpent * sellPercentage / level);
            sellValueText.text = "$" + sellValue;
        }
    }

    private void UpdateUpgradeUI()
    {
        levelText.text = "Level: " + level + "/" + maxLevel;
        rangeText.text = "Range: " + targetingRange.ToString("F1");
        bpsText.text = "Fire Rate: " + BPS.ToString("F2");
    }
    
}
