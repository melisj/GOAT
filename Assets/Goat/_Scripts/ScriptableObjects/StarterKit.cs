using Goat.Player;
using Goat.Storage;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using static DayNightCycle;

public class StarterKit : MonoBehaviour
{
    [SerializeField] private Money money;
    [SerializeField] private Electricity electricity;
    [SerializeField] private TimeOfDay timeOfDay;
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private SatisfactionLevel satisfactionLevel;

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
        satisfactionLevel.Satisfaction = 0;
        money.Amount = money.StarterAmount;
        playerInventory.InitInventory();
    }

    private void OnDisable()
    {
        electricity.ClearAll();
    }
}