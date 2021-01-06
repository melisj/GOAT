using UnityAtoms.BaseAtoms;
using UnityEngine;

[CreateAssetMenu(fileName = "SatisfactionLevel", menuName = "ScriptableObjects/GlobalVariables/SatisfactionLevel")]
public class SatisfactionLevel : ScriptableObject
{
    [SerializeField] private int satisfaction;
    [SerializeField] private IntEvent onSatisfactionChanged;

    public int Satisfaction
    {
        get => satisfaction;
        set
        {
            satisfaction = value;
            onSatisfactionChanged.Raise(value);
        }
    }
}