using UnityEngine;
using System.Collections;
using UnityAtoms;
using Goat.Expenses;

namespace Goat.Events
{
    [EditorIcon("atom-icon-cherry")]
    [CreateAssetMenu(menuName = "Unity Atoms/Events/ExpenseEvent", fileName = "ExpenseEvent")]
    public class ExpenseEvent : AtomEvent<Expense>
    {
    }
}