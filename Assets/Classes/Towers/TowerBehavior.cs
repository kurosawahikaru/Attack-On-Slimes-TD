using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBehavior : MonoBehaviour
{

    public LayerMask EnemiesLayer;
    public Enemy Target;
    public Transform TowerPivot;
    public GameObject RangeSphere;

    public float Damage;
    public float Firerate;
    public float Range;
    public int SummonCost = 100;

    private float Delay;

    private IDamageMethod CurrentDamageMethodClass;

    // Start is called before the first frame update
    void Start()

    {
        RangeSphere.transform.localScale = new Vector3(Range * 2, Range * 2, Range * 2);
        CurrentDamageMethodClass = GetComponent<IDamageMethod>();

        if(CurrentDamageMethodClass == null)
        {
            Debug.LogError("Towers: no damage class attached to given tower");
        }
        else
        {
            CurrentDamageMethodClass.Init(Damage, Firerate);
        }

        
        Delay = 1 / Firerate;
    }

    public void Tick()
    {
        
        CurrentDamageMethodClass.DamageTick(Target);
        if (Target != null)
        {
            RangeSphere.transform.localScale = new Vector3(Range * 2, Range * 2, Range * 2); 
            TowerPivot.transform.rotation = Quaternion.LookRotation(Target.RootPart.transform.position-TowerPivot.transform.position);
        }
    }

    private void OnDrawGizmos()
    {
        if (Target != null)
        {
            Gizmos.DrawWireSphere(transform.position, Range);
            Gizmos.DrawLine(TowerPivot.position, Target.RootPart.transform.position);
        }
    }
}
