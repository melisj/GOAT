using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityAtoms.BaseAtoms;

[System.Serializable]
public class Narrative
{
    [SerializeField]
    private Sprite sprite;
    [SerializeField]
    private string sentence;

    public Sprite Sprite { get => sprite; set => sprite = value; }
    public string Sentence { get => sentence; set => sentence = value; }
}

public class NarrativeManager : MonoBehaviour
{
    [SerializeField]
    private Narrative[] narrative;
    [SerializeField] private VoidEvent OnNarrativeFinished;
    private TextMeshProUGUI textMesh;
    private Image portrait;
    private int narrativeIndex = 0;

    private void Start()
    {
        textMesh = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        portrait = gameObject.GetComponentInChildren<Image>();

        textMesh.text = narrative[narrativeIndex].Sentence;
        portrait.sprite = narrative[narrativeIndex].Sprite;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            NextSentence();
        }
    }

    public void NextSentence()
    {
        if (narrativeIndex < narrative.Length - 1)
        {
            narrativeIndex++;
            textMesh.text = narrative[narrativeIndex].Sentence;
            portrait.sprite = narrative[narrativeIndex].Sprite;
            portrait.transform.DOPunchScale(Vector3.one / 3, 0.3f, 10, 10f);
        }
        else
        {
            OnNarrativeFinished.Raise();
            Destroy(this.gameObject);
        }
    }
}