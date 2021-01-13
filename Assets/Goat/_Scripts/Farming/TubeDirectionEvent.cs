using UnityEngine;
using UnityAtoms;

namespace Goat.Farming
{
    [EditorIcon("atom-icon-cherry")]
    [CreateAssetMenu(menuName = "Unity Atoms/Events/TubeDirectionEvent", fileName = "TubeDirectionEvent")]
    public class TubeDirectionEvent : AtomEvent<WithOwner<TubeDirection>> 
    {
    }

}