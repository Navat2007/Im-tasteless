using System;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private float currentTime;

    public EventHandler<float> OnTimerChange;

    private float _nextTime;
    private float _scoreTimerMax = 1f;
    private float _scoreTimer = 0;

    private void Update()
    {
        if (Time.time > _nextTime)
        {
            _nextTime = Time.time + 1;
            
            currentTime += 1;
            OnTimerChange?.Invoke(this, currentTime);

            _scoreTimer += 1;

            if (_scoreTimer >= _scoreTimerMax)
            {
                _scoreTimer = 0;
            }
        }
        
    }

    public void Reset()
    {
        currentTime = 0;
    }
}
