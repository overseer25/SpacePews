public class StorageComponent : ShipComponent
{
    // The number of slots the storage component adds to the inventory.
    public int slotCount;

    protected override void Awake()
    {
        base.Awake();
        mounted = false;
        itemType = ItemType.Storage;
    }
}
