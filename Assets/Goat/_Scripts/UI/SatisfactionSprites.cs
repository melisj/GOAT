using UnityEngine;

[CreateAssetMenu(fileName = "SatisfactionSprites", menuName = "ScriptableObjects/GlobalVariables/SatisfactionSprites")]
public class SatisfactionSprites : ScriptableObject
{
    [SerializeField] private Sprite happy;
    [SerializeField] private Sprite unhappy;
    [SerializeField] private Sprite neutral;

    public Sprite Happy => happy;
    public Sprite UnHappy => unhappy;
    public Sprite Neutral => neutral;
}