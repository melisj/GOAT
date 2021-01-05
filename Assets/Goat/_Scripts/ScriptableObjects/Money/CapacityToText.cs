using UnityEngine;
using TMPro;
using UnityAtoms.BaseAtoms;
using UnityAtoms;

public class CapacityToText : MonoBehaviour, IAtomListener<int>
{
    [SerializeField] private TextMeshProUGUI capacityText;
    [SerializeField] private IntVariable capacity;

    public void OnEventRaised(int item)
    {
        ChangeText(capacity.Value);
    }

    private void Awake()
    {
        ChangeText(capacity.Value);
    }

    private void OnEnable()
    {
        capacity.Changed.RegisterListener(this);
    }

    private void OnDisable()
    {
        capacity.Changed.UnregisterListener(this);
    }

    private void ChangeText(float value)
    {
        capacityText.text = ((int)value).ToString() + "/30";
    }
}