using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenDamageEffect : MonoBehaviour
{
    [SerializeField] private Image overlayImage;
    [SerializeField] private float flashDuration = 0.2f;   // Ne kadar hýzlý yanýp sönecek
    [SerializeField] private float fadeSpeed = 2f;        // Ne kadar hýzlý sönecek
    [SerializeField] private Color flashColor = new Color(1f, 0f, 0f, 0.5f); // Yarý saydam kýrmýzý
    public static ScreenDamageEffect Instance { get; private set; }
    private Color _targetColor = Color.clear;

    void Awake()
    {
        Instance = this;
        if (overlayImage != null)
            overlayImage.color = Color.clear;
    }

    void Update()
    {
        if (overlayImage != null)
        {
            // Renk yumuþakça hedef renge doðru kayar
            overlayImage.color = Color.Lerp(overlayImage.color, _targetColor, fadeSpeed * Time.deltaTime);
        }
    }

    public void Flash()
    {
        if (overlayImage == null) return;

        // Önce kýsa süreli flash
        _targetColor = flashColor;
        CancelInvoke(nameof(ResetFlash));
        Invoke(nameof(ResetFlash), flashDuration);
    }

    private void ResetFlash()
    {
        _targetColor = Color.clear;
    }
}
