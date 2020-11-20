using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Goat.Storage
{
    public class ResourceUI : MonoBehaviour
    {
        [SerializeField] private Resource resource;
        [SerializeField] private TextMeshProUGUI textMesh;
        [SerializeField] private Image image;
        //private void Awake()
        //{
        //    image.sprite = resource.Image;
        //    textMesh.text = resource.ResourceType.ToString() + ": " + resource.Amount.ToString();
        //    resource.AmountChanged += Resource_AmountChanged;
        //}

        public void SetupUI(Resource res)
        {
            resource = res;
            image.sprite = resource.Image;
            textMesh.text = resource.Amount.ToString();
            resource.AmountChanged += Resource_AmountChanged;
        }

        private void Resource_AmountChanged(object sender, int amount)
        {
            textMesh.text = resource.Amount.ToString();
        }
        

    }
}