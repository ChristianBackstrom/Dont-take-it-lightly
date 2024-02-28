using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostAnimations : MonoBehaviour
{
	[SerializeField]
	private MovementData movementData;
	private Animator animator;

	private Vector3 lastPosition;

	private void Awake()
	{
		animator = GetComponentInChildren<Animator>();
	}

	private void Update()
	{
		animator.SetFloat("Speed", Vector3.Distance(lastPosition, transform.position) * Time.deltaTime * 10 * movementData.Speed);

		lastPosition = transform.position;
	}
}
