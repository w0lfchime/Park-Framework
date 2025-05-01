using UnityEngine;

public class HitBox2D : MonoBehaviour
{
    public float damage = 10f;
    public Vector2 knockback = new Vector2(5, 10);
    public float hitStun = 0.2f;

    private HitManager ownerHitManager;

    private void Start()
    {
        ownerHitManager = GetComponentInParent<HitManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HurtBox2D hurtBox = other.GetComponent<HurtBox2D>();
        if (hurtBox != null)
        {
            HitData hitData = new HitData(this, hurtBox, damage, knockback, hitStun);
            hurtBox.ReceiveHit(hitData);
        }
    }
}
