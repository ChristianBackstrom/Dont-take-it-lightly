using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractButton : MonoBehaviour, IHumanInteractable
{
    [Header("Data")]
    [SerializeField]
    private InteractableData data;

    [Header("Base")]
    [SerializeField]
    protected UnityEvent OnClick;

    private bool cooledDowned = true;
    private float timer = 0;


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

    public virtual bool Interact(Transform interactor)
    {
        if (!IsInteractable(interactor, out float dist))
        {
            return false;
        }

        OnClick?.Invoke();

        print("Button Pressed");

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
        //currently the button has no mesh and therefore applying the fresnel shader is unnecessary
    }
    public void ResetHighlighter()
    {
    }
}
