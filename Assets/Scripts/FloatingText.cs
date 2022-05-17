using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private Image armorImage;
    [SerializeField] private TMP_Text text;
    private float _destroyTime = 3;
    
    private static int _sortingOrder;
    private const float DisappearTimerMax = 1f;
    
    private float _disappearTimer;
    private Color _textColor;
    private Vector3 _moveVector;

    private void Awake()
    {
        if (text == null)
        {
            throw new NotImplementedException("Floating text, не назначен текст");
        }
        
        if (armorImage == null)
        {
            throw new NotImplementedException("Floating text, не назначено изображение брони");
        }
    }

    private void Start()
    {
        Destroy(gameObject, _destroyTime);
    }

    private void Update()
    {
        transform.position += _moveVector * Time.deltaTime;
        _moveVector -= _moveVector * (5f * Time.deltaTime);

        _disappearTimer -= Time.deltaTime;

        if (_disappearTimer > DisappearTimerMax * .8f)
        {
            float increaseScaleAmount = 4;
            transform.localScale += Vector3.one * (increaseScaleAmount * Time.deltaTime);
        }
        else
        {
            float decreaseScaleAmount = 2;
            if(transform.localScale.x > 0)
                transform.localScale -= Vector3.one * (decreaseScaleAmount * Time.deltaTime);
        }

        if (_disappearTimer < 0)
        {
            float disappear_speed = 3f;
            _textColor.a -= disappear_speed * Time.deltaTime;
            text.color = _textColor;
            
            if(_textColor.a < 0)
                Destroy(gameObject);
        }
    }
    
    public void Setup(String textValue, bool isCritical, bool isShield)
    {
        text.SetText(textValue);

        if (isCritical)
        {
            text.fontSize = 24;
            text.color = Color.red;
        }
            
        _textColor = text.color;
        _disappearTimer = DisappearTimerMax;
        
        _moveVector = new Vector3(UnityEngine.Random.Range(-.8f, .8f), 1) * 20f;
        
        if(isShield)
            armorImage.gameObject.SetActive(true);
    }
}
