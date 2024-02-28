using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogueSoundController : MonoBehaviour
{
	[SerializeField, Range(0, 1)] private float pitchRange;
	[SerializeField] private AudioClip samVoice;
	[SerializeField] private AudioClip edgarVoice;

	private AudioSource audioSource;

	private const int letterConstant = 'a';


	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
	}

	public void PlayLetter(char character, char letter)
	{
		if (character == 'E')
		{
			audioSource.clip = edgarVoice;
		}
		else if (character == 'S')
		{
			audioSource.clip = samVoice;
		}
		else
		{
			audioSource.clip = null;
		}

		audioSource.pitch = GetPitch(letter);



		audioSource.Play();
	}

	private float GetPitch(char letter)
	{
		float pitch = 1;

		float pitchChange = ((letter - letterConstant) / 26f) * pitchRange;

		pitchChange -= pitchRange / 2f;

		return pitch + pitchChange;
	}
}
