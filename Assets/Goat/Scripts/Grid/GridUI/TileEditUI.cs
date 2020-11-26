using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goat.Grid.UI
{
    public class TileEditUI : BasicGridUIElement
    {
        private Tile latestSelectedTile;
        [SerializeField] private GameObject tileEditPanel;
        
        public void SetSelectedTile(Tile selectedTile) {
            latestSelectedTile = selectedTile;
        }

        public void OnClickFloor(int type) { 
            latestSelectedTile?.EditFloor((FloorType)type, 0);
            //GridUIManager.HideUI();
        }

        public void OnClickBuilding(int type) {
            latestSelectedTile?.EditBuilding((BuildingType)type, 0);
           // GridUIManager.HideUI();
        }

    }
}
