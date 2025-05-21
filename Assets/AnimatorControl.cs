using UnityEngine;

public class AnimatorControl : MonoBehaviour
{
    Animator anim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (anim.IsInTransition(0)) return;

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Swap"))
        {
            if (stateInfo.normalizedTime >= 1f)
            {
                Debug.Log("Hide");
                transform.GetChild(0).gameObject.SetActive(false);
            }
        }

        if (stateInfo.IsName("Unswap"))
        {
            if (stateInfo.normalizedTime >= 1f)
            {
                Debug.Log("Show");
                transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }
}
