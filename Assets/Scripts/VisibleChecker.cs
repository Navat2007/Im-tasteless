using System.Collections;
using System.Collections.Generic;
using Helpers;
using Interface;
using UnityEngine;

public class VisibleChecker : MonoBehaviour
{
    [SerializeField] private List<Fadeable> currentObjects = new ();
    [SerializeField] private List<Fadeable> alreadyTransparentObjects = new ();
    [SerializeField] private LayerMask layerMask;
    
    private Transform _player;
    private Transform _camera;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _camera = Camera.main.transform;

        StartCoroutine(CheckObjectsBetweenCamera());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (layerMask.Includes(other.gameObject.layer) && other.gameObject.TryGetComponent(out MeshRenderer meshRenderer))
        {
            //meshRenderer.enabled = false;
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (layerMask.Includes(other.gameObject.layer) && other.gameObject.TryGetComponent(out MeshRenderer meshRenderer))
        {
            //meshRenderer.enabled = true;
        }
    }
    
    private IEnumerator CheckObjectsBetweenCamera()
    {
        void MakeObjectsTransparent()
        {
            foreach (var item in currentObjects)
            {
                if (!alreadyTransparentObjects.Contains(item))
                {
                    item.ShowTransparent();
                    alreadyTransparentObjects.Add(item);
                }
            }
        }
        
        void MakeObjectsSolid()
        {
            for (int i = alreadyTransparentObjects.Count - 1; i >= 0 ; i--)
            {
                if (!currentObjects.Contains(alreadyTransparentObjects[i]))
                {
                    alreadyTransparentObjects[i].ShowSolid();
                    alreadyTransparentObjects.Remove(alreadyTransparentObjects[i]);
                }
            }
        }
        
        while (true)
        {
            currentObjects.Clear();
            
            float distance = Vector3.Magnitude(_camera.position - _player.position);
            Ray rayForward = new Ray(_camera.position, _player.position - _camera.position);
            Ray rayBackward = new Ray(_player.position, _camera.position - _player.position);
            
            RaycastHit[] hitsForward = Physics.RaycastAll(rayForward, distance);
            RaycastHit[] hitsBackward = Physics.RaycastAll(rayBackward, distance);
            
            foreach (RaycastHit hit in hitsForward)
            {
                GameObject hitGameObject = hit.collider.gameObject;
                
                if (hitGameObject.transform.parent != null && hitGameObject.transform.parent.gameObject.TryGetComponent(out Fadeable fadeable))
                {
                    if(!currentObjects.Contains(fadeable)) 
                        currentObjects.Add(fadeable);
                }
            }
            
            foreach (RaycastHit hit in hitsBackward)
            {
                GameObject hitGameObject = hit.collider.gameObject;
                
                if (hitGameObject.transform.parent != null && hitGameObject.transform.parent.gameObject.TryGetComponent(out Fadeable fadeable))
                {
                    if(!currentObjects.Contains(fadeable)) 
                        currentObjects.Add(fadeable);
                }
            }

            MakeObjectsSolid();
            MakeObjectsTransparent();

            yield return new WaitForSeconds(.3f);
        }
    }
}
