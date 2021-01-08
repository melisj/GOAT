using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.AI
{
    public class SetCharacterMesh : MonoBehaviour
    {
        [SerializeField] private Characters characters;
        [SerializeField] private GameObject root;

        private void Awake()
        {
            SkinnedMeshRenderer skinnedMesh = GetComponent<SkinnedMeshRenderer>();
            if (characters != null)
            {
                //print(randex);
                Character character = characters.GetCharacter;
                skinnedMesh.sharedMesh = character.Mesh;
                root.name = character.CharacterNames.GetName;
            }
        }
    }
}