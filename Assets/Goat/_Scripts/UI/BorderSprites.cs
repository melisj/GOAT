using UnityEngine;

[CreateAssetMenu(fileName = "BorderSprites", menuName = "ScriptableObjects/GlobalVariables/BorderSprites")]
public class BorderSprites : ScriptableObject
{
    [SerializeField] private Sprite left;
    [SerializeField] private Sprite middle;
    [SerializeField] private Sprite right;

    public Sprite Left => left;
    public Sprite Middle => middle;
    public Sprite Right => right;
}