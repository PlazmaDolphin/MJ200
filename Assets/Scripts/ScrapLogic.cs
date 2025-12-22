using UnityEngine;

public class ScrapLogic : MonoBehaviour
{
    public void Die()
    {
        InventoryManager.Instance.PickupScrap();
        Destroy(this.gameObject);
    }
}
