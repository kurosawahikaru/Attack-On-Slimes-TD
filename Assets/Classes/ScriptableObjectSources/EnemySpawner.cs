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
        StartCoroutine(LevelDesign(true, _currentLevel, 3.5f));

    }

    IEnumerator LevelDesign(bool status, int level, float waitTime)
    {
        int count = 0;
        int wave1, wave2, wave3, wave4, wave5;
        yield return new WaitForSeconds(waitTime);
        while (status == true)
        {
            if(level == 1)
            {
                wave1 = 2;
                wave2 = 5;
                wave3 = 9;
                wave4 = 14;
                wave5 = 20;

                for (int i = 0; i < wave1; i++)
                {
                    GameLoopManager.EnqueueEnemyIDToSummon(1);
                    count++;
             
                    yield return new WaitForSeconds(1);
                }
                yield return new WaitForSeconds(2);

                for (int i = 0; i < wave2 ; i++)
                {
                    GameLoopManager.EnqueueEnemyIDToSummon(1);
                    count++;
                    
                    yield return new WaitForSeconds(1);
                }
                yield return new WaitForSeconds(2);
               

                for (int i = 0; i < wave3 ; i++)
                {
                    if (i==2 || i==8)
                    {
                        GameLoopManager.EnqueueEnemyIDToSummon(2);
                        count++;
                        
                        yield return new WaitForSeconds(1);
                    }
                    else
                    {
                        GameLoopManager.EnqueueEnemyIDToSummon(1);
                        count++;
                        
                        yield return new WaitForSeconds(1);
                    }
                }
                yield return new WaitForSeconds(2);
             

                for (int i = 0; i < wave4; i++)
                {
                    if (i==0||i==5||i==10|i==12)
                    {
                        GameLoopManager.EnqueueEnemyIDToSummon(2);
                        count++;
                        
                        yield return new WaitForSeconds(1);
                    }
                    else if (i==9||i==13)
                    {
                        GameLoopManager.EnqueueEnemyIDToSummon(3);
                        count++;
                        
                        yield return new WaitForSeconds(1);
                    }
                    else
                    {
                        GameLoopManager.EnqueueEnemyIDToSummon(1);
                        count++;
                       
                        yield return new WaitForSeconds(1);
                    }


                }
                yield return new WaitForSeconds(2);
             

                for (int i = 0; i < wave5; i++)
                {
                    if (i<6|| i>16 )
                    {
                        GameLoopManager.EnqueueEnemyIDToSummon(3);
                        count++;
                        
                        yield return new WaitForSeconds(1);
                    }
                    else if (i%2!=0)
                    {
                        GameLoopManager.EnqueueEnemyIDToSummon(2);
                        count++;
                       
                        yield return new WaitForSeconds(1);
                    }
                    else
                    {
                        GameLoopManager.EnqueueEnemyIDToSummon(1);
                        count++;

                        yield return new WaitForSeconds(1);
                    }


                }
                yield return new WaitForSeconds(2);
          
                //if (count >= PlayerStatistics.GetMaxGoal())
                //{
                //    status = false;
                //    //PlayerStatistics.WinGame();
                //}
            }
            else if (level == 2)
            {

            }
            else
            {

            }


            if (count >= PlayerStatistics.GetMaxGoal())
            {
                GameLoopManager.EnqueueEnemyIDToSummon(1);
                count++;

                yield return new WaitForSeconds(1);
            }
            if (count >= 60) status = false;
        }
        yield return null;
    }
}
