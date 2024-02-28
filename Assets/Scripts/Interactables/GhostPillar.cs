using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GhostPillar : MonoBehaviour, IGhostInteractable
{
    [Header("Data")]
    [SerializeField]
    private InteractableData data;

    [Header("Base")]
    [SerializeField]
    protected UnityEvent OnClick;

    private bool cooledDowned = true;
    private float timer = 0;

    InteractableHighlighter highlighter;

    private void Start()
    {
        
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
    }

    public bool Interact(Transform interactor)
    {
        if (!IsInteractable(interactor, out float dist))
        {
            return false;
        }

        GhostPossesion ghostPossesion = interactor.GetComponent<GhostPossesion>();
        ghostPossesion.Possess(gameObject, true, false);
        PossesionShaderManager shaderManager = gameObject.GetComponentInChildren<PossesionShaderManager>();
	    if (shaderManager != null) StartCoroutine(shaderManager.Activate()); 

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
}
