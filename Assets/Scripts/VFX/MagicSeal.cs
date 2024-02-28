using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MagicSeal : MonoBehaviour
{
    [Header("Pulsing")]
    [SerializeField]
    private Color valleyColor;

    [SerializeField]
    private Color peakColor;

    [SerializeField]
    private float pusleSpeed;

    [Header("Completing")]
    [SerializeField]
    private Transform mask;

    [SerializeField]
    private float fullyDown = -14.5f;

    [SerializeField]
    private float fullyUp = -2f;

    private SpriteRenderer rend;

    private bool lit = false;

    private void Start()
    {
        rend = GetComponent<SpriteRenderer>();  
    }

    private void Update()
    {
        if (lit)
        {
            return;
        }

        rend.color = Color.Lerp(valleyColor, peakColor, Mathf.Cos(Time.time * pusleSpeed));
    }

    public async Task LightItUp()
    {
        float t = 0;

        Vector3 startPos = mask.localPosition;
        startPos.y = fullyDown;

        Vector3 targetPos = startPos;
        targetPos.y = fullyUp;

        while (t <= 1.0f)
        {
            t += Time.deltaTime;

            mask.transform.localPosition = Vector3.Lerp(startPos, targetPos, t);

            await Task.Yield();
        }
    }

    public async Task LightItDown()
    {
        float t = 0;

        Vector3 startPos = mask.localPosition;
        startPos.y = fullyUp;

        Vector3 targetPos = startPos;
        targetPos.y = fullyDown;

        while (t <= 1.0f)
        {
            t += Time.deltaTime;

            mask.transform.localPosition = Vector3.Lerp(startPos, targetPos, t);

            await Task.Yield();
        }
    }
}
