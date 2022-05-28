using UnityEngine;

public abstract class Ability: MonoBehaviour
{
    [Header("Параметры умения")]
    [field: SerializeField] protected int id;
    [field: SerializeField] protected string title;
    [field: SerializeField] protected Sprite image;
    [Space(10)]
    [field: SerializeField] protected float reuseTimer;
    [field: SerializeField] protected float duration;

    public abstract void Activate();

    public float GetDuration => duration;
    public float GetReuseTimer => reuseTimer;
    public Sprite GetImage => image;
}