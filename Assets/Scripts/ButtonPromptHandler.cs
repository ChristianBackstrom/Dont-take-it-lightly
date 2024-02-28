using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//<Summary>
//ButtonPromptHandler is a script which handles one player's button prompts at a time.
//
public class ButtonPromptHandler : MonoBehaviour
{
    public Vector3 offset;

    //Im imagining that only one prompt would be needed at a time, therefore the variable is declared up here 
    GameObject prompt; //UI prefab to instantiate

    //do we need a reference to the target player as well? probably
    [SerializeField] Transform transformToFollow;

    Animator animator;
    GhostInteractor ghostInteractor;
    Interactor interactor;
    //we need to use worldtoscreenpoint() so a cam is necessary
    Camera cam;



    private void Start()
    {
        //the animator is on the child component. This script is (as of now) meant to be on a canvas object
        animator = this.GetComponentInChildren<Animator>();
        //hopefully this is still appropriate despite us using cinemachine
        cam = Camera.main;
        interactor = transformToFollow.GetComponent<Interactor>();
        ghostInteractor = transformToFollow.GetComponent<GhostInteractor>();
    }
    private void Update()
    {
        if (animator == null) return;
        //TODO: set animator states based on context
        if (interactor)
        {
            HandleHumanPrompts();
        }
        if (ghostInteractor)
        {
            HandleGhostPrompts();
        }
    }
    private void HandleHumanPrompts()
    {
        if (interactor.currInteractingWith != null)
        {
            animator.SetTrigger("StopShowing");
            //risky business below
            interactor.currInteractingWith = null;
        }

        if (interactor.currHighlighted == null)
            animator.SetTrigger("StopShowing");
    }

    private void HandleGhostPrompts()
    {
        if (ghostInteractor.currInteractingWith != null)
        {
            animator.SetTrigger("StopShowing");
            //risky business below
            ghostInteractor.currInteractingWith = null;
        }

        if (ghostInteractor.currHighlighted == null)
            animator.SetTrigger("StopShowing");
    }
    public void DisplayPrompt(PlayerEntry entry, Vector3 pos)
    {
        if (!this.prompt)
        {
            Vector3 newPos = cam.WorldToScreenPoint(pos);
            this.prompt = Instantiate(entry.instance, newPos, Quaternion.identity, this.transform);
            animator = prompt.GetComponent<Animator>();
        }
    }

}
