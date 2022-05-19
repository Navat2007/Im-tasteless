using UnityEngine;

public static class Helper
{
    public static bool IsCritical(float chance)
    {
        return Random.Range(0, 101) < chance;
    } 
}