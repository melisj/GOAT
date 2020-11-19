using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Goat.Resource 
{
    public class ResourceUI : MonoBehaviour
    {
        [SerializeField] private Resource resource;
        [SerializeField] private TextMeshProUGUI textMesh;
        [SerializeField] private Image image;
        private void Awake()
        {
            image.sprite = resource.Image;
            textMesh.text = resource.ResourceType.ToString() + ": " + resource.Amount.ToString();
            resource.AmountChanged += Resource_AmountChanged;
        }

        private void Resource_AmountChanged(object sender, int amount)
        {
            textMesh.text = resource.ResourceType.ToString() + ": " + resource.Amount.ToString();
        }

        private void OnDisable()
        {
            resource.AmountChanged -= Resource_AmountChanged;
        }
    }
}