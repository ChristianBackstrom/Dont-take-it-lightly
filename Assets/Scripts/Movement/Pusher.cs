using System;
using System.Threading.Tasks;
using UnityEngine;

public class Pusher : MonoBehaviour, IDataObject
{
	[SerializeField]
	private LayerMask collisionLayerMask;

	[SerializeField]
	private float pushSpeed = 1f;

	[SerializeField, Range(0, 1)]
	private float stopPushingThreshold = .5f;

    [Header("Audio")]
    [SerializeField]
    private SimpleAudioEvent pushSound;

    private GameObject pushedObject;

	public bool Pushing => pushing;
	public bool IsMoving => isMoving;
	public Vector3 MovingDirection => movingDirection;
	public Vector3 Direction => direction;

	private Vector3 movingDirection;

	private bool pushing;
	private bool isMoving;
	private Vector3 direction;

	private Movement movement;
	private InputHandler inputHandler;
	private Interactor interactor;
	private float timer = 0;

	private bool stopPushingQueued = false;

	private void Awake()
	{
		movement = GetComponent<Movement>();
		inputHandler = GetComponent<InputHandler>();
		interactor = GetComponent<Interactor>();
	}

	public bool Push(GameObject gameObject)
	{
		Vector3 dir = transform.position - gameObject.transform.position;

		float horizontalDotProduct = Vector3.Dot(dir.normalized, Vector3.right);
		float verticalDotProduct = Vector3.Dot(dir.normalized, Vector3.forward);

		if (Mathf.Abs(horizontalDotProduct) >= Mathf.Abs(verticalDotProduct))
		{
			direction = Vector3.right * Mathf.Sign(horizontalDotProduct);
		}
		else
		{
			direction = Vector3.forward * Mathf.Sign(verticalDotProduct);
		}

		interactor.enabled = false;

		pushedObject = gameObject;

		timer = 0;

		movement.Lock();
		pushing = true;
		isMoving = false;

		Vector3 targetObject = pushedObject.transform.position;
		targetObject.y = transform.position.y;

		transform.position = targetObject + direction;

		transform.LookAt(targetObject, Vector3.up);

		SetColliders();
		return true;
	}

	private void Update()
	{
		if (pushing)
		{
			HandleMovement();
		}
	}

	private void LateUpdate()
	{
		if (pushing)
		{
			timer += Time.deltaTime;
			if (timer > 0.2f)
			{
				if (!isMoving && inputHandler.Interact.WasPressedThisFrame())
				{
					StopPushing();
					return;
				}
			}
		}
	}

	private void HandleMovement()
	{
		if (isMoving)
		{
			return;
		}

		Vector2 move = inputHandler.Movement.ReadValue<Vector2>();
		move.Normalize();

		if (move == Vector2.zero) return;


		Vector3 moveWorld = new Vector3(Mathf.Round(move.x) * Mathf.Abs(direction.x), 0, Mathf.Round(move.y) * Mathf.Abs(direction.z));
		movingDirection = moveWorld;

		if (CheckDirection(moveWorld) && moveWorld != Vector3.zero)
		{
			Move(moveWorld);
		}
	}

	private async void Move(Vector3 dir)
	{
		isMoving = true;

		AudioManager.Instance.PlaySoundEffect(pushSound);

		float t = 0;

		Vector3 startPosition = pushedObject.transform.position;
		Vector3 targetPosition = startPosition + dir.normalized;
		Vector3 playerStartPosition = transform.position;
		Vector3 playerTargetPosition = playerStartPosition + dir.normalized;


		while (t <= 1.0f)
		{
			t += Time.deltaTime * pushSpeed;

			if (t > stopPushingThreshold && inputHandler.Interact.WasPressedThisFrame())
			{
				stopPushingQueued = true;
			}

			pushedObject.transform.position = Vector3.Lerp(startPosition, targetPosition, EasingFunctions.EaseInOut(t, 2));
			transform.position = Vector3.Lerp(playerStartPosition, playerTargetPosition, EasingFunctions.EaseInOut(t, 2));


			await Task.Yield();
		}

		pushedObject.transform.position = targetPosition;
		transform.position = playerTargetPosition;
		isMoving = false;

		if (stopPushingQueued) StopPushing();
	}

	private bool CheckDirection(Vector3 dir)
	{
		Debug.DrawRay(pushedObject.transform.position, dir, Color.red);


		bool playerCollision = !Physics.Raycast(transform.position, dir, 1f, collisionLayerMask);

		return !Physics.Raycast(pushedObject.transform.position, dir, 1f, collisionLayerMask) && playerCollision;
	}

	public async void StopPushing()
	{
		if (pushedObject == null)
		{
			return;
		}

		SetColliders();
		pushing = false;
		pushedObject = null;

		stopPushingQueued = false;

		movement.Unlock();

		await Task.Yield();
		interactor.enabled = true;
	}

	private void SetColliders()
	{
		Collider pushed = pushedObject.GetComponentInChildren<Collider>();
		Collider thisObject = this.GetComponent<Collider>();
		pushed.enabled = !pushed.enabled;
		thisObject.enabled = !thisObject.enabled;
	}


	public void Save(SaveData data)
	{

	}

	public void Load(SaveData data, bool reset = false)
	{
		StopPushing();
	}

}
