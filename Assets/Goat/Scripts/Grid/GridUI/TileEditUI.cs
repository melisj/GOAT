using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAT.Grid
{
    public class TileEditUI : MonoBehaviour
    {
        private Tile latestSelectedTile; 
        
        public void ShowUI(Tile selectedTile) {
            latestSelectedTile = selectedTile; 
            gameObject.SetActive(true); 
        }

        public void HideUI() { 
            gameObject.SetActive(false); 
        }

        public void OnClickFloor(FloorType type) { 
            latestSelectedTile?.EditFloor(type); 
        }

        public void OnClickBuilding(BuildingType type) {
            latestSelectedTile?.EditBuilding(type); 
        }

    }
}
