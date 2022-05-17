using System;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private int scoreOnTime = 5;
    [SerializeField] private float currentTime;

    public EventHandler<float> OnTimerChange;

    private float _scoreTimerMax = 1f;
    private float _scoreTimer = 0;

    private void OnEnable()
    {
        //GameManager.LevelManager.OnExit += Reset;
    }
    
    private void OnDisable()
    {
        //GameManager.LevelManager.OnExit -= Reset;
    }

    private void Update()
    {
        /*
        if (GameManager.Instance.GetState == GameState.PLAY)
        {
            
        }
        */
        
        currentTime += Time.deltaTime;
        OnTimerChange?.Invoke(this, currentTime);

        _scoreTimer += Time.deltaTime;

        if (_scoreTimer >= _scoreTimerMax)
        {
            _scoreTimer = 0;
            //CurrencyManager.Instance.AddCurrency(Currency.SCORE, scoreOnTime);
        }
    }

    public void Reset()
    {
        currentTime = 0;
    }
}
