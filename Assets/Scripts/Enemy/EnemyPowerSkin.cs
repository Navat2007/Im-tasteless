using System.Collections;
using UnityEngine;

public class EnemyPowerSkin : MonoBehaviour
{
    [SerializeField] private Color color;
    [SerializeField] private float blinkDuration = 0.4f;

    private Renderer _renderer;
    private Material _material;
    private Color _color;
    private bool _isWorking;

    private void OnEnable()
    {
        StartCoroutine(Blink());
    }

    private void OnDisable()
    {
        StopCoroutine(Blink());
    }

    private IEnumerator Blink()
    {
        _isWorking = true;

        while (_isWorking)
        {
            if (_material == null)
            {
                _material = GetComponent<Enemy>().Renderer.material;
                _color = _material.color;
                yield return new WaitForSeconds(1);
            }
            else
            {
                _material.color = color * 2;
                yield return new WaitForSeconds(blinkDuration);
                _material.color = _color * 1;
                yield return new WaitForSeconds(blinkDuration);
            }
        }
    }

    public void Switch()
    {
        _isWorking = !_isWorking;
    }
}
