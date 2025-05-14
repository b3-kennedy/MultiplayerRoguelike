using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
    public float health;

    [ClientRpc(RequireOwnership = false)]
    public void TakeDamageClientRpc(float damage)
    {
        health -= damage;
        if(health <= 0)
        {
            
            Destroy(gameObject);
        }
    }
}
