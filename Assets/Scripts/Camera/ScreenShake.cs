using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class ScreenShake : MonoBehaviour
{
	[ContextMenu("Shake")]
	private void startShake()
	{
		Shake(1000f, .01f, 10, FindObjectOfType<CinemachineVirtualCamera>());
	}

	public async static void Shake(float magnitude, float frequency, float duration, CinemachineVirtualCamera vcam)
	{
		vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = magnitude;
		vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = frequency;

		await Task.Delay(Mathf.RoundToInt(duration * 1000));

		vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
		vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 0;
	}
}
