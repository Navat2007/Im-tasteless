using System.Collections;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private float stopPauseSpeed = 10;
    
    private void OnEnable()
    {
        ControllerManager.pauseManager = this;
    }

    private void OnDisable()
    {
        ControllerManager.pauseManager = null;
    }

    public void StartPause()
    {
        StopCoroutine(SmoothPause());
        Time.timeScale = 0;
        Cursor.visible = true;
        GameManager.SetGameState(GameManager.GameState.PAUSE);
    }
    
    public void StopPause(bool smooth = true)
    {
        if(smooth)
            StartCoroutine(SmoothPause());
        else
        {
            Time.timeScale = 1;
            GameManager.SetGameState(GameManager.GameState.PLAY); 
        }
        
        Cursor.visible = false;
    }

    private IEnumerator SmoothPause()
    {
        GameManager.SetGameState(GameManager.GameState.PLAY); 
        
        while (Time.timeScale < 1)
        {
            Time.timeScale += 0.001f * stopPauseSpeed;
            yield return null;
        }
    }
}