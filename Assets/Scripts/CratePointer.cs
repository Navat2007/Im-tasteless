using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CratePointer : MonoBehaviour
{
    [SerializeField] private Transform worldPointer;
    [SerializeField] private Image image;
    [SerializeField] private Image backImage;
    [SerializeField] private float fadeTimer = 10;
    
    private Camera _camera;
    public bool IsShow { get; private set; }
    public bool IsFade { get; private set; }

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void OnEnable()
    {
        IsFade = false;
        IsShow = true;
        
        StartCoroutine(Fade(new Color(1, 1, 1, 1), new Color(1, 1, 1, 0), fadeTimer));
    }

    private void Update()
    {
        if (ControllerManager.player != null && IsShow)
        {
            Vector3 fromPlayerToCrate = transform.position - ControllerManager.player.transform.position;
            Ray ray = new Ray(ControllerManager.player.transform.position, fromPlayerToCrate);

            // Ordering: [0] = Left, [1] = Right, [2] = Down, [3] = Up, [4] = Near, [5] = Far
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(_camera);

            float minDistance = Mathf.Infinity;
        
            for (int i = 0; i < 4; i++)
            {
                if (planes[i].Raycast(ray, out float distance))
                {
                    if (distance < minDistance)
                        minDistance = distance;
                }
            }

            minDistance = Mathf.Clamp(minDistance, 0, fromPlayerToCrate.magnitude);

            Vector3 worldPosition = ray.GetPoint(minDistance - 1);
            worldPointer.position = _camera.WorldToScreenPoint(worldPosition);
        }
    }
    
    private IEnumerator Fade(Color from, Color to, float time)
    {
        float speed = 1 / time;
        float percent = 1;

        while (percent > 0)
        {
            percent -= Time.deltaTime * speed;
            image.color = Color.Lerp(to, from, percent);
            backImage.color = Color.Lerp(to, from, percent);
            yield return null;
        }

        IsShow = false;
        IsFade = true;
    }

    public void SetInvisible()
    {
        IsShow = false;
        image.color = new Color(1, 1, 1, 0);
        backImage.color = new Color(1, 1, 1, 0);
    }
    
    public void SetVisible()
    {
        IsShow = true;
        image.color = new Color(1, 1, 1, 0.4f);
        backImage.color = new Color(1, 1, 1, 0.4f);
    }
}