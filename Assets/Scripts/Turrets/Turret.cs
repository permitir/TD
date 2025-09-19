using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Timeline;

public class Turret : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform turretRotationPoint;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firingPoint;
    [SerializeField] private GameObject upgradeUI;
    [SerializeField] private Button upgradeButton;
    
    [Header("Attributes")]
    [SerializeField] private float targetingRange = 5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float BPS = 1; // Bullet Per Second
    [SerializeField] private int baseUpgradeCost = 100;

    private float bpsBase;
    private float targetingRangeBase;

    private Transform target;
    private float timeUntilFire;

    private int level = 1;

    private void Start()
    {
        //Turrets' basic needs
        bpsBase = BPS;
        targetingRangeBase = targetingRange;

        upgradeButton.onClick.AddListener(Upgrade);
    }

    private void Update()
    {
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
        //gets bullet prefab and shoots it at the enemy from the turrets firing point.
        GameObject bulletObj = Instantiate(bulletPrefab, firingPoint.position, Quaternion.identity);
        Bullet bulletScript = bulletObj.GetComponent<Bullet>();
        bulletScript.SetTarget(target);
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
        upgradeUI.SetActive(true);
    }

    public void CloseUpgradeUI()
    {
        upgradeUI.SetActive(false);
        UIManager.main.SetHoveringState(false);
    }

    public void Upgrade()
    {
        //What happens after user buys a turret/tower upgrade
        if (UpgradeCalculator() > LevelManager.main.currency) return;

        LevelManager.main.SpendCurrency(UpgradeCalculator());

        level++;

        BPS = BPSCalculator();
        targetingRange = RangeCalculator();

        CloseUpgradeUI();
        Debug.Log("New BPS: " + BPS);
        Debug.Log("New Targeting Range: " + targetingRange);
        Debug.Log("New cost: " + UpgradeCalculator());
    }

    private int UpgradeCalculator()
    {
        //Updates the new value of how much its going to cost the next upgrade for the newly upgraded turret/tower
        return Mathf.RoundToInt(baseUpgradeCost * Mathf.Pow(level, 0.8f));
    }

    private float BPSCalculator() {
        //Calculates the new BulletPerSecond speed if upgraded again
        return bpsBase * Mathf.Pow(level, 0.4f);
    }

    private float RangeCalculator()
    {
        //Calculates the new targeting range if upgraded again
        return targetingRangeBase * Mathf.Pow(level, 0.2f);
    }

    private void OnDrawGizmosSelected()
    {
        //How I can see the turret/tower range
        Handles.color = Color.green;
        Handles.DrawWireDisc(transform.position, transform.forward, targetingRange);
    }
    
}
