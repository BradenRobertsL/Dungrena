﻿using UnityEngine;

public class PatrolState : AbstractEnemyState
{
    public override string Name { get { return "Patrol"; } }

    public float maxDistance = 10;
    public float maxPatrolInterval = 20;

    private float patrolInterval = 0;

    public override void init()
    {
        base.init();
        Executing = true;
        enemyAI.targetPosition = pickNewTarget();
        enemyAI.reachedEndOfPath = false;
        patrolInterval = Random.Range(0, maxPatrolInterval);
    }

    public override void execute(AbstractEnemyAI enemyAI)
    {
        enemyAI.AstarMoveToTarget();
        enemyAI.UpdatePhysics();

        if (enemyAI.reachedEndOfPath)
        {
            Executing = false;
        }
    }

    public override bool conditionsMet(AbstractEnemyAI enemyAI)
    {
        if (enemyAI.currentState.Name == "Patrol")
        {
            if (!enemyAI.reachedEndOfPath)
                return true;
        }
        else if (enemyAI.currentState.Name == "Idle")
        {
            IdleState idleState = (IdleState) enemyAI.currentState;

            if (idleState.timeIdle > patrolInterval)
                return true;
        }

        return false;
    }

    private Vector2 pickNewTarget()
    {
        Vector2 target = new Vector2(transform.position.x, transform.position.y) + Random.insideUnitCircle * Random.Range(maxDistance/9, maxDistance);
        return target;
    }
}