using Interface;
using UnityEngine;

namespace Character
{
    public abstract class Character : MonoBehaviour, IHealth, IDamageable
    {
        public abstract float Health { get; set; }
    }
}