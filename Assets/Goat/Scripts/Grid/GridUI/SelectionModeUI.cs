using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GOAT.Grid
{
    public class SelectionModeUI : BasicGridUIElement
    {
        [SerializeField] private GameObject tileInfoPanel;

        [SerializeField] private Text tileBuildingTypeText;
        [SerializeField] private Text tileFloorTypeText;
        [SerializeField] private Text tilePositionText;

        public void SetTileInfo(TileInformation info) {
            tileBuildingTypeText.text = info.buildingType.ToString();
            tileFloorTypeText.text = info.floorType.ToString();

            tilePositionText.text = info.TilePosition.ToString();
        }
    }
}
