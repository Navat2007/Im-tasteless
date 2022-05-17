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
    public int strongMaxCount;
    public int fastMaxCount;
    [Space]
    public Color colorStandard = Color.red;
    public Color colorStrong = Color.gray;
    public Color colorFast = Color.blue;

    [Header("Параметры стандартного врага")]
    public int xpOnDeath = 1;
    public float damage = 10;
    public float moveSpeed = 5;
    public float health = 40;
}
