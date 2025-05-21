using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class LootDrop : NetworkBehaviour
{
    public LootTable lootTable;

    public override void OnDestroy()
    {
        DropLoot();
    }

    public void DropLoot()
    {
        if (!IsServer) return;

        if (lootTable == null || lootTable.lootTable.Count == 0)
            return;

        float roll = Random.Range(0f, 100f);
        float cumulative = 0f;

        foreach (var loot in lootTable.lootTable)
        {
            cumulative += loot.chance;

            if (roll <= cumulative)
            {
                if (loot.lootObject != null)
                {
                    ServerLootSpawner.Instance.SpawnLootServerRpc(loot.lootObject.name, transform.position.x, transform.position.y, transform.position.z);
                }
                return;
            }
        }
    }
}
