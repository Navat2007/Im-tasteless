using UnityEngine;

public abstract class Ability
{
    [Header("Параметры умения")]
    [field: SerializeField] protected int id;
    [field: SerializeField] protected string title;
    [field: SerializeField] protected Sprite image;
    [field: SerializeField] protected float reuseTimer;

    public abstract void Activate();
}