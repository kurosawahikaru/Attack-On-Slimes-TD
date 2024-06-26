using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemySpawner : MonoBehaviour
{
    private float _waitTime = 3f;
    private string _currentLevel;

    // Start is called before the first frame update
    void Start()
    {
        _currentLevel = SceneManager.GetActiveScene().name;
        StartCoroutine(LevelDesign(true, _currentLevel, _waitTime));
    }

    IEnumerator LevelDesign(bool status, string levelName, float waitTime)
    {
        int count = 0;
        yield return new WaitForSeconds(waitTime);
        while (status == true)
        {
            yield return new WaitForSeconds(1);
            GameLoopManager.EnqueueEnemyIDToSummon(1);
            count++;
            if (count >= 10) status = false;
        }
        yield return null;
    }
}
