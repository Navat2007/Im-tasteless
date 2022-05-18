using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave", menuName = "ScriptableObjects/CreateWaveObject", order = 1)]
public class WaveSO : ScriptableObject
{
    [Header("Параметры волны")]
    public bool infinite;
    public int enemyCount = 20;
    public float timeBetweenSpawn = 2;
        
    [Header("Варианты врагов")]
    public List<Enemy> enemyList = new ();
    [Space]
    public float powerEnemyChance;
    public int fatMaxCount;
    public int fastMaxCount;

    [Header("Дополнительная прибавка к параметрам каждого врага")]
    public int xpOnDeath = 0;
    public float damage = 0;
    public float moveSpeed = 0;
    public float health = 0;
}
