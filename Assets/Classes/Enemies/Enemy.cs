using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int NodeIndex;

    public List<Effect> ActiveEffects;

    public Transform RootPart;
    public float DamageResistance=1f;
    public float MaxHealth;
    public float Health;
    public float Speed;
    public int ID;

    public HealthBar _healthBar;

    public void Init()
    {
        ActiveEffects = new List<Effect>();
        Health = MaxHealth;
        transform.position = GameLoopManager.NodePositions[0];
        NodeIndex = 0;
    }

    public void Tick()
    {
        for (int i = 0; i < ActiveEffects.Count; ++i)
        {
            if (ActiveEffects[i].ExpireTime > 0f)
            {
                if (ActiveEffects[i].DamageDelay > 0f)
                {
                    ActiveEffects[i].DamageDelay -=Time.deltaTime;

                }
                else
                {
                    GameLoopManager.EnqueueDamageData(new EnemyDamageData(this, ActiveEffects[i].Damage, 1f));
                    ActiveEffects[i].DamageDelay = 1f / ActiveEffects[i].DamageRate;
                }
            }

            ActiveEffects[i].ExpireTime -= Time.deltaTime;
        }

        ActiveEffects.RemoveAll(x=> x.ExpireTime < 0f);
    }
}

