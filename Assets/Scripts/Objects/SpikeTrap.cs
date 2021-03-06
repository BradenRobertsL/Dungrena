﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class SpikeTrap : MonoBehaviour
{
    public int spikeCooldown;
    private float timePassed;
    public int trapDamage;
    private new BoxCollider2D collider2D;
    private Animator animator;
    void Awake()
    {
        animator = GetComponent<Animator>();

        collider2D = GetComponent<BoxCollider2D>();
        collider2D.isTrigger = true;
        collider2D.enabled = false;
    }

    private void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Spike"))
            return;

        if (timePassed > spikeCooldown)
        {
            ActivateTrap();
        }
        else
        {
            timePassed += Time.deltaTime;
        }
    }

    public void ActivateTrap()
    {
        animator.Play("Spike");
        timePassed = 0;
    }

    protected void SpikesOn()
    {
        collider2D.enabled = true;
        var bounds = new Bounds(collider2D.bounds.center, collider2D.bounds.size);
        bounds.Expand(0.8f);

        var guo = new GraphUpdateObject(bounds);
        guo.addPenalty = 2000;

        AstarPath.active.UpdateGraphs(guo);
    }

    protected void SpikesOff()
    {
        collider2D.enabled = false;
        var bounds = new Bounds(collider2D.bounds.center, collider2D.bounds.size);
        bounds.Expand(0.6f);
        var guo = new GraphUpdateObject(bounds);
        guo.addPenalty = 2000;

        AstarPath.active.UpdateGraphs(guo);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var entity = other.GetComponent<AbstractEntity>();

        if (entity == null) return;

        entity.ApplyAttack(trapDamage);
    }
}
