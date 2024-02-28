using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterAnimation : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //we do -0.00001f here because not doing so would display the animation's beginning again
        Destroy(animator.gameObject, stateInfo.length - 0.0001f);
    }

}
