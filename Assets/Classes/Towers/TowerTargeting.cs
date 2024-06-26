using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class TowerTargeting 
{
    public enum TargetType
    {
        First,
        Last,
        Close,
    }
    public static Enemy GetTarget(TowerBehavior CurrentTower,  TargetType TargetMethod)
    {
        Collider[] EnemiesInRange = Physics.OverlapSphere(CurrentTower.transform.position, CurrentTower.Range, CurrentTower.EnemiesLayer);

        NativeArray<EnemyData> EnemiesToCalculate = new(EnemiesInRange.Length,Allocator.TempJob);

        NativeArray<Vector3> NodePositions = new(GameLoopManager.NodePositions, Allocator.TempJob);

        NativeArray<float> NodeDistances= new(GameLoopManager.NodeDistances, Allocator.TempJob);

        NativeArray<int> EnemyToIndex = new(new int[] {-1}, Allocator.TempJob);

        int EnemyIndexToReturn = -1;


        for (int i = 0;i<EnemiesToCalculate.Length;i++)
        {
            Enemy CurrentEnemy = EnemiesInRange[i].transform.parent.GetComponent<Enemy>();
            int EnemyIndexInList = EntitySummoner.EnemiesInGame.FindIndex(x => x == CurrentEnemy);
            EnemiesToCalculate[i]=new EnemyData(CurrentEnemy.transform.position, CurrentEnemy.NodeIndex,CurrentEnemy.Health, EnemyIndexInList);
        }

        SearchForEnemy EnemySearchJob = new()
        {
            _EnemiesToCalculate = EnemiesToCalculate,
            _NodeDistances = NodeDistances,
            _NodePositions = NodePositions,
            _EnemyToIndex = EnemyToIndex,
            
            TargetingType = (int)TargetMethod,
            TowerPosition = CurrentTower.transform.position,
        };

        switch ((int)TargetMethod)
        {
            case 0:
                EnemySearchJob.CompareValue = Mathf.Infinity;
                break;
            case 1:
                EnemySearchJob.CompareValue= Mathf.NegativeInfinity;
                break;
            case 2:
                goto case 0;
            case 3:
                goto case 1;
            case 4:
                goto case 0;
        }

        JobHandle dependency = new();
        JobHandle SearchJobHandle = EnemySearchJob.Schedule(EnemiesToCalculate.Length, dependency);

        SearchJobHandle.Complete();

        if (EnemyToIndex[0] != -1)
        {
            EnemyIndexToReturn = EnemiesToCalculate[EnemyToIndex[0]].EnemyIndex;

            EnemiesToCalculate.Dispose();
            NodePositions.Dispose();
            NodeDistances.Dispose();
            EnemyToIndex.Dispose();

            return EntitySummoner.EnemiesInGame[EnemyIndexToReturn];
        }

        EnemiesToCalculate.Dispose();
        NodePositions.Dispose();
        NodeDistances.Dispose();
        EnemyToIndex.Dispose();

        return null;
        
    }

    struct EnemyData
    {
        public EnemyData(Vector3 position, int nodeindex, float hp, int enemyIndex)
        {
            EnemyPosition=  position;
            NodeIndex= nodeindex;
            EnemyIndex = enemyIndex;
            Health = hp;
        }
        public Vector3 EnemyPosition;
        public int EnemyIndex;
        public int NodeIndex;
        public float Health;
    }

    struct SearchForEnemy : IJobFor
    {
        [NativeDisableParallelForRestriction] public NativeArray<EnemyData> _EnemiesToCalculate;
        [NativeDisableParallelForRestriction] public NativeArray<Vector3> _NodePositions;
        [NativeDisableParallelForRestriction] public NativeArray<float> _NodeDistances;
        [NativeDisableParallelForRestriction] public NativeArray<int> _EnemyToIndex;
        public Vector3 TowerPosition;
        public float CompareValue;
        public int TargetingType;

        public void Execute(int index)
        {
            float DistanceToEnemy;
            float CurrentEnemyDistanceToEnd;
            switch (TargetingType)
            {
                case 0:
                    CurrentEnemyDistanceToEnd = GetDistanceToEnd(_EnemiesToCalculate[index]);
                    if (CurrentEnemyDistanceToEnd < CompareValue)
                    {
                        _EnemyToIndex[0] = index;
                        CompareValue = CurrentEnemyDistanceToEnd;
                    }
                    break;
                case 1:
                    CurrentEnemyDistanceToEnd = Vector3.Distance(TowerPosition, _EnemiesToCalculate[index].EnemyPosition);
                    if (CurrentEnemyDistanceToEnd > CompareValue)
                    {
                        _EnemyToIndex[0] = index;
                        CompareValue = CurrentEnemyDistanceToEnd;
                    }
                    break;
                case 2:
                    DistanceToEnemy = Vector3.Distance(TowerPosition, _EnemiesToCalculate[index].EnemyPosition);
                    if (DistanceToEnemy < CompareValue)
                    {
                        _EnemyToIndex[0] = index;
                        CompareValue = DistanceToEnemy;
                    }
                    break;
                case 3: //strong

                    if (_EnemiesToCalculate[index].Health > CompareValue)
                    {
                        _EnemyToIndex[0] = index;
                        CompareValue = _EnemiesToCalculate[index].Health;
                    }
                    break;
                case 4: //weak
                    if (_EnemiesToCalculate[index].Health < CompareValue)
                    {
                        _EnemyToIndex[0] = index;
                        CompareValue = _EnemiesToCalculate[index].Health;
                    }
                    break;
            }
        }

        private float GetDistanceToEnd(EnemyData EnemyToEvaluate)
        {
            float FinalDistance = Vector3.Distance(EnemyToEvaluate.EnemyPosition, _NodePositions[EnemyToEvaluate.NodeIndex]);

            for(int i = EnemyToEvaluate.NodeIndex; i < _NodeDistances.Length; i++)
            {
                FinalDistance += _NodeDistances[i];
            }

            return FinalDistance;
        }
    }
}
