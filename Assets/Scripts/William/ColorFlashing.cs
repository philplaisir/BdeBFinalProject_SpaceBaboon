using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// This script will flash a UI Image component with the given parameters. Useful for creating
/// quick, animated UI flashes.
/// Created by: Adam Chandler
/// Make sure that you attach this script to an Image component. You can optionally call the
/// flash remotely and pass in new flash values, or you can predefine settings in the Inspector
/// </summary>

[RequireComponent(typeof(SpriteRenderer))]
public class ColorFlashing : MonoBehaviour
{
    // events
    public event Action OnStop = delegate { };
    public event Action OnCycleStart = delegate { };
    public event Action OnCycleComplete = delegate { };

    Coroutine _flashRoutine = null;
    SpriteRenderer m_spriteRenderer;

    #region Initialization
    private void Awake()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        // initial state
        SetAlphaToDefault();
    }

    #endregion

    #region Public Functions

    public void Flash(float secondsForOneFlash, float minAlpha, float maxAlpha, Color color, float TotalDuration)
    {
        if (secondsForOneFlash <= 0) { return; }    // 0 speed wouldn'y make sense

        if (_flashRoutine != null)
            StopCoroutine(_flashRoutine);
        _flashRoutine = StartCoroutine(
            FlashRoutine(secondsForOneFlash, minAlpha, maxAlpha, color, TotalDuration)
            );
    }

    public void StopFlash()
    {
        if (_flashRoutine != null)
            StopCoroutine(_flashRoutine);

        SetAlphaToDefault();

        OnStop?.Invoke();
    }
    #endregion

    #region Private Functions
    IEnumerator FlashRoutine(float secondsForOneFlash, float minAlpha, float maxAlpha, Color color, float TotalDuration)
    {

        SetColor(color);
        // half the flash time should be on flash in, the other half for flash out
        float flashInDuration = secondsForOneFlash / 2;
        float flashOutDuration = secondsForOneFlash / 2;

        OnCycleStart?.Invoke();
        // flash in
        //Debug.Log("Start Flash");
        for (float t = 0; t < TotalDuration; t += Time.deltaTime) 
        {
            for (float y = 0f; y <= flashInDuration; y += Time.deltaTime)
            {
                Color newColor = m_spriteRenderer.color;
                newColor.a = Mathf.Lerp(minAlpha, maxAlpha, y / flashInDuration);
                m_spriteRenderer.color = newColor;
                yield return null;
            }
            // flash out
            for (float y = 0f; y <= flashOutDuration; y += Time.deltaTime)
            {
                Color newColor = m_spriteRenderer.color;
                newColor.a = Mathf.Lerp(maxAlpha, minAlpha, y / flashOutDuration);
                m_spriteRenderer.color = newColor;
                yield return null;
            }
            yield return null;
        }
        SetAlphaToDefault();
        OnCycleComplete?.Invoke();
    }

    private void SetColor(Color newColor)
    {
        m_spriteRenderer.color = newColor;
    }

    private void SetAlphaToDefault()
    {
        Color newColor = m_spriteRenderer.color;
        newColor.a = 1;
        m_spriteRenderer.color = newColor;
    }

    #endregion
}