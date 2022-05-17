using System;
using System.Collections;
using System.Collections.Generic;
using Interface;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10;
    [SerializeField] private float damage = 1;
    [SerializeField] private LayerMask collisionMask;

    private float _timeToDestroy = 3;
    private float _skinWidth = .1f;

    private void Start()
    {
        Destroy(gameObject, _timeToDestroy);

        /*
        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, .1f, collisionMask);
        if (initialCollisions.Length > 0)
        {
            OnHitObject(initialCollisions[0], transform.position);
        }
        */
    }

    private void Update()
    {
        float moveDistance = speed * Time.deltaTime;

        //CheckCollisions(moveDistance);
        
        transform.Translate(Vector3.forward * moveDistance);
    }

    private void CheckCollisions(float moveDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, moveDistance + _skinWidth, collisionMask, QueryTriggerInteraction.Collide))
        {
            //Debug.Log($"Hit {hit.collider.gameObject.name}");
            OnHitObject(hit.collider, hit.point);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var enemy = other.gameObject;
        
        if (enemy.GetComponent<IDamageable>() != null && enemy.TryGetComponent(out HealthSystem healthSystem))
        {
            healthSystem.TakeHit(damage, Helper.GetCriticalChance(10), other.ClosestPoint(transform.position), transform.forward);
        }
        
        Destroy(gameObject);
    }

    private void OnHitObject(Collider col, Vector3 hitPoint)
    {
        var enemy = col.gameObject;
        
        if (enemy.GetComponent<IDamageable>() != null && enemy.TryGetComponent(out HealthSystem healthSystem))
        {
            healthSystem.TakeHit(damage, Helper.GetCriticalChance(10), hitPoint, transform.forward);
        }
        
        Destroy(gameObject);
    }

    public void SetSpeed(float value)
    {
        speed = value;
    }
    
    public void SetDamage(float value)
    {
        damage = value;
    }
}
