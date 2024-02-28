using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GhostPossesion : MonoBehaviour, IDataObject
{
	[Header("Collision")]
	[SerializeField]
	private SkinnedMeshRenderer[] meshRenderer;

	[SerializeField]
	private LayerMask collisionLayerMask;

	[Header("Stats")]
	[SerializeField]
	private float scrapeSpeed = 1f;

	[SerializeField]
	private float rotationAngles = 15f;

	[Header("VFX")]
	[SerializeField]
	private PooledMonoBehaviour possessParticle;

	[SerializeField]
	private PooledMonoBehaviour scrapeLine;

	[Header("Audio")]
    [SerializeField]
    private SimpleAudioEvent possessionSound;

    [SerializeField]
    private SimpleAudioEvent possessLightSound;

    [SerializeField]
    private SimpleAudioEvent leavingLightSound;

	[SerializeField]
	private SimpleAudioEvent rotateSound;

	[SerializeField]
	private SimpleAudioEvent scrapeSound;

    private Movement movement;
	private Animator animator;
	private InputHandler inputHandler;
	private GhostInteractor interactor;
	private GameObject possesion;

	private Vector2 moveInput;
	private Vector3 possessDirection;

	private float timer = 0;
	private float whyLevel = 0;
	private bool possessing = false;
	private bool isMoving = false;
	private bool isRotating = false;
	private bool canMove = false;
	private bool canRotate = false;

	private void Start()
	{
		movement = GetComponent<Movement>();
		inputHandler = GetComponent<InputHandler>();
		interactor = GetComponent<GhostInteractor>();
		animator = GetComponentInChildren<Animator>();
	}

	private void Update()
	{
		if (!possessing)
		{
			return;
		}

		timer += Time.deltaTime;
		if (timer > 0.2f)
		{
			if (!isMoving && inputHandler.Interact.WasPressedThisFrame())
			{
				StopPossesion();
				return;
			}
		}

		moveInput = inputHandler.Movement.ReadValue<Vector2>();

		HandleMovement();
		HandleRotation();
	}

	private void HandleRotation()
	{
		if (!canRotate || isRotating)
		{
			return;
		}

		if (Mathf.Abs(moveInput.x) > 0)
		{
			RotateSmoot();
		}
	}

	private async void RotateSmoot()
	{
		isRotating = true;

		AudioManager.Instance.PlaySoundEffect(rotateSound);

		float t = 0;
		Quaternion startRotation = possesion.transform.rotation;
		Quaternion targetRotation = Quaternion.AngleAxis(Mathf.Sign(moveInput.x) * rotationAngles, Vector3.up) * possesion.transform.rotation;

		while (t <= 1.0f)
		{
			t += Time.deltaTime * 2.5f;

			possesion.transform.rotation = Quaternion.SlerpUnclamped(startRotation, targetRotation, EasingFunctions.EaseOut(t, 4));

			await Task.Yield();
		}

		possesion.transform.rotation = targetRotation;
		isRotating = false;
	}

	private void HandleMovement()
	{
		if (isMoving || !canMove)
		{
			return;
		}

		if (moveInput.x > 0.5f && CheckDirection(Vector3.right))
		{
			Move(Vector3.right);
		}
		else if (moveInput.y > 0.5f && CheckDirection(Vector3.forward))
		{
			Move(Vector3.forward);
		}
		else if (moveInput.x < -0.5f && CheckDirection(Vector3.left))
		{
			Move(Vector3.left);
		}
		else if (moveInput.y < -0.5f && CheckDirection(Vector3.back))
		{
			Move(Vector3.back);
		}
	}

	private async void Move(Vector3 dir)
	{
		isMoving = true;

		AudioManager.Instance.PlaySoundEffect(scrapeSound);

		float t = 0;

		Vector3 startPosition = possesion.transform.position;
		Vector3 targetPosition = startPosition + dir;

		bool scraped = false;
		while (t <= 1.0f)
		{
			t += Time.deltaTime * scrapeSpeed;

			possesion.transform.position = Vector3.LerpUnclamped(startPosition, targetPosition, EasingFunctions.EaseInOutBack(t));

			if (t > 0.5f && !scraped)
			{
				scraped = true;
				Scrape(dir);
			}

			await Task.Yield();
		}

		possesion.transform.position = targetPosition;
		isMoving = false;
	}

	private void Scrape(Vector3 dir)
	{
		int amount = UnityEngine.Random.Range(2, 8);

		for (int i = 0; i < amount; i++)
		{
			Vector3 pos = possesion.transform.position + new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), 0, UnityEngine.Random.Range(-0.5f, 0.5f)) - dir * 0.5f;
			float length = UnityEngine.Random.Range(0.4f, 1);

			LineRenderer line = scrapeLine.GetAtPosAndRot<PooledMonoBehaviour>(Vector3.zero, Quaternion.identity).GetComponent<LineRenderer>();
			line.SetPosition(0, pos);
			line.SetPosition(1, pos + dir * length);
		}
	}

	public async void Possess(GameObject gameObject, bool canMove, bool canRotate)
	{
		PossesionShaderManager shaderManager = gameObject.GetComponentInChildren<PossesionShaderManager>();
		StartCoroutine(shaderManager.Activate());
		HandleAudio(gameObject);

		HandleDisabling(gameObject);

		this.canMove = canMove;
		this.canRotate = canRotate;
		whyLevel = transform.position.y;

		movement.Lock();
		timer = 0;
		possessing = true;
		isMoving = true;
		isRotating = true;

        possessDirection = (transform.position - gameObject.transform.position).normalized;
		transform.rotation = Quaternion.LookRotation(-possessDirection, Vector3.up);

		animator.SetTrigger("Possess");
		animator.SetBool("Possessing", true);

		possesion = gameObject;

		await Task.Delay(200);
		possessParticle.GetAtPosAndRot<PooledMonoBehaviour>(possesion.transform.position, Quaternion.identity);
		await Task.Delay(800);
		isMoving = false;
		isRotating = false;

		for (int i = 0; i < meshRenderer.Length; i++)
		{
			meshRenderer[i].enabled = false;
		}

		void HandleDisabling(GameObject gameObject)
		{
			interactor.enabled = false;

			interactor.GetComponent<Collider>().enabled = false;
			if (gameObject.TryGetComponent(out Collider col)) col.enabled = false;
			SpriteRenderer renderer = gameObject.GetComponentInChildren<SpriteRenderer>(true);

			if (renderer != null) renderer.gameObject.SetActive(true);
		}

        void HandleAudio(GameObject gameObject)
        {
			if (gameObject.GetComponentInChildren<LightEmitter>())
			{
				AudioManager.Instance.PlaySoundEffect(possessLightSound);
			}
			else
			{
                AudioManager.Instance.PlaySoundEffect(possessionSound);
            }
        }
    }

	public async void StopPossesion()
	{
		if (possesion == null)
		{
			return;
		}

		HandleAudio(possesion);

        Vector3 pos = possesion.transform.position;
		pos.y += 0.75f;
		pos = GetOutPos(pos);

		for (int i = 0; i < meshRenderer.Length; i++)
		{
			meshRenderer[i].enabled = true;
		}

		pos.y = whyLevel;
		transform.position = pos;

		Vector3 dir = (possesion.transform.position - pos).normalized;
		transform.localRotation = Quaternion.LookRotation(dir, Vector3.up);
		animator.SetBool("Possessing", false);
		possessing = false;

		interactor.GetComponent<Collider>().enabled = true;
		if (possesion.TryGetComponent(out Collider colldier)) colldier.enabled = true;
		if (possesion.TryGetComponent(out Rotateable rot)) rot.StopPossessing();
		
		PossesionShaderManager shaderManager = possesion.GetComponentInChildren<PossesionShaderManager>();
		if (shaderManager != null) StartCoroutine(shaderManager.Activate()); 

		SpriteRenderer renderer = possesion.GetComponentInChildren<SpriteRenderer>();

		if (renderer != null) renderer.gameObject.SetActive(false);

		await Task.Delay(200);
		possessParticle.GetAtPosAndRot<PooledMonoBehaviour>(possesion.transform.position, Quaternion.identity);
		possesion = null;

		await Task.Delay(800);
		movement.Unlock();
		interactor.enabled = true;

        void HandleAudio(GameObject gameObject)
        {
            if (gameObject.GetComponentInChildren<LightEmitter>())
            {
                AudioManager.Instance.PlaySoundEffect(possessLightSound);
            }
            else
            {
                AudioManager.Instance.PlaySoundEffect(possessionSound);
            }
        }
    }

	private Vector3 GetOutPos(Vector3 pos)
	{
		if (CheckDirection(possessDirection))
		{
			pos += possessDirection * 1.5f;
		}
		else if (CheckDirection(Vector3.right))
		{
			pos += Vector3.right * 1.5f;
		}
		else if (CheckDirection(Vector3.forward))
		{
			pos += Vector3.forward * 1.5f;
		}
		else if (CheckDirection(Vector3.left))
		{
			pos += Vector3.left * 1.5f;
		}
		else
		{
			pos += Vector3.back * 1.5f;
		}

		return pos;
	}

	private bool CheckDirection(Vector3 dir)
	{
		Debug.DrawRay(possesion.transform.position, dir * 1.5f, Color.red);
		return !Physics.Raycast(possesion.transform.position, dir * 1.5f, 1.5f, collisionLayerMask);
	}

	public void Save(SaveData data)
	{

	}

	public void Load(SaveData data, bool reset = false)
	{
		for (int i = 0; i < meshRenderer.Length; i++)
		{
			meshRenderer[i].enabled = true;
		}
		animator.SetBool("Possessing", false);
		possessing = false;
		possesion = null;
		movement.Unlock();
		interactor.enabled = true;
	}

}
