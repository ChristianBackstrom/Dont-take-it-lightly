using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class KeepInView : MonoBehaviour
{
    [SerializeField] CinemachineTargetGroup targetGroup;
    [SerializeField] float delayTime = 2f;
    float timer;

    void Update()
    {   
        if (timer < delayTime)
        {
            timer += Time.deltaTime;
            return;
        }
        foreach (CinemachineTargetGroup.Target target in targetGroup.m_Targets)
        {
            target.target.position = AdjustPosition(target.target);        //confusing bc cinemachine calls the transform in the target struct "target" :/
        }
    }

    void OnEnable()
    {
        timer = 0;
    }


    Vector3 AdjustPosition(Transform objectTransform)
    {
        Vector3 objectViewportPos = Camera.main.WorldToViewportPoint(objectTransform.position);

        if (objectViewportPos.x < 0 || objectViewportPos.y > 1 || objectViewportPos.y < 0 || objectViewportPos.x > 1)
        {
            return Vector3.MoveTowards(objectTransform.position, targetGroup.Sphere.position, 10f * Time.deltaTime);
        }

        return objectTransform.position;
    }
}
