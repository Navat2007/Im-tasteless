using System;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [SerializeField] private float speed = 40;
    
    private void Update()
    {
        transform.Rotate(Vector3.forward * speed * Time.deltaTime);
    }
}
