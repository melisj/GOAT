using UnityEngine;
using UnityEngine.UI;

public class QuitOnClick : MonoBehaviour
{
    [SerializeField] private Button button;

    private void Awake()
    {
        button.onClick.AddListener(() => Application.Quit());
    }
}