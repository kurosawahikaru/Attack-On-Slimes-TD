using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class EnemySpawner : MonoBehaviour
{
    private string _currentLevel;
    private PlayerStat PlayerStatistics;

    // Start is called before the first frame update
    void Start()
    {
        PlayerStatistics = FindObjectOfType<PlayerStat>();
        _currentLevel = SceneManager.GetActiveScene().name;
        StartCoroutine(LevelDesign(true, _currentLevel, 5f));

    }

    IEnumerator LevelDesign(bool status, string levelName, float waitTime)
    {
        int count = 0;
        int basicCount = 0;
        int quickCount = 0;
        int heavyCount = 0;
        yield return new WaitForSeconds(waitTime);
        while (status == true)
        {
            
            if (levelName =="Level 1")
            {
                //wave 1
                
                GameLoopManager.EnqueueEnemyIDToSummon(1);
                count++;
                yield return new WaitForSeconds(1);
            }

            if (count >= PlayerStatistics.GetMaxGoal()) status = false;
        }
        yield return null;
    }
}
