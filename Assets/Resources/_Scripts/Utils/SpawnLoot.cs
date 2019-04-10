using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLoot : MonoBehaviour{

    [System.Serializable]
    public class ItemRarityPair
    {
        public Item item;
        public float chance;
    }

    System.Random rand = new System.Random(System.DateTime.Now.Millisecond);

    public ItemRarityPair[] itemPool; // The item pool for this entity. Contains Items and their associated chance of dropping.

	public void Spawn()
    {
        // Check each item, and determine if it spawns.
        foreach(ItemRarityPair pair in itemPool)
        {
            if(rand.NextDouble() < pair.chance)
            {
                Instantiate(pair.item, transform.position + new Vector3(Random.insideUnitCircle.x, Random.insideUnitCircle.y, 0), Quaternion.identity);
            }
        }
    }
	
}
