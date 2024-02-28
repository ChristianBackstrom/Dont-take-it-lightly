using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GhostInteractor : MonoBehaviour
{
    private InputHandler inputHandler;
    private IGhostInteractable[] interactables;


    public IGhostInteractable currHighlighted { get; private set; }
    public IGhostInteractable currInteractingWith { get; set; }
    private PlayerEntry playerEntry;
    private InputDetector inputDetector;
    private ButtonPromptHandler promptHandler; //one prompthandler is needed per character

    bool firstInit = true;

    private void Start()
    {
        promptHandler = GameObject.Find("GhostButtonPrompts").GetComponent<ButtonPromptHandler>();
        interactables = FindObjectsOfType<MonoBehaviour>().OfType<IGhostInteractable>().ToArray();
        inputHandler = GetComponent<InputHandler>();
        inputDetector = FindObjectOfType<InputDetector>();
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
        float distance = float.MaxValue;
        int index = -1;
        //look through each interactable and store the index of each
        // if the distance to them is lower than float "distance"
        for (int i = 0; i < interactables.Length; i++)
        {
            if (interactables[i].IsInteractable(transform, out float dist))
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
            interactables[index].Interact(transform);
            currInteractingWith = interactables[index];
        }
    }
    private void FindClosestInteractable()
    {
        float distance = float.MaxValue;
        int index = -1;

        for (int i = 0; i < interactables.Length; i++)
        {
            if (interactables[i].IsInteractable(transform, out float dist))
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
            currHighlighted = interactables[index];
            //highlight interactable
            currHighlighted.Highlight();
            //TODO: show interact button prompt
            promptHandler.DisplayPrompt(inputDetector.connectedDevices[inputHandler.Device], this.transform.position);
            return;
        }
        currHighlighted = null;
    }
    private void OnEnable()
    {
        //keep code in OnEnable() from running on first init
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
        float distance = float.MaxValue;
        int index = -1;
        for (int i = 0; i < interactables.Length; i++)
        {
            if (interactables[i].IsInteractable(transform, out float dist))
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
            interactables[index].ResetHighlighter();
        }
    }
}
