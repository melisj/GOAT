using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.AI
{
    public class SetCharacterMesh : MonoBehaviour
    {
        [SerializeField] private Characters characters;
        [SerializeField] private GameObject root;

        public Character Character { get; private set; }

        private void Awake()
        {
            SkinnedMeshRenderer skinnedMesh = GetComponent<SkinnedMeshRenderer>();
            if (characters != null)
            {
                //print(randex);
                Character = characters.GetCharacter;
                skinnedMesh.sharedMesh = Character.Mesh;
                root.name = Character.CharacterNames.GetName;
            }
        }
    }
}