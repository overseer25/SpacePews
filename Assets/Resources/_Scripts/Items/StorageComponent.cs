public class StorageComponent : ShipComponent
{
    // The number of slots the storage component adds to the inventory.
    public int slotCount;

    void Awake()
    {
        itemColor = ItemColors.colors[(int)itemTier];
        itemType = ItemType.Storage;
    }
}
