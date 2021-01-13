using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InfoBox : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI infoTextFitterTM;
    [SerializeField] private TextMeshProUGUI infoTextTM;
    [SerializeField] private BorderContentFitter contentFitter;

    public void Setup(Vector3 pos, OnHoverInfo hoverInfo)
    {
        transform.position = pos;
        infoTextFitterTM.text = hoverInfo.InfoToShow;
        infoTextTM.text = hoverInfo.InfoToShow;
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}