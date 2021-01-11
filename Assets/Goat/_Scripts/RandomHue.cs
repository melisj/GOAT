using Goat.Helper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MaterialPropertySetter))]
public class RandomHue : MonoBehaviour
{
    [SerializeField] private MaterialPropertySetter propSetter;

    private void Awake()
    {
        propSetter.MaterialValueToChanges[0].NewFloat = Random.Range(0, 361);
        propSetter.ModifyValues();
    }
}