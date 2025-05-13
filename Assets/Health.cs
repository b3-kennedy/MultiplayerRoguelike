using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
    public float health;

    [ClientRpc(RequireOwnership = false)]
    public void TakeDamageClientRpc(float damage)
    {
        health -= damage;
        Debug.Log("deal damage client");
        if(health <= 0)
        {
            
            Destroy(gameObject);
        }
    }
}
