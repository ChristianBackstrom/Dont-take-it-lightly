using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class Rotateable : MonoBehaviour, IGhostInteractable
{
	[Header("Data")]
	[SerializeField]
	private InteractableData data;

	[Header("Base")]
	[SerializeField]
	private UnityEvent OnClick;

	[Header("VFX")]
	[SerializeField]
	private PooledMonoBehaviour predictionPrefab; 

	[SerializeField]
	private LightEmitter[] lightEmitters;

	private List<PooledMonoBehaviour> predictions = new List<PooledMonoBehaviour>();

	private Quaternion lastRot;

	private float predictionTimer = 0;
	private bool cooledDowned = true;
	private float timer = 0;
	private bool possessed = false;

	private const float RotationAmount = 22.5f;

	//reference to the highlighter so other scripts can check if it is null or not
	InteractableHighlighter highlighter;

	//place to store the interactor (so other scripts can check if it is null or not)
	GhostInteractor ghostInteractor;

	private void Start()
	{
		highlighter = GetComponentInChildren<InteractableHighlighter>();

        float rotationPercentage = transform.rotation.eulerAngles.y / 360;
		int rotations = Mathf.RoundToInt((360f / RotationAmount) * rotationPercentage);
		transform.rotation = Quaternion.AngleAxis(RotationAmount * rotations, Vector3.up);
	}

	private void Update()
	{
		if (!cooledDowned)
		{
			timer += Time.deltaTime;
			if (timer > data.Cooldown)
			{
				timer = 0;
				cooledDowned = true;
			}
		}

		if (possessed)
		{
			if (lastRot == transform.rotation)
			{
				predictionTimer += Time.deltaTime;
				if (predictionTimer >= 2.5f && predictions.Count == 0)
				{
					predictionTimer = 0;
					TrySpawnPredictions();
				}
			}
			else
			{
				RemovePredictions();
				predictionTimer = 0;
			}
		}

		lastRot = transform.rotation;
	}

	private void TrySpawnPredictions()
	{
		for (int i = 0; i < lightEmitters.Length; i++)
		{
			lightEmitters[i].Cleanup();
            LightTree tree = lightEmitters[i].ShootRay(lightEmitters[i].transform.position, lightEmitters[i].transform.forward);
            InvestigateBranch(tree, null);
        }
		
    }

	private void InvestigateBranch(LightTree tree, LightTree? parent)
	{
        if (!parent.HasValue)
		{
			InvestigateKids(tree);
			return;
		}

		float dist = Vector3.Distance(tree.Origin, transform.position);
		if (dist < 3f)
		{
			predictions.Add(predictionPrefab.GetAtPosAndRot<PooledMonoBehaviour>(tree.Origin, Quaternion.LookRotation(tree.Direction)));
			predictions.Add(predictionPrefab.GetAtPosAndRot<PooledMonoBehaviour>(tree.Origin - parent.Value.Direction, Quaternion.LookRotation(parent.Value.Direction)));
			return;
		}

		InvestigateKids(tree);

        void InvestigateKids(LightTree tree)
		{
            if (tree.LightChildren == null || tree.LightChildren.Count == 0)
            {
				return;
            }

            for (int i = 0; i < tree.LightChildren.Count; i++)
			{
				InvestigateBranch(tree.LightChildren[i], tree);
			}
		}
	}

	private void RemovePredictions()
	{
		for (int i = 0; i < predictions.Count; i++)
		{
			predictions[i].gameObject.SetActive(false);
		}

		predictions.Clear();
	}

	public virtual bool Interact(Transform interactor)
	{
		if (!IsInteractable(interactor, out float dist))
		{
			return false;
		}

		interactor.GetComponent<GhostPossesion>().Possess(gameObject, false, true);
		possessed = true;

		OnClick?.Invoke();
		cooledDowned = false;
		return true;
	}

	public bool IsInteractable(Transform interactor, out float distance)
	{
		distance = Vector3.Distance(interactor.position, transform.position);
		bool dist = distance < data.Range;

		return dist && cooledDowned;
	}

	public void Highlight()
	{
		//highlighter.currStrength += 8 * Time.fixedDeltaTime;
	}

	public void ResetHighlighter()
	{
		//highlighter.Reset();
	}

	public void StopPossessing()
	{
		RemovePredictions();
		

		possessed = false;
		predictionTimer = 0;
	}
}
