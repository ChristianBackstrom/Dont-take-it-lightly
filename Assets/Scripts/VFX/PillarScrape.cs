using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PillarScrape : MonoBehaviour
{
    [SerializeField]
    private float lifeTime = 1;

    private LineRenderer line;

    private MaterialPropertyBlock block;

    private void OnEnable()
    {
        line = GetComponent<LineRenderer>();

        block = new MaterialPropertyBlock();
        line.GetPropertyBlock(block);
        StartCoroutine(StartTheThing());
    }

    private IEnumerator StartTheThing()
    {
        float t = 0;

        while (t <= 1.0f)
        {
            t += Time.deltaTime / lifeTime;

            block.SetFloat("_Transparency", 1.0f - t);
            line.SetPropertyBlock(block);

            yield return null;
        }
    }
}
