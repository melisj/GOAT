using UnityEngine;

[CreateAssetMenu(fileName = "SelectDeselectSprites", menuName = "ScriptableObjects/GlobalVariables/SelectDeselectSprites")]
public class SelectDeselectSprites : ScriptableObject
{
    [SerializeField] private Sprite selected;
    [SerializeField] private Sprite deselected;

    public Sprite Selected => selected;
    public Sprite Deselected => deselected;
}