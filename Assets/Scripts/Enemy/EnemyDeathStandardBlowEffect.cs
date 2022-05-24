using System.Collections;
using UnityEngine;

public class EnemyDeathStandardBlowEffect : EnemyDeathEffect
{
    private void OnEnable()
    {
        healthSystem.OnDeath += OnDeath;
    }

    private void OnDisable()
    {
        healthSystem.OnDeath -= OnDeath;
    }
    
    protected override void OnDeath(ProjectileHitInfo projectileHitInfo)
    {
        healthSystem.enabled = false;
        
        enemy.GetEnemyController.OnDeath();
        
        StartCoroutine(Fade(materialColor, Color.grey, 10));
    }
    
    private IEnumerator Fade(Color from, Color to, float time)
    {
        float speed = 1 / time;
        float percent = 1;

        while (percent > 0)
        {
            percent -= Time.deltaTime * speed;
            material.color = Color.Lerp(to, from, percent);
            yield return null;
        }
        
        enemy.Die();
    }
}