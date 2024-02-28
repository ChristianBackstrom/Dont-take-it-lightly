using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using System;

public class Interactor : MonoBehaviour
{
    //inputhandler of the parent gameObject
    private InputHandler inputHandler;
    //the ButtonPromptHandler is what allows us to instantiate buttonprompts
    private ButtonPromptHandler promptHandler;
    private PlayerEntry playerEntry;
    private InputDetector inputDetector;



    //reference to the interactable were currently interacting with. 
    public IHumanInteractable currInteractingWith { get; set; }

    //reference to the currently highlighted interactable. buttonprompthandler checks 
    public IHumanInteractable currHighlighted { get; private set; }


    private bool firstInit = true; //bool which keeps track of whether or not start has run once or not 
    private void Start()
    {
        inputHandler = GetComponent<InputHandler>();
        inputDetector = FindObjectOfType<InputDetector>();
        promptHandler = GameObject.Find("SamButtonPrompts").GetComponent<ButtonPromptHandler>();
        firstInit = false;
    }

    void Update()
    {
        FindClosestInteractable();
        if (inputHandler.Controls == null) return;

        if (inputHandler.Interact.WasPressedThisFrame())
        {
            TryInteract();
        }
    }

    private void TryInteract()
    {
        IHumanInteractable[] ints = FindObjectsOfType<MonoBehaviour>().OfType<IHumanInteractable>().ToArray();
        float distance = float.MaxValue;
        int index = -1;

        for (int i = 0; i < ints.Length; i++)
        {
            if (ints[i].IsInteractable(transform, out float dist))
            {
                if (dist < distance)
                {
                    distance = dist;
                    index = i;
                }
            }
        }

        if (index != -1)
        {
            ints[index].Interact(transform);
            //set currInteractingWith to found interactable
            currInteractingWith = ints[index];
        }
    }
    void FindClosestInteractable()
    {
        IHumanInteractable[] ints = FindObjectsOfType<MonoBehaviour>().OfType<IHumanInteractable>().ToArray();
        float distance = float.MaxValue;
        int index = -1;

        for (int i = 0; i < ints.Length; i++)
        {
            if (ints[i].IsInteractable(transform, out float dist))
            {
                if (dist < distance)
                {
                    distance = dist;
                    index = i;
                }
            }
        }
        if (index != -1)
        {
			currHighlighted = ints[index];
            //highlight interactable
            currHighlighted.Highlight();
            //TODO: show interact button prompt
            promptHandler.DisplayPrompt(inputDetector.connectedDevices[inputHandler.Device], transform.position);
			return;
        }
		currHighlighted = null;
    }
    private void OnEnable()
    {
        //keep code in OnEnable() from running in Start()
        if (firstInit) return;
        ResetNearestInteractable();
    }

    private void ResetNearestInteractable()
    {
        if (currInteractingWith != null)
        {
            currInteractingWith.ResetHighlighter();
            currInteractingWith = null;
        }
        IHumanInteractable[] ints = FindObjectsOfType<MonoBehaviour>().OfType<IHumanInteractable>().ToArray();
        float distance = float.MaxValue;
        int index = -1;
        for (int i = 0; i < ints.Length; i++)
        {
            if (ints[i].IsInteractable(transform, out float dist))
            {
                if (dist < distance)
                {
                    distance = dist;
                    index = i;
                }
            }
        }
        if (index != -1)
        {
            ints[index].ResetHighlighter();
        }
    }
}
