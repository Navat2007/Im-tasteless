using UnityEngine;


public class EnemyPowerSkin : MonoBehaviour
{
    [SerializeField] private float duration = 1;
    
    private Material _material;
    private Color _color;
    private int _emissionColor = Shader.PropertyToID("_EmissionColor");

    private void Awake()
    {
        _material = GetComponent<MeshRenderer>().material;
    }

    private void Start()
    {
        _material.EnableKeyword("_EMISSION");
        _material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        _material.SetColor(_emissionColor, _color);
    }

    private void Update () {
       
        float intensity = Mathf.PingPong (Time.time, duration) / duration;
        _material.EnableKeyword("_EMISSION");
        _material.SetColor(_emissionColor, Color.yellow * intensity);
    }

    public void SetColor(Color color)
    {
        _color = color;
    }
}
