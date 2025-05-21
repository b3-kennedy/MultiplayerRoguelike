using Unity.Netcode;
using UnityEngine;

public class LootCollectMove : NetworkBehaviour
{
    public bool canBeCollected = false;
    public Transform target;

    public float lerpSpeed = 5f;

    public void SetCanBeCollected(bool value)
    {
        canBeCollected = value;
    }

    public bool GetCanBeCollectedValue()
    {
        return canBeCollected;
    }

    public void SetTarget(Transform t)
    {
        target = t;
    }

    public Transform GetTarget()
    {
        return target;
    }

    // Update is called once per frame
    void Update()
    {



        if (canBeCollected && target)
        {
            transform.position = Vector3.Lerp(transform.position, target.position, lerpSpeed * Time.deltaTime);
        }

        if (target && Vector3.Distance(transform.position, target.position) <= 1f)
        {
            Destroy(gameObject);
        }
    }
}
