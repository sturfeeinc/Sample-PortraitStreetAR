using System.Collections;
using UnityEngine;

public class ExpandingCircle : MonoBehaviour
{
    public float ScaleAmount = 0.25f;
    public float AlphaReducAmount = 0.015f;

    private Vector3 _origScale;
    private Color _origColor;

    private Vector3 _scale;
    private Color _color;

    private SpriteRenderer _spriteRenderer;

    private bool _reset = false;
    private bool _active = false;
    private bool _endAnimation = false;

    void Awake()
    {
        _scale = transform.localScale;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _color = _spriteRenderer.color;

        _origColor = _color;
        _origScale = _scale;
    }

    public void Activate(Vector3 pos, Quaternion rot)
    {
        transform.position = pos;
        transform.rotation = rot;

        if (!_active)
        {
            _active = true;
            Reset();
            StartCoroutine(Animate());
        }
        else
        {
            _reset = true;
        }
    }

    public void EndAnimation()
    {
        if (_active)
        {
            _endAnimation = true;
        }
        else
        {
            _endAnimation = false;
            _active = false;
            _reset = false;

            transform.parent.gameObject.SetActive(false);
        }
    }

    private IEnumerator Animate()
    {
        if (_endAnimation)
        {
            _endAnimation = false;
            _active = false;
            _reset = false;

            transform.parent.gameObject.SetActive(false);
        }
        else
        {
            yield return new WaitForFixedUpdate();

            if (_reset)
            {
                Reset();
                _reset = false;
            }

            _scale += Vector3.one * ScaleAmount;
            _color.a -= 0.015f;
            transform.localScale = _scale;
            _spriteRenderer.color = _color;

            if (_color.a > 0)
            {
                StartCoroutine(Animate());
            }
            else
            {
                _active = false;
            }
        }
    }

    private void Reset()
    {
        //      _reset = true;
        _scale = _origScale;
        _color = _origColor;
        _spriteRenderer.color = _origColor;
        transform.localScale = _origScale;
    }

}
