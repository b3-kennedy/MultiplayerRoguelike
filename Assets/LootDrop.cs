using Unity.Netcode;
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

        foreach (var loot in lootTable.lootTable)
        {
            float roll = Random.Range(0f, 100f);

            if (roll <= loot.chance)
            {
                if (loot.lootObject != null)
                {
                    ServerLootSpawner.Instance.SpawnLootServerRpc(
                        loot.lootObject.name,
                        transform.position.x,
                        transform.position.y,
                        transform.position.z
                    );
                }
            }
        }
    }
}
