using Goat.Pooling;

public class TileAnimationPooled : TileAnimation, IPoolObject
{
    public int PoolKey { get; set; }
    public ObjectInstance ObjInstance { get; set; }

    public void OnGetObject(ObjectInstance objectInstance, int poolKey)
    {
        ObjInstance = objectInstance;
        PoolKey = poolKey;
        Create();
    }

    public void OnReturnObject()
    {
        Destroy(() => gameObject.SetActive(false));
    }
}