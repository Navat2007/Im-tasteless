using System.Collections;
using UnityEngine;

public class EnemyPowerSkin : MonoBehaviour
{
    [SerializeField] private float blinkDuration = 0.4f;

    private ZombieType _zombieType;
    private Renderer _renderer;
    private Material _material;
    private Color _color;
    private int _emissionColor = Shader.PropertyToID("_EmissionColor");
    private bool _isWorking;

    private void Start()
    {
        StartCoroutine(Blink());
    }

    private IEnumerator Blink()
    {
        _isWorking = true;

        while (_isWorking)
        {
            switch (_zombieType)
            {
                case ZombieType.FAST:
                    _material.color = Color.blue * 2;
                    break;
                default:
                    _material.color = Color.yellow * 2;
                    break;
            }
            yield return new WaitForSeconds(blinkDuration);
            _material.color = _color * 1;
            yield return new WaitForSeconds(blinkDuration);
        }
    }

    public EnemyPowerSkin SetColor(Color color)
    {
        _color = color;
        return this;
    }
    
    public EnemyPowerSkin SetZombieType(ZombieType zombieType)
    {
        _zombieType = zombieType;
        return this;
    }

    public EnemyPowerSkin SetRenderer(Renderer newRenderer)
    {
        _renderer = newRenderer;
        _material = _renderer.material;
        return this;
    }

    public void Switch()
    {
        _isWorking = !_isWorking;
    }
}
