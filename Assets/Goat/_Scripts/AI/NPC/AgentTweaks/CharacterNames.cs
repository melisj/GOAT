using UnityEngine;

namespace Goat.AI
{
    [CreateAssetMenu(fileName = "CharacterNames", menuName = "ScriptableObjects/GlobalVariables/CharacterNames")]
    public class CharacterNames : ScriptableObject
    {
        [SerializeField] private string[] names;
        public string GetName => names[Random.Range(0, names.Length)];
    }
}