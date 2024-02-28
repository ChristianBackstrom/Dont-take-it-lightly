using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SnekLight : MonoBehaviour
{
    private static readonly int shPropColor = Shader.PropertyToID("_EmissionColor");

    [SerializeField]
    private Renderer rend;

    [SerializeField]
    private float fadeSpeed;

    private MaterialPropertyBlock block;

    private void Start()
    {
        block = new MaterialPropertyBlock();
    }

    [ContextMenu("Do the thing")]
    public async void SnekHit()
    {
        print("Doing the thing");

        float t = 0;

        Color start = Color.green * 2.5f;
        Color target = Color.black;

        block = new MaterialPropertyBlock();

        rend.GetPropertyBlock(block, 0);
        block.SetColor(shPropColor, start);

        rend.SetPropertyBlock(block, 0);

        while (t <= 1.0f)
        {
            t += Time.deltaTime * fadeSpeed;

            rend.GetPropertyBlock(block, 0);
            block.SetColor(shPropColor, Color.Lerp(start, target, EasingFunctions.EaseIn(t, 3)));
            rend.SetPropertyBlock(block, 0);

            print(block.GetColor(shPropColor));

            await Task.Yield();
        }
    }
}
