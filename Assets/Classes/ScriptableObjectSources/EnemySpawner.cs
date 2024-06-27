using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class EnemySpawner : MonoBehaviour
{
    private int _currentLevel;
    private PlayerStat PlayerStatistics;

    // Start is called before the first frame update
    void Start()
    {
        PlayerStatistics = FindObjectOfType<PlayerStat>();
        _currentLevel = PlayerStatistics.GetLevel();
        Debug.Log(_currentLevel);
        StartCoroutine(LevelDesign(true, _currentLevel, 5f));

    }

    IEnumerator LevelDesign(bool status, int level, float waitTime)
    {
        int count = 0;
        int basicCount = 0;
        int quickCount = 0;
        int heavyCount = 0;
        int wave1, wave2, wave3, wave4 = 0;
        yield return new WaitForSeconds(waitTime);
        while (status == true)
        {
            if(level == 1)
            {
                wave1 = 2;
                wave2 = 5;
                wave3 = 9;
                wave4 = 14;

                for (int i = 0; i < wave1; i++)
                {
                    GameLoopManager.EnqueueEnemyIDToSummon(1);
                    count++;
                    basicCount++;
                    yield return new WaitForSeconds(1);
                }
                yield return new WaitForSeconds(3);

                for (int i = 0; i < wave2 ; i++)
                {
                    GameLoopManager.EnqueueEnemyIDToSummon(1);
                    count++;
                    basicCount++;
                    yield return new WaitForSeconds(1);
                }
                yield return new WaitForSeconds(3);
                basicCount = 0;

                for (int i = 0; i < wave3 ; i++)
                {
                    if ((basicCount == 3 && heavyCount==0) || (basicCount == 7 && heavyCount==1))
                    {
                        GameLoopManager.EnqueueEnemyIDToSummon(2);
                        count++;
                        heavyCount++;
                        yield return new WaitForSeconds(1);
                    }
                    else
                    {
                        GameLoopManager.EnqueueEnemyIDToSummon(1);
                        count++;
                        basicCount++;
                        yield return new WaitForSeconds(1);
                    }
                }
                yield return new WaitForSeconds(3);
                basicCount = 0;
                heavyCount = 0;

                for (int i = 0; i < wave4; i++)
                {
                    if ((basicCount == 0 && heavyCount == 0) || (basicCount == 4 && heavyCount == 1) || (basicCount == 8 && heavyCount == 2) || (basicCount == 10 && heavyCount == 3))
                    {
                        GameLoopManager.EnqueueEnemyIDToSummon(2);
                        count++;
                        heavyCount++;
                        yield return new WaitForSeconds(1);
                    }
                    else
                    {
                        GameLoopManager.EnqueueEnemyIDToSummon(1);
                        count++;
                        basicCount++;
                        yield return new WaitForSeconds(1);
                    }
                    
                    
                }
                yield return new WaitForSeconds(3);
                basicCount = 0;
                heavyCount = 0;
                if (count >= PlayerStatistics.GetMaxGoal())
                {
                    status = false;
                    //PlayerStatistics.WinGame();
                }
            }
            else if (level == 2)
            {

            }
            else
            {

            }


            if (count >= PlayerStatistics.GetMaxGoal()) status = false;
        }
        yield return null;
    }
}
