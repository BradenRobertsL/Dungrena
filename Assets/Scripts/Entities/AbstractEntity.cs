﻿using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class AbstractEntity : MonoBehaviour, IDamageable
{

    [Header("Physics Values")]
    public int maxSpeed = 7;
    public float acceleration = 1;
    public float friction = 1.9f;

    protected Vector2 moveDirection;

    [Header("Player Stats")]
    [SerializeField]
    private float maxHealth;
    private float health;

    private float pickUpRange = 1.5f;
    private Vector3 pickUpOffset = new Vector2(0, 0.5f);

    [HideInInspector]
    public bool canMove = true;
    [HideInInspector]
    public bool applyFriction = true;
    [HideInInspector]
    public bool attacking = false;

    protected Rigidbody2D rb2d;
    protected Collider2D collider2d;
    protected GameEvent onCollision;

    public LayerMask layerMask; // TODO: Remove layermask from public variables

    private void Awake()
    {
        health = maxHealth;
        rb2d = GetComponent<Rigidbody2D>();
    }

    public virtual void UpdatePhysics()
    {

        // TODO: friction and acceleration multipliers
        var _friction = friction;

        Vector2 newVel = rb2d.velocity;

        if (canMove) { newVel = UpdateMovement(newVel); }

        if (applyFriction)
        {
            // Friction when not moving
            if (moveDirection.x == 0)
                newVel.x = Approach(newVel.x, 0, _friction / 2);
            if (moveDirection.y == 0)
                newVel.y = Approach(newVel.y, 0, _friction / 2);
        }

        rb2d.velocity = newVel;
    }

    public virtual Vector2 UpdateMovement(Vector2 newVel)
    {
        // Left
        if (moveDirection.x < 0)
        {
            // Apply acceleration left
            if (newVel.x > 0)
                newVel.x = Approach(newVel.x, 0, friction);
            newVel.x = Approach(newVel.x, moveDirection.x * maxSpeed, acceleration);
        }

        // Right
        if (moveDirection.x > 0)
        {
            // Apply acceleration right
            if (newVel.x < 0)
                newVel.x = Approach(newVel.x, 0, friction);
            newVel.x = Approach(newVel.x, moveDirection.x * maxSpeed, acceleration);
        }

        // Down
        if (moveDirection.y < 0)
        {
            // Apply acceleration down
            if (newVel.y > 0)
                newVel.y = Approach(newVel.y, 0, friction);
            newVel.y = Approach(newVel.y, moveDirection.y * maxSpeed, acceleration);
        }

        // Up
        if (moveDirection.y > 0)
        {
            // Apply acceleration up
            if (newVel.y < 0)
                newVel.y = Approach(newVel.y, 0, friction);
            newVel.y = Approach(newVel.y, moveDirection.y * maxSpeed, friction);
        }

        return newVel;
    }

    // TODO: Handle death event.
    public void Damage(float amount)
    {
        health -= amount;
    }

    public void ApplyAttack(float damage, float knockback, AbstractEntity attacker)
    {
        // Damage
        Damage(damage);

        // Knockback
        var knockbackDirection = attacker.transform.position - transform.position;
        AddForce(knockbackDirection.normalized, knockback);

        // TODO: Damage/Bleeding effect + Hit flash
    }

    // Set movement input
    public void SetMovement(Vector2 direction)
    {
        moveDirection = direction;
    }

    // For adding extra velocity without moveSpeed bounds to rb2d
    public void AddVelocity(Vector2 direction, float speed)
    {
        rb2d.velocity += (direction.normalized * speed);
    }

    // For adding force to rb2d
    public void AddForce(Vector2 direction, float strength) {
        rb2d.AddForce(direction.normalized * strength);
    }

    // Reset velocity
    public void ResetVelocity()
    {
        rb2d.velocity = Vector2.zero;
    }

    // Approach end from start at an increment of shift
    public float Approach(float start, float end, float shift)
    {
        float val = 0;

        if (start < end)
        {
            val = Mathf.Min(start + shift, end);
        }
        else
        {
            val = Mathf.Max(start - shift, end);
        }

        return val;
    }

    // Pick up 
    public void pickupItem()
    {
        Debug.Log("Picking up Item");
        IPickupable itemToPickup = null;

        // Search for nearby items
        Collider2D[] items = Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), layerMask);

        // No items
        if (items.Length == 0)
            return;

        // Find eligible item
        foreach (var item in items)
        {
            float distance = Vector2.Distance(transform.position, item.transform.position);

            if (distance > pickUpRange)
                continue;

            itemToPickup = item.GetComponent<IPickupable>();
        }

        // Pickup closes
        itemToPickup.pickup(this);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position + pickUpOffset, pickUpRange);
    }
}
