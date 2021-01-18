using Goat.Farming.Electricity;
using Goat.Grid.UI;
using Goat.Player;
using UnityAtoms.BaseAtoms;
using UnityEngine;

public class StarterKit : MonoBehaviour
{
    [SerializeField] private Money money;
    [SerializeField] private Electricity electricity;
    [SerializeField] private TimeOfDay timeOfDay;
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private SatisfactionLevel satisfactionLevel;
    [SerializeField] private GridUIInfo uiInfo;

    [SerializeField] private VoidEvent onGridReset;

    private void Awake()
    {
        ResetValues();
    }

    private void ResetValues()
    {
        timeOfDay.Reset();
        satisfactionLevel.Satisfaction = 0;
        money.Amount = money.StarterAmount;
        playerInventory.InitInventory();
        uiInfo.CurrentUIElement = UIElement.None;
    }

    private void OnEnable()
    {
        onGridReset.Register(electricity.ClearAll);
    }

    private void OnDisable()
    {
        electricity.ClearAll();
        onGridReset.Unregister(electricity.ClearAll);
    }
}