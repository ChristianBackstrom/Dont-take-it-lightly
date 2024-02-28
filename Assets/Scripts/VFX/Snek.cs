using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snek : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField]
    private float humanRetreatDistance = 5f;

    [SerializeField]
    private float ghostLookAtDistance = 5f;

    [SerializeField]
    private float maxStretchAmount = 1f;

    [SerializeField]
    private float snakeHeadSpeed = 1;

    [Header("Setup")]
    [SerializeField]
    private Transform target;

    [SerializeField]
    private Transform targetOrigin;

    [SerializeField]
    private Transform fullyOutPos;

    [SerializeField]
    private Transform fullyInPos;

    private Camera cam;
    private Transform human;
    private Transform ghost;

    private Vector3 targetTargetPosition;
    private Vector3 bodyTargetPosition;

    private bool isSnaking = false;

    private void Start()
    {
        FindObjectOfType<CameraManager>().OnRoomActivate[1].AddListener(Activate);

        human = FindObjectOfType<Interactor>().transform;
        ghost = FindObjectOfType<GhostInteractor>().transform;
        cam = Camera.main;

        //Activate();
    }

    private void Activate()
    {
        isSnaking = true;
    }

    private void Update()
    {
        if (!isSnaking)
        {
            return;
        }

        float humanDist = Vector3.Distance(targetOrigin.position, human.position);
        float ghostDist = Vector3.Distance(targetOrigin.position, ghost.position);

        if (humanDist < humanRetreatDistance)
        {
            RetreatFromHuman();
        }
        else if (ghostDist < ghostLookAtDistance)
        {
            LookAtGhost();
        }
        else
        {
            LookAtPlayer();
        }

        UpdateDistance(humanDist);

        target.position = Vector3.Lerp(target.position, targetTargetPosition, Time.deltaTime * snakeHeadSpeed);
        transform.position = Vector3.Lerp(transform.position, bodyTargetPosition, Time.deltaTime * snakeHeadSpeed * 2);
    }

    private void UpdateDistance(float dist)
    {
        float percent = (float)(dist - 3.0f) / (float)(humanRetreatDistance - 3);
        percent = Mathf.Clamp01(percent);
        bodyTargetPosition = Vector3.Lerp(fullyInPos.position, fullyOutPos.position, percent);
    }

    private void LookAtPlayer()
    {
        Vector3 dir = (cam.transform.position - targetOrigin.position).normalized;
        Vector3 pos = targetOrigin.position + dir * maxStretchAmount;

        targetTargetPosition = pos;
    }

    private void LookAtGhost()
    {
        Vector3 dir = (ghost.transform.position - targetOrigin.position).normalized;
        float stretch = Mathf.Min(maxStretchAmount, Vector3.Distance(targetOrigin.position, ghost.position) + 0.5f);
        Vector3 pos = targetOrigin.position + dir * stretch;

        targetTargetPosition = pos;
    }

    private void RetreatFromHuman()
    {
        targetTargetPosition = transform.parent.position;
    }
}
