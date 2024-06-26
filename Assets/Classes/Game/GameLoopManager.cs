using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

public class GameLoopManager : MonoBehaviour
{
    
    public static List<TowerBehavior> TowersInGame;
    public static Vector3[] NodePositions;
    public static float[] NodeDistances;


    private static Queue<ApplyEffectData> EffectsQueue;
    private static Queue<EnemyDamageData> DamageData;
    private static Queue<Enemy> EnemiesToRemove;
    private static Queue<int> EnemyIDsToSummon;

    private PlayerStat PlayerStatistics;

    public Transform NodeParent;
    public bool LoopShouldEnd;

    // Start is called before the first frame update
    void Start()
    {
        PlayerStatistics = FindObjectOfType<PlayerStat>();
        EffectsQueue = new Queue<ApplyEffectData>();
        DamageData = new Queue<EnemyDamageData>();
        TowersInGame = new List<TowerBehavior>();

        EnemyIDsToSummon = new Queue<int>();    
        EnemiesToRemove = new Queue<Enemy>();
        EntitySummoner.Init();

        NodePositions = new Vector3[NodeParent.childCount];
        for(int i = 0; i< NodePositions.Length; i++)
        {
            NodePositions[i]=NodeParent.GetChild(i).position;
        }

        NodeDistances = new float[NodePositions.Length - 1];
        for (int i = 0; i < NodeDistances.Length; i++)
        {
            NodeDistances[i] = Vector3.Distance(NodePositions[i], NodePositions[i+1]);
        }

        

        StartCoroutine(GameLoop());

        //InvokeRepeating(nameof(SummonTest), 0f, 1f);
        
    }

    IEnumerator GameLoop()
    {


        while (LoopShouldEnd==false)
        {
            //spawn enemies
            if (EnemyIDsToSummon.Count > 0)
            {
                for(int i=0; i<EnemyIDsToSummon.Count; i++)
                {
                    EntitySummoner.SummonEnemy(EnemyIDsToSummon.Dequeue());
                }
            }

            //spawn towers
            //move enemies

            NativeArray<Vector3> NodesToUse = new(NodePositions, Allocator.TempJob);
            NativeArray<int> NodeIndices = new(EntitySummoner.EnemiesInGame.Count, Allocator.TempJob);
            NativeArray<float> EnemySpeeds = new(EntitySummoner.EnemiesInGame.Count, Allocator.TempJob);
            TransformAccessArray EnemyAccess = new(EntitySummoner.EnemiesInGameTransform.ToArray(), 2);

            for(int i = 0; i < EntitySummoner.EnemiesInGame.Count; i++)
            {
                EnemySpeeds[i] = EntitySummoner.EnemiesInGame[i].Speed;
                NodeIndices[i] = EntitySummoner.EnemiesInGame[i].NodeIndex;
            }

            MoveEnemiesJob MoveJob = new()
            {
                NodePositions = NodesToUse,
                EnemySpeed = EnemySpeeds,
                NodeIndex = NodeIndices,
                deltaTime = Time.deltaTime
            };

            JobHandle MoveJobHandle = MoveJob.Schedule(EnemyAccess);
            MoveJobHandle.Complete();

            for(int i=0; i < EntitySummoner.EnemiesInGame.Count; i++)
            {
                EntitySummoner.EnemiesInGame[i].NodeIndex = NodeIndices[i];

                if (EntitySummoner.EnemiesInGame[i].NodeIndex == NodePositions.Length)
                {
                    EnqueueEnemyToRemove(EntitySummoner.EnemiesInGame[i]);
                    PlayerStatistics.AddLife(-1);
                    if (0 >= PlayerStatistics.GetLife())
                    {
                        PlayerStatistics.GameOver();
                    }
                }
            }
            
            NodesToUse.Dispose();
            EnemySpeeds.Dispose();
            EnemyAccess.Dispose();
            NodeIndices.Dispose();

            //tick towers

            foreach(TowerBehavior tower in TowersInGame)
            {
                tower.Target = TowerTargeting.GetTarget(tower, TowerTargeting.TargetType.Close);
                tower.Tick();
                
            }
            //apply effects
            if (EffectsQueue.Count > 0)
            {
                
                for (int i = 0; i < EffectsQueue.Count; i++)
                {

                    ApplyEffectData CurrentDamageData = EffectsQueue.Dequeue();
                    Effect EffectDuplicate = CurrentDamageData.EnemyToAffect.ActiveEffects.Find(x => x.EffectName == CurrentDamageData.EffectToApply.EffectName);

                    if (EffectDuplicate == null)
                    {
                        CurrentDamageData.EnemyToAffect.ActiveEffects.Add(CurrentDamageData.EffectToApply);
                    }
                    else
                    {
                        EffectDuplicate.ExpireTime = CurrentDamageData.EffectToApply.ExpireTime;
                    }

                }
            }

            //tick enemies
            foreach (Enemy CurrentEnemy in EntitySummoner.EnemiesInGame)
            {
                CurrentEnemy.Tick();
            }

            //damage enemies

            if (DamageData.Count > 0)
            {
                
                for (int i = 0; i < DamageData.Count; i++)
                {
                    
                    EnemyDamageData CurrentDamageData = DamageData.Dequeue();
                    CurrentDamageData.TargetedEnemy.Health -= CurrentDamageData.TotalDamage / CurrentDamageData.Resistance;
                    CurrentDamageData.TargetedEnemy._healthBar.UpdateHealthBar(CurrentDamageData.TargetedEnemy.MaxHealth, CurrentDamageData.TargetedEnemy.Health);
                    

                    if (CurrentDamageData.TargetedEnemy.Health <= 0f)
                    {
                        if (!EnemiesToRemove.Contains(CurrentDamageData.TargetedEnemy))
                        {
                            EnqueueEnemyToRemove(CurrentDamageData.TargetedEnemy);
                            PlayerStatistics.AddMoney((int)CurrentDamageData.TargetedEnemy.MaxHealth);
                            PlayerStatistics.AddGoal(1);
                            if (PlayerStatistics.WinCondition()) PlayerStatistics.WinGame();
                        }
                    }
                }
            }
            //remove enemies

            if (EnemiesToRemove.Count > 0)
            {
                for (int i = 0; i < EnemiesToRemove.Count; i++)
                {
                    EntitySummoner.RemoveEnemy(EnemiesToRemove.Dequeue());
                }
            }
            
            //remove towers
            
            

            yield return null;



        }
    }



