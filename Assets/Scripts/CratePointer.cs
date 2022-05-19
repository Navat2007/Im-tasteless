using UnityEngine;

public class CratePointer : MonoBehaviour
{
    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.current;
    }

    private void Update()
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

        Vector3 worldPosition = ray.GetPoint(minDistance);
    }
}
