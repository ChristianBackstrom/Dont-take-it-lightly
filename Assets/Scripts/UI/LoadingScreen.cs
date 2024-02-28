using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] Transform circleMask;
    [SerializeField] TextMeshProUGUI loadingText;

    private Vector3 startScale;

    private void Awake()
    {
        startScale = circleMask.localScale;
    }

    public async Task FadeOut()
    {
        float speed = 1f;
        float t = 0;

        Vector3 targetScale = Vector3.zero;

        while (t <= 1.0f && circleMask != null)
        {
            t += Time.deltaTime * speed;

            circleMask.localScale = Vector3.Lerp(startScale, targetScale, EasingFunctions.EaseOut(t));

            await Task.Yield();
        }
    }

    public async Task FadeIn()
    {
        float speed = 1f;
        float t = 0;

        Vector3 startScale = Vector3.zero;
        Vector3 targetScale = this.startScale;

        while (t <= 1.0f && circleMask != null)
        {
            t += Time.deltaTime * speed;

            circleMask.localScale = Vector3.Lerp(startScale, targetScale, EasingFunctions.EaseOut(t));

            await Task.Yield();
        }
    }

    public async void UpdateLoadingScreen(string text)
    {
        loadingText.SetText(text);
        while (loadingText != null)
        {
            loadingText.color = GetRandomColor();
            await Task.Yield();
        }   
    }


    private Color GetRandomColor(){
        Color color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1);
        return color;
    }
}
