using Goat.Storage;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI electricityText;
    [SerializeField] private Electricity electricity;


    private void OnEnable() {
        electricity.ElectricityChangedEvent += ElectricityChanged;
        ElectricityChanged(0, 0);
    }

    private void OnDisable() {
        electricity.ElectricityChangedEvent -= ElectricityChanged;
    }

    private void ElectricityChanged(int newUsed, int newCapacity) {
        electricityText.text = string.Format("{0} / {1}", newUsed, newCapacity);

    }
}
