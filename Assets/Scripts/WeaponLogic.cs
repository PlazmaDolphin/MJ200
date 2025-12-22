using UnityEngine;
//TODO: Add support for being unarmed
public class WeaponLogic : MonoBehaviour
{
    public GameObject weapon, bulletPrefab;
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
    // TODO: Add other weapons than knife
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lastAttack = Time.time - KNIFE_COOLDOWN;
        SwitchWeapon(weaponType);
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(0, 0, GetMouseAngle());
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
            GameObject bullet = Instantiate(bulletPrefab, weapon.transform.position, transform.rotation);
            bullet.GetComponent<Bullet>().initBullet(GetNormalizedMouseDirection() * GUN_VEL, GUN_DAMAGE, GUN_KNOCKBACK);
        }
        if (weaponType == 2)
        {
            grenade.StartGrenadeCharge();
        }
        Invoke(nameof(EndAttack), KNIFE_DURATION);
    }
    private float GetMouseAngle()
    {
        Vector2 mousePos = Input.mousePosition;
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        // if angle between 90 and -90, flip across y-axis
        if (mousePos.x < screenCenter.x) return -Mathf.Atan2(screenCenter.y - mousePos.y, mousePos.x - screenCenter.x) * Mathf.Rad2Deg + 180f;
        else return Mathf.Atan2(mousePos.y - screenCenter.y, mousePos.x - screenCenter.x) * Mathf.Rad2Deg;
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