    public static void EnqueueEffectToApply(ApplyEffectData effectData)
    {
        EffectsQueue.Enqueue(effectData);
    }

    public static void EnqueueDamageData(EnemyDamageData damagedata)
    {
        DamageData.Enqueue(damagedata);
    }
    public static void EnqueueEnemyIDToSummon(int ID)
    {
        EnemyIDsToSummon.Enqueue(ID);
    }

    public static void EnqueueEnemyToRemove(Enemy EnemyToRemove)
    {
        EnemiesToRemove.Enqueue(EnemyToRemove);
    }


}

public class Effect
{

    public Effect(string effectName,  float damageRate, float damage, float expireTime)
    {
        EffectName = effectName;
        DamageRate = damageRate;
        ExpireTime = expireTime;
        Damage = damage;

    }
    public string EffectName;

    public float Damage;
    public float ExpireTime;
    public float DamageRate;
    public float DamageDelay;
}

public struct ApplyEffectData
{
    public ApplyEffectData(Enemy enemytoAffect, Effect effectToApply)
    {
        EnemyToAffect = enemytoAffect;
        EffectToApply = effectToApply;
    }

    public Enemy EnemyToAffect;
    public Effect EffectToApply;
}

public struct EnemyDamageData
{
    public EnemyDamageData(Enemy target, float damage, float resistance)
    {
        TargetedEnemy= target;
        TotalDamage= damage;
        Resistance= resistance;
    }

    public Enemy TargetedEnemy;
    public float TotalDamage;
    public float Resistance;
}

public struct MoveEnemiesJob : IJobParallelForTransform
{
    [NativeDisableParallelForRestriction]
    public NativeArray<int> NodeIndex;

    [NativeDisableParallelForRestriction]
    public NativeArray<float> EnemySpeed;

    [NativeDisableParallelForRestriction]
    public NativeArray<Vector3> NodePositions;


    public float deltaTime;
    public void Execute( int index, TransformAccess transform)
    {
        if (NodeIndex[index]< NodePositions.Length)
        {
            Vector3 PositionToMoveTo = NodePositions[NodeIndex[index]];

            transform.position = Vector3.MoveTowards(transform.position, PositionToMoveTo, EnemySpeed[index] * deltaTime);

            if (transform.position == PositionToMoveTo)
            {
                NodeIndex[index]++;
            }
        }


    }
}