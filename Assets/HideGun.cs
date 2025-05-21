using UnityEngine;

public class HideGun : StateMachineBehaviour
{
    private bool hasTriggered;

    // Called every frame while in the state
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!hasTriggered && stateInfo.normalizedTime >= 1f && !animator.IsInTransition(layerIndex))
        {
            hasTriggered = true; // prevent it from triggering multiple times

            // Call your logic here
            animator.gameObject.transform.GetChild(0).gameObject.SetActive(false); // or a custom event
            Debug.Log("Animation ended: Hide object");
        }
    }

    // Reset flag on enter
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        hasTriggered = false;
    }
}
