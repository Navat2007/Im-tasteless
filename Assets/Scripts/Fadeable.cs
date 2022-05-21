using System.Collections;
using Interface;
using UnityEngine;

public class Fadeable : MonoBehaviour, IFadeable
{
    [SerializeField] private GameObject solidObject;
    [SerializeField] private GameObject transparentObject;

    private void Awake()
    {
        ShowSolid();
    }

    public void ShowTransparent()
    {
        transparentObject.SetActive(true);
        solidObject.SetActive(false);
    }
    
    public void ShowSolid()
    {
        transparentObject.SetActive(false);
        solidObject.SetActive(true);
    }
    
    private IEnumerator FadeTo(bool fadeIn, float duration, MeshRenderer meshRenderer)
    {
        float counter = 0f;

        float a, b;
        if (fadeIn)
        {
            a = 0.2f;
            b = 1;
        }
        else
        {
            a = 1;
            b = 0.2f;
        }

        Color meshColor = meshRenderer.material.color;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.Lerp(a, b, counter / duration);

            meshRenderer.material.color = new Color(meshColor.r, meshColor.g, meshColor.b, alpha);
            yield return null;
        }
    }
}