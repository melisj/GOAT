using UnityEngine;

[CreateAssetMenu(fileName = "ConjunctiveAdverbs", menuName = "ScriptableObjects/GlobalVariables/ConjunctiveAdverbs")]
public class ConjunctiveAdverbs : ScriptableObject
{
    [SerializeField] private string[] additions;
    [SerializeField] private string[] contrasts;

    public string GetAddition => additions[Random.Range(0, additions.Length)];
    public string GetContrast => contrasts[Random.Range(0, contrasts.Length)];

}