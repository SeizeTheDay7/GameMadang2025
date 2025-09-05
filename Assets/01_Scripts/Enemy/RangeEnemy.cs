using EditorAttributes;
using System;
using UnityEngine;

public class RangeEnemy : EnemyBase
{
    [Button]
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Attack()
    {
        StartCoroutine(CoAttack());
    }
}
