using Goat.Storage;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using static DayNightCycle;

public class StarterKit : MonoBehaviour
{
    [SerializeField] private Money money;
    [SerializeField] private Electricity electricity;
    [SerializeField] private TimeOfDay timeOfDay;

    private void Awake()
    {
        ResetValues();
    }

    private void ResetValues()
    {
        Buyable[] buyables = Resources.LoadAll<Buyable>("");

        for (int i = 0; i < buyables.Length; i++)
        {
            buyables[i].Amount = buyables[i].StarterAmount;
        }
        timeOfDay.Reset();
        money.Amount = money.StarterAmount;
    }

    private void OnDisable()
    {
        electricity.ClearAll();
    }
}