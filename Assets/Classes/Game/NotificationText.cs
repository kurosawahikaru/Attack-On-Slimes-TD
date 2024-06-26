using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class NotificationText : MonoBehaviour
{
    public float _destroyTime = 1.5f;

    void Start()
    {
        Destroy(gameObject,_destroyTime);
    }

}
