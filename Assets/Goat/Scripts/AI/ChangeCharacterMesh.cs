using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.AI
{
    public class ChangeCharacterMesh : MonoBehaviour
    {
        [SerializeField] private CharacterMeshes meshes;
        private void Awake()
        {
            SkinnedMeshRenderer skinnedMesh = GetComponent<SkinnedMeshRenderer>();
            if (meshes != null)
            {
                int randex = Random.Range(0, meshes.characterMeshes.Count);
                print(randex);
                skinnedMesh.sharedMesh = meshes.characterMeshes[randex];
            }
        }
    }
}

