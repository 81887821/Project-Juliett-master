﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MissileBody : Enemy
{
    [Space]
    public MissileHead SpareHead;
    [Space]
    public Sprite NormalBodySprite;
    public Sprite HitBodySprite;
    [Space]
    public float HeadLaunchInterval = 3f;

    private MissileHead currentHead;
    private float hitStateEndTime;
    private bool attackEnabled = true;
    private float nextHeadLaunch;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        SpareHead.gameObject.SetActive(false);
        LaunchHead();
    }

    protected override void Update()
    {
        base.Update();

        float now = Time.time;
        if (now >= hitStateEndTime)
        {
            attackEnabled = true;
            spriteRenderer.sprite = NormalBodySprite;
        }
        if (now >= nextHeadLaunch)
            LaunchHead();
    }

    public override void Die()
    {
        hitStateEndTime = float.MaxValue;
        attackEnabled = false;
        nextHeadLaunch = float.MaxValue;
        if (currentHead != null)
            currentHead.OnBodyDestroyed();
        base.Die();
    }

    public override void OnAttack(IInteractable target)
    {
        if (attackEnabled)
            target.OnDamaged(this, 1);
    }

    public override void OnDamaged(IInteractable attacker, int damage, Vector2 knockback)
    {
        if (attacker.Equals(currentHead))
            currentHead = null;

        hitStateEndTime = Time.time + 1f;
        attackEnabled = false;
        spriteRenderer.sprite = HitBodySprite;

        base.OnDamaged(attacker, damage, knockback);
    }

    protected override void UpdateVelocity()
    {
        const float targetVelocityX = 0f;

        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.Collisions.below) ? AccelerationTimeGrounded : AccelerationTimeAirborne);
        velocity.y += Gravity * Time.deltaTime;
    }

    private void LaunchHead()
    {
        currentHead = Instantiate(SpareHead, transform);
        currentHead.gameObject.SetActive(true);
        nextHeadLaunch = Time.time + HeadLaunchInterval;
    }
}
