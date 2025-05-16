using UnityEngine;

public class HurtBox2D : MonoBehaviour
{
    private HitManager ownerHitManager;

    private void Start()
    {
        ownerHitManager = GetComponentInParent<HitManager>();
    }

    public void ReceiveHit(HitData hitData)
    {
        ownerHitManager.SetLastHit(hitData);
        Debug.Log($"{gameObject.name} got hit! Damage: {hitData.damage}");
    }
}
