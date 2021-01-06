using UnityEngine;

public class WithOwner<T>
{
    private T gtype;
    private GameObject owner;

    public WithOwner(T gtype, GameObject owner)
    {
        this.gtype = gtype;
        this.owner = owner;
    }

    public T Gtype => gtype;
    public GameObject Owner => owner;
}