using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class FinalRoomHandler : MonoBehaviour
{
    public UnityEvent OnWin;
    public UnityEvent OnTouchOrb;

    [SerializeField]
    private Transform[] thingsToSpin;

    [SerializeField]
    private GameObject winPanel;

    private List<FinalRoomPillar> reportedPillars = new List<FinalRoomPillar>();

    private bool waiting = false;
    private int amount = 5;
    private float timer = 0;
    private float timerMax = 10;

    private void Update()
    {
        if (waiting)
        {
            timer += Time.deltaTime;
            if (timer >= timerMax)
            {
                Reset();
            }
        }
    }

    public void ReportingIn(FinalRoomPillar pillar)
    {
        if (reportedPillars.Contains(pillar))
        {
            return;
        }

        reportedPillars.Add(pillar);
        waiting = true;
        timer = 0;

        if (reportedPillars.Count >= amount)
        {
            // WIN
            print("You win");
            Reset();

            OnWin?.Invoke();
            Win();
        }
    }

    private async void Win()
    {
        float t = 0;
        float speed = 0.2f;
        float spinSpeed = 30;

        Vector3[] startPos = new Vector3[thingsToSpin.Length];
        Vector3[] targetPos = new Vector3[thingsToSpin.Length];
        for (int i = 0; i < startPos.Length; i++)
        {
            startPos[i] = thingsToSpin[i].position;
            targetPos[i] = startPos[i] - Vector3.up * 3;
        }

        while (t <= 1.0f)
        {
            t += Time.deltaTime * speed;

            for (int i = 0; i < thingsToSpin.Length; i++)
            {
                thingsToSpin[i].rotation = Quaternion.AngleAxis(spinSpeed * Time.deltaTime, Vector3.up) * thingsToSpin[i].rotation;
                thingsToSpin[i].position = Vector3.LerpUnclamped(startPos[i], targetPos[i], EasingFunctions.EaseInOutElastic(t));
            }

            await Task.Yield();
        }

    }

    public void ActuallyWin()
    {
        Instantiate(winPanel, FindObjectOfType<Canvas>().transform);

        OnTouchOrb?.Invoke();
    }

    public void Reset()
    {
        timer = 0; 
        waiting = false;

        for (int i = 0; i < reportedPillars.Count; i++)
        {
            reportedPillars[i].UnLight();
        }

        reportedPillars.Clear();
    }
}
