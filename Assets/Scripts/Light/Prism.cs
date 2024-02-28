using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prism : MonoBehaviour
{
    [Header("Directions")]
    [SerializeField]
    private Transform[] lightThings;

    [Header("Color")]
    [SerializeField]
    private bool changeColor = false;

    [SerializeField]
    private Gradient[] lightColors;

    [Header("Shape")]
    [SerializeField]
    private bool changeShape = false;

    [SerializeField]
    private Material[] lightMaterials;

    public List<Transform> LightRefractions(Vector3 dir, out List<Gradient> lightColors, out List<Material> lightMaterials)
    {
        float value = 0f;
        int index = 0;

        for (int i = 0; i < lightThings.Length; i++)
        {
            float val = Vector3.Dot(dir, -lightThings[i].forward);
            if (val > value)
            {
                value = val;
                index = i;
            }
        }

        lightColors = new List<Gradient>();
        lightMaterials = new List<Material>();
        List<Transform> sides = new List<Transform>();
        for (int i = 0; i < lightThings.Length; i++)
        {
            if (i == index)
            {
                continue;
            }

            sides.Add(lightThings[i]);

            if (changeColor)
            {
                lightColors.Add(GetColor(i));
            }

            if (changeShape)
            {
                lightMaterials.Add(GetMat(i));
            }
        }

        return sides;
    }

    private Gradient GetColor(int i)
    {
        if (i >= lightColors.Length)
        {
            return GetColor(i - 1);
        }

        return lightColors[i];
    }

    private Material GetMat(int i)
    {
        if (i >= lightMaterials.Length)
        {
            return GetMat(i - 1);
        }

        return lightMaterials[i];
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        for (int i = 0; i < lightThings.Length; i++)
        {
            Gizmos.DrawRay(lightThings[i].position, lightThings[i].forward);
        }
    }
}
