using System;
using UnityEngine;

public static class EventBus
{
    public static class PlayerEvents
    {
        public static Action OnDeath;
        public static Action OnRevive;
    }
    
    public static class SpawnerEvents
    {
        public static Action<int, int> OnNewWave;
        public static Action OnSpawn;
    }
    
    public static class EnemyEvents
    {
        public static Action<int> OnEnemyCountChange;
        public static Action OnSpawn;
    }
    
    public static class InputEvents
    {
        public static Action<Vector2> OnInputMoveChange;
    }
    
    public static class AdsEvents
    {
        public static Action OnAdsNeedToShow;
        public static Action OnAdsShown;
        public static Action OnAdsClose;
        public static Action OnAdsFailed;
    }
}