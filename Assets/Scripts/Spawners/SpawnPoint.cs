using System.Collections;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public bool available;

    [SerializeField] private float scanTime = 1;
    [SerializeField] private float minDistance = 35;
    [SerializeField] private float maxDistance = 90;
    [SerializeField] private float distance;
    
    private Transform _player;

    private void Start()
    {
        _player = ControllerManager.player.transform;
        StartCoroutine(CheckDistance());
    }

    private IEnumerator CheckDistance()
    {
        while (true)
        {
            distance = Vector3.Distance(transform.position, _player.position);

            if (distance >= minDistance && distance <= maxDistance)
                available = true;
            else
                available = false;

            yield return new WaitForSeconds(scanTime);
        }
    }
}
