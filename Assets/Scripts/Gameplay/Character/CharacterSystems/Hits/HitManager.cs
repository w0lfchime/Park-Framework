using UnityEngine;
using System.Collections.Generic;
using static UnityEngine.Analytics.IAnalytic;

public struct HitData
{
    public HitBox2D hitBox;
    public HurtBox2D hurtBox;
    public float damage;
    public Vector2 knockback;
    public float hitStun;

    public HitData(HitBox2D hitBox, HurtBox2D hurtBox, float damage, Vector2 knockback, float hitStun)
    {
        this.hitBox = hitBox;
        this.hurtBox = hurtBox;
        this.damage = damage;
        this.knockback = knockback;
        this.hitStun = hitStun;
    }
}


public class HitManager : MonoBehaviour
{
    public List<HitBox2D> hitBoxes = new List<HitBox2D>();
    public List<HurtBox2D> hurtBoxes = new List<HurtBox2D>();

    private HitData lastHitReceived;

    public void EnableHitBoxes(bool enable)
    {
        foreach (var hitBox in hitBoxes)
        {
            hitBox.gameObject.SetActive(enable);
        }
    }

    public void SetLastHit(HitData hitData)
    {
        lastHitReceived = hitData;
    }

    public HitData GetLastHit()
    {
        return lastHitReceived;
    }
}
