using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("Other Portal")]
    [SerializeField]
    private Portal otherPortal;

    [Header("Visual")]
    [SerializeField]
    private Transform middle;

    [SerializeField]
    private Transform notMiddle;

    [SerializeField]
    private float outerRotSpeed = 1;

    [SerializeField]
    private float middleRotSpeed = 1;

    public Vector3 Position => middle.position;
    public Vector3 Direction => transform.right;

    private void Update()
    {
        middle.rotation = Quaternion.AngleAxis(Time.deltaTime * -middleRotSpeed, transform.right) * middle.rotation;
        notMiddle.rotation = Quaternion.AngleAxis(Time.deltaTime * outerRotSpeed, transform.right) * notMiddle.rotation;
    }

    public Vector3 GetOtherDirection(out Vector3 pos)
    {
        pos = otherPortal.Position;
        return otherPortal.Direction;
    }
}
