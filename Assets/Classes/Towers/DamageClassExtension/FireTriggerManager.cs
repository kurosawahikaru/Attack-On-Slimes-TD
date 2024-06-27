using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTriggerManager : MonoBehaviour
{
    [SerializeField] private FlamethrowerDamage BaseClass;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Effect FlameEffect = new("Fire", BaseClass.Firerate, (BaseClass.Damage)/2, 3f);
            ApplyEffectData EffectData = new(EntitySummoner.EnemyTransformPairs[other.transform.parent], FlameEffect);
            GameLoopManager.EnqueueEffectToApply(EffectData);
        }
    }

}
