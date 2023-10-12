using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CratePointer : MonoBehaviour
{
    public bool IsShow { get; private set; }
    public bool IsFade { get; private set; }
    public bool IsActive { get; private set; }

    [SerializeField] private Transform worldPointer;
    [SerializeField] private Image image;
    [SerializeField] private Transform pointerImage;
    [SerializeField] private Image backImage;
    [SerializeField] private float fadeTimer = 10;
    [SerializeField] private float offScreenThreshold = 10;

    private Camera _camera;
    private float _nextUpdateTime;
    private bool _isUIOpen;
    private Image _prevImage;
    private Image _prevBackImage;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void OnEnable()
    {
        IsFade = false;
        IsShow = true;

        GameUI.instance.OnUIOpen += OnUIOpen;
        GameUI.instance.OnUIClose += OnUIClose;

        StartCoroutine(Fade(new Color(1, 1, 1, 1), new Color(1, 1, 1, 0), fadeTimer));
    }

    private void OnDisable()
    {
        GameUI.instance.OnUIOpen -= OnUIOpen;
        GameUI.instance.OnUIClose -= OnUIClose;
    }

    private void OnUIOpen()
    {
        _isUIOpen = true;
        _prevImage = image;
        _prevBackImage = backImage;

        image.color = new Color(1, 1, 1, 0);
        backImage.color = new Color(1, 1, 1, 0);
    }

    private void OnUIClose()
    {
        _isUIOpen = false;

        image.color = _prevImage.color;
        backImage.color = _prevBackImage.color;
    }

    private void Update()
    {
        SetPointerPosition();
    }

    private void SetPointerPosition()
    {
        if (ControllerManager.player != null && IsShow)
        {
            if (_isUIOpen) return;

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

    private void SetPointerPosition2()
    {
        if (ControllerManager.player != null && IsActive && _isUIOpen == false)
        {
            Vector3 targetDirection = transform.position - ControllerManager.player.transform.position;
            float distanceToTarget = targetDirection.magnitude;
            
            if (distanceToTarget < offScreenThreshold)
            {
                //gameObject.SetActive(false);
                //IsShow = false;
                Debug.Log($"Target distance < offScreenThreshold: {distanceToTarget}");
                SetInvisible();
            }
            else
            {
                Vector3 targetViewportPosition =
                    _camera.WorldToViewportPoint(transform.position);

                if (targetViewportPosition.x > 0 && targetViewportPosition.x < 1 && targetViewportPosition.y > 0 &&
                    targetViewportPosition.y < 1)
                {
                    //gameObject.SetActive(false);
                    Debug.Log($"Target in viewport: {targetViewportPosition.x}, {targetViewportPosition.y}");
                    SetInvisible();
                }
                else
                {
                    //gameObject.SetActive(true);
                    SetVisible();
                    
                    Vector3 screenEdge = _camera.ViewportToWorldPoint(
                        new Vector3(Mathf.Clamp(targetViewportPosition.x, .1f, .9f),
                            Mathf.Clamp(targetViewportPosition.y, .1f, .9f),
                            _camera.nearClipPlane
                        ));
                    
                    Debug.Log($"Target off screen edge: {screenEdge}");
                    
                    pointerImage.position = new Vector3(screenEdge.x, screenEdge.y, 0);
                    //image.transform.position = new Vector3(screenEdge.x, screenEdge.y, 0);
                }
            }
        }
    }

    private IEnumerator Fade(Color from, Color to, float time)
    {
        float speed = 1 / time;
        float percent = 1;

        while (percent > 0)
        {
            if (_isUIOpen)
                yield return null;

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
    
    public void SetActive(bool value)
    {
        IsActive = value;
    }
}