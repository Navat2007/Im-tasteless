using UnityEngine;

public static class Helper
{
    public static bool GetCriticalChance(float chance)
    {
        return Random.Range(0, 101) < chance;
    } 
}