using Goat.Storage;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "ResourceTileData", menuName = "ScriptableObjects/GlobalVariables/ResourceTileData")]
public class ResourceTileData : Floor
{
    [SerializeField] private Resource resource;
    [SerializeField, InfoBox("Please set this to the initial color of the mesh")] private Color initialColor;
    [SerializeField, ProgressBar(0, 360, ColorGetter = "GetShiftedColor")] private float hueShift;
    [SerializeField, Range(0, 100)] private int chanceToSpawn;

    private Color GetShiftedColor
    {
        get
        {
            float h, s, v;
            Color.RGBToHSV(initialColor, out h, out s, out v);
            float hue = ((h + hueShift) / 360);
            h = (hue < 0) ? hue + 1 : (hue > 1) ? hue - 1 : hue;
            return Color.HSVToRGB(h, s, v);
        }
    }

    public Resource Resource => resource;
    public float HueShift => hueShift;
    public int ChanceToSpawn => chanceToSpawn;
}