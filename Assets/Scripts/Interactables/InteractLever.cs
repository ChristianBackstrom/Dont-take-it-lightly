using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractLever : MonoBehaviour, IHumanInteractable
{
    [Header("Data")]
    [SerializeField]
    private InteractableData data;

    [Header("Base")]
    [SerializeField]
    protected UnityEvent OnClick;

    [Header("Lever")]
    [SerializeField]
    private UnityEvent OnLeverActivated;

    [SerializeField]
    private UnityEvent OnLeverDeactivated;

    private bool isActivated = false;
    private bool cooledDowned = true;
    private float timer = 0;

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

        OnClick?.Invoke();

        isActivated = !isActivated;

        if (isActivated)
        {
            OnLeverActivated?.Invoke();
        }
        else
        {
            OnLeverDeactivated?.Invoke();
        }

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

    }

    public void ResetHighlighter()
    {
        
    }
}
