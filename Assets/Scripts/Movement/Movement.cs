using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
	[SerializeField] 
	private MovementData data;

	private CharacterController characterController;
	private ParticleSystem dustParticles;
	private InputHandler inputHandler;
	private AudioSource source;

	private Vector3 moveInput;
	private Vector3 lastPosition;
	private Vector3 newLastPosition;

	private bool shouldBeOff = false;
	private bool locked = false;
	private bool resetFrame = false;
	private float originalY;

	private void Awake()
	{
		characterController = GetComponent<CharacterController>();
		dustParticles = GetComponentInChildren<ParticleSystem>();
        source = GetComponentInChildren<AudioSource>();
		inputHandler = GetComponent<InputHandler>();

		originalY = transform.position.y;
	}

	private void Update()
	{
		if (source != null && shouldBeOff && source.isPlaying) source.Pause();
		shouldBeOff = true;

		transform.position = new Vector3(transform.position.x, originalY, transform.position.z);
		if (resetFrame)
		{
			resetFrame = false;
			characterController.enabled = true;
			return;
		}

		if (inputHandler.Controls == null || locked) return;

		GetInputs();
		SetParticles();

		lastPosition = transform.position;

		if (moveInput == Vector3.zero) return;

		Quaternion targetRotation = Quaternion.LookRotation(moveInput);
		transform.position = new Vector3(transform.position.x, originalY, transform.position.z);

		this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, Time.deltaTime * data.RotationSmoothingSpeed);
		characterController.SimpleMove(moveInput * data.Speed);

		if (source != null && !source.isPlaying) source.Play();
		shouldBeOff = false;
    }

    private void SetParticles()
	{
		if (dustParticles == null) return;


		if (lastPosition != transform.position && !dustParticles.isEmitting)
		{
			dustParticles.Play();
			return;
		}

		if (lastPosition == transform.position && dustParticles.isEmitting)
		{
			dustParticles.Stop();
		}
	}

	private void GetInputs()
	{
		Vector2 input = inputHandler.Movement.ReadValue<Vector2>();

		moveInput = new Vector3(input.x, 0, input.y);

	}

	public void Lock()
	{
		locked = true;
	}

	public void Unlock()
	{
		locked = false;
	}

	public void Reset()
	{
		if (characterController == null) return;
		characterController.enabled = false;
		resetFrame = true;
	}
}