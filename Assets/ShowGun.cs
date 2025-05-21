using UnityEngine;

public class ShowGun : StateMachineBehaviour
{
    private bool hasTriggered;

    // Called every frame while in the state
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!hasTriggered && stateInfo.normalizedTime >= 0.5f && !animator.IsInTransition(layerIndex))
        {
            hasTriggered = true; // prevent it from triggering multiple times

            // Call your logic here
            animator.gameObject.transform.GetChild(0).gameObject.SetActive(true); // or a custom event
        }
    }

    // Reset flag on enter
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        hasTriggered = false;
    }
}
