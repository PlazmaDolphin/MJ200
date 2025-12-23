using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
//TODO: Add support for being unarmed
public class WeaponLogic : MonoBehaviour
{
<<<<<<< Updated upstream
<<<<<<< Updated upstream
    public GameObject weapon, bulletPrefab;
=======
=======
>>>>>>> Stashed changes
    public TextMeshProUGUI ammoDisplay;
    public RadialLoader radialLoader;
    public GameObject weapon, bulletPrefab, bulletSpawn;
>>>>>>> Stashed changes
    public Animator WeaponAnimator;
    public Sprite[] weaponSprites;
    public GrenadeGuide grenade;
    private int weaponType = 0; // 0: knife, 1: gun, 2: grenade.
    private const float KNIFE_DURATION = 0.25f;
    private const float KNIFE_COOLDOWN = 0.4f, GUN_COOLDOWN = 0.25f, GRENADE_COOLDOWN = 3f;
    private const float KNIFE_KNOCKBACK = 15f, GUN_KNOCKBACK = 5f;
    private const int KNIFE_DAMAGE = 1, GUN_DAMAGE = 2;
    private const float GUN_VEL = 10f;
    private float lastAttack;

    private Coroutine reloadRoutine;
    private bool isReloading;

    private int ammoClipSize = 6;
    private int currentAmmo;
    // TODO: Add other weapons than knife
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lastAttack = Time.time - KNIFE_COOLDOWN;
        SwitchWeapon(weaponType);
        currentAmmo = ammoClipSize;
        ammoDisplay.text = "Ammo: " + currentAmmo + " / " + ammoClipSize;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localRotation = Quaternion.Euler(0, 0, GetMouseAngle());
    }
    public void SwitchWeapon(int typeOffset)
    {
        weaponType += typeOffset;
        weaponType %= 3;
        if(weaponType < 0) weaponType = weaponSprites.Length-1;
        //set sprite
        weapon.GetComponent<SpriteRenderer>().sprite = weaponSprites[weaponType];
    }
    public void UseWeapon()
    {
        // if cooldown is active, do nothing
        if (Time.time - lastAttack < (weaponType == 0 ? KNIFE_COOLDOWN : weaponType == 1 ? GUN_COOLDOWN : GRENADE_COOLDOWN))
            return;
        weapon.GetComponent<Collider2D>().enabled = weaponType == 0; // Only enable collider for knife
        lastAttack = Time.time;
        if (weaponType == 0){
            WeaponAnimator.SetTrigger("stab");
            weapon.GetComponent<Collider2D>().enabled = true;
        }
        if (weaponType == 1)
        {
            // Fire bullet
<<<<<<< Updated upstream
<<<<<<< Updated upstream
            GameObject bullet = Instantiate(bulletPrefab, weapon.transform.position, transform.rotation);
            bullet.GetComponent<Bullet>().initBullet(GetNormalizedMouseDirection() * GUN_VEL, GUN_DAMAGE, GUN_KNOCKBACK);
=======
=======
>>>>>>> Stashed changes
            if (currentAmmo > 0)
            {
                GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.transform.position, transform.rotation);
                bullet.GetComponent<Bullet>().initBullet(GetNormalizedMouseDirection() * GUN_VEL, GUN_DAMAGE, GUN_KNOCKBACK);
                currentAmmo--;
                ammoDisplay.text = "Ammo: " + currentAmmo + " / " + ammoClipSize;
            }
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
        }
        if (weaponType == 2)
        {
            grenade.StartGrenadeCharge();
        }
        Invoke(nameof(EndAttack), KNIFE_DURATION);
    }

    public void BeginReloadHold()
    {
        if (isReloading) return;
        if (weaponType != 1) return;
        if (InventoryManager.Instance.ScrapCount <= 0) return;
        if (currentAmmo >= ammoClipSize) return; // optioneel

        reloadRoutine = StartCoroutine(ReloadHoldRoutine(1f));
    }

    public void CancelReloadHold()
    {
        if (!isReloading) return;

        if (reloadRoutine != null)
        {
            StopCoroutine(reloadRoutine);
            reloadRoutine = null;
        }

        isReloading = false;
        radialLoader.CancelLoading();
    }

    private IEnumerator ReloadHoldRoutine(float duration)
    {
        isReloading = true;
        radialLoader.StartLoading(duration);

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            radialLoader.SetProgress(t / duration);
            yield return null;
        }

        FinishReload();
        isReloading = false;
        reloadRoutine = null;
        radialLoader.CompleteAndHide();
    }

    private void FinishReload()
    {
        currentAmmo = ammoClipSize;
        InventoryManager.Instance.RemoveScrap(1);
        ammoDisplay.text = "Ammo: " + currentAmmo + " / " + ammoClipSize;
    }

    private float GetMouseAngle()
    {
        Vector3 ms = Mouse.current.position.ReadValue();
        ms.z = -Camera.main.transform.position.z;
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(ms);
        Vector3 worldDir = mouseWorld - transform.parent.position;
        Vector3 localDir = transform.parent.InverseTransformVector(worldDir);

        float angle = Mathf.Atan2(localDir.y, localDir.x) * Mathf.Rad2Deg;
        return angle;
    }

    private Vector2 GetNormalizedMouseDirection()
    {
        Vector2 mousePos = Input.mousePosition;
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        return (mousePos - screenCenter).normalized;
    }

    private void EndAttack()
    {
        weapon.GetComponent<Collider2D>().enabled = false;
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision with " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("police"))
        {
            collision.gameObject.GetComponent<PoliceLogic>().damage(transform.position, KNIFE_KNOCKBACK, 1);
        }
    }
}
