using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LightReciever : MonoBehaviour
{
    [Header("Light Hit")]
	public UnityEvent OnLightReceived;

	public GameObject LatestGameObject;

    [Header("Screen Shake")]
    [SerializeField]
    private bool shouldShake = true;

    [SerializeField]
    private float magnitude = 0.5f;

    [SerializeField]
    private float frequency = 1f;

    [SerializeField]
    private float duration = 0.2f;

    [Header("Stupid")]
    [SerializeField]
    private bool requireColor = false;

    [SerializeField]
    private Gradient requiredColor;

    public bool RequireColor => requireColor;
    public Gradient RequiredColor => requiredColor;

    private CinemachineBrain brain;

    private void Start()
    {
        brain = FindObjectOfType<CinemachineBrain>();

        OnLightReceived.AddListener(Shake);
    }

    private void OnDisable()
    {
        OnLightReceived.RemoveListener(Shake);
    }

    public void LightHit(GameObject gameObject)
	{
		LatestGameObject = gameObject;
		OnLightReceived?.Invoke();
	}

    public void Shake()
    {
        if (!shouldShake) return;

        ScreenShake.Shake(magnitude, frequency, duration, brain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>());
    }
}
