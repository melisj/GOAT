using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace Goat.Helper
{
    public class MaterialPropertySetter : MonoBehaviour
    {
        [SerializeField] private List<MaterialValueToChange> materialValueToChanges;
        [SerializeField] private Renderer renderer;
        [SerializeField] private int propertyID;

        private MaterialPropertyBlock propertyBlock;

        public List<MaterialValueToChange> MaterialValueToChanges { get => materialValueToChanges; set => materialValueToChanges = value; }

        private void Start()
        {
            propertyBlock = new MaterialPropertyBlock();
            renderer = GetComponent<Renderer>();

            if (!renderer)
            {
                Debug.LogErrorFormat("<b>Renderer is null at object {0}</b>", gameObject);
                return;
            }
            Setup();
        }

        public void ModifyValues()
        {
            Setup();
        }

        // Use this for initialization
        [Button]
        private void Setup()
        {
            if (propertyBlock == null)
            {
                propertyBlock = new MaterialPropertyBlock();
            }
            if (!renderer)
            {
                renderer = GetComponent<Renderer>();
            }

            if (!renderer)
            {
                Debug.LogErrorFormat("<b>Renderer is null at object {0}</b>", gameObject);
                return;
            }

            for (int i = 0; i < materialValueToChanges.Count; i++)
            {
                MaterialValueToChange item = materialValueToChanges[i];

                propertyID = Shader.PropertyToID(item.PropertyName);
                switch (item.MaterialValue)
                {
                    case MaterialValue.Float:
                        ChangeFloat(item);
                        break;

                    case MaterialValue.Int:
                        ChangeInt(item);
                        break;

                    case MaterialValue.Color:
                        ChangeColor(item);
                        break;

                    case MaterialValue.ColorHDR:
                        ChangeColorHDR(item);
                        break;

                    case MaterialValue.Texture:
                        ChangeTexture(item);
                        break;
                }
            }

            renderer.SetPropertyBlock(propertyBlock);
        }

        private void ChangeColor(MaterialValueToChange materialValueToChange)
        {
            propertyBlock.SetColor(propertyID, materialValueToChange.NewColor);
        }

        private void ChangeColorHDR(MaterialValueToChange materialValueToChange)
        {
            propertyBlock.SetColor(propertyID, materialValueToChange.NewColorHDR);
        }

        private void ChangeFloat(MaterialValueToChange materialValueToChange)
        {
            propertyBlock.SetFloat(propertyID, materialValueToChange.NewFloat);
        }

        private void ChangeInt(MaterialValueToChange materialValueToChange)
        {
            propertyBlock.SetInt(propertyID, materialValueToChange.NewInt);
        }

        private void ChangeTexture(MaterialValueToChange materialValueToChange)
        {
            propertyBlock.SetTexture(propertyID, materialValueToChange.NewTexture);
        }
    }

    [System.Serializable]
    public class MaterialValueToChange
    {
        [SerializeField] private MaterialValue materialValueToChange;
        [SerializeField] private string propertyName;
        [SerializeField, ShowIf("materialValueToChange", MaterialValue.Color)] private Color newColor;
        [SerializeField, ShowIf("materialValueToChange", MaterialValue.ColorHDR), ColorUsageAttribute(true, true)] private Color newColorHDR;
        [SerializeField, ShowIf("materialValueToChange", MaterialValue.Float)] private float newFloat;
        [SerializeField, ShowIf("materialValueToChange", MaterialValue.Int)] private int newInt;
        [SerializeField, ShowIf("materialValueToChange", MaterialValue.Texture)] private Texture newTexture;
        public MaterialValue MaterialValue { get => materialValueToChange; set => materialValueToChange = value; }
        public string PropertyName { get => propertyName; set => propertyName = value; }
        public Color NewColor { get => newColor; set => newColor = value; }
        public Color NewColorHDR { get => newColorHDR; set => newColorHDR = value; }
        public float NewFloat { get => newFloat; set => newFloat = value; }
        public int NewInt { get => newInt; set => newInt = value; }
        public Texture NewTexture { get => newTexture; set => newTexture = value; }
    }

    public enum MaterialValue
    {
        Float,
        Int,
        Color,
        ColorHDR,
        Texture
    }
}