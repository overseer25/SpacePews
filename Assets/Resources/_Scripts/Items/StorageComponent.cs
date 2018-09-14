public class StorageComponent : ShipComponent
{

    void Awake()
    {
        itemColor = ItemColors.colors[(int)itemTier];
        itemType = ItemType.Storage;
    }
}
