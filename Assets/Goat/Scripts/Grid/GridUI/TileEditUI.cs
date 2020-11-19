using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAT.Grid
{
    public class TileEditUI : BasicGridUIElement
    {
        private Tile latestSelectedTile;
        [SerializeField] private GameObject tileEditPanel;
        
        public void SetSelectedTile(Tile selectedTile) {
            latestSelectedTile = selectedTile;
        }

        public void OnClickFloor(int type) { 
            latestSelectedTile?.EditFloor((FloorType)type);
            //GridUIManager.HideUI();
        }

        public void OnClickBuilding(int type) {
            latestSelectedTile?.EditBuilding((BuildingType)type);
           // GridUIManager.HideUI();
        }

    }
}
