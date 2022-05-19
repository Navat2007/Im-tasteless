using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CratePointer : MonoBehaviour
{
    [SerializeField] private Transform worldPointer;
    [SerializeField] private Image image;
    [SerializeField] private float fadeTimer = 10;
    
    private Camera _camera;
    private bool _isShow = true;

    private void Awake()
    {
        _camera = Camera.main;

        StartCoroutine(Fade(new Color(1, 1, 1, 1), new Color(1, 1, 1, 0), fadeTimer));
    }

    private void Update()
    {
        if (ControllerManager.player != null && _isShow)
        {
            Vector3 fromPlayerToEnemy = transform.position - ControllerManager.player.transform.position;
            Ray ray = new Ray(ControllerManager.player.transform.position, fromPlayerToEnemy);
            //Debug.DrawRay(ControllerManager.player.transform.position, fromPlayerToEnemy);

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

            minDistance = Mathf.Clamp(minDistance, 0, fromPlayerToEnemy.magnitude);

            Vector3 worldPosition = ray.GetPoint(minDistance);
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
            yield return null;
        }
        
        Destroy(this);
    }
}
