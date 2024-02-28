using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanAnimations : MonoBehaviour
{
	[SerializeField]
	private MovementData movementData;

	private Animator animator;
	private Pusher pusher;

	private Vector3 lastPosition;

	private bool lastPushing;

	private void Awake()
	{
		animator = GetComponentInChildren<Animator>();
		pusher = GetComponent<Pusher>();
	}

	private void Update()
	{
		animator.SetFloat("Speed", Vector3.Distance(lastPosition, transform.position) * Time.deltaTime * 10_000);

		lastPosition = transform.position;

		animator.SetBool("Pushing", pusher.Pushing);
		animator.SetBool("IsMoving", pusher.IsMoving);

		if (lastPushing == false && pusher.Pushing == true)
		{
			animator.SetTrigger("StartPushing");
		}

		float pushingDirection = 0;
		if (pusher.IsMoving)
		{
			Vector3 movingDirection = pusher.MovingDirection;

			if (pusher.Direction == movingDirection)
			{
				pushingDirection = -1;
			}
			else if (-pusher.Direction == movingDirection)
			{
				pushingDirection = 1;
			}

		}
		animator.SetFloat("Direction", pushingDirection);

		lastPushing = pusher.Pushing;

	}
}
