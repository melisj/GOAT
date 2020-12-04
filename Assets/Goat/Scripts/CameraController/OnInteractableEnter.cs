using Goat;
using Goat.Grid.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Goat.Grid.UI.GridUIInfo;

public class OnInteractableEnter : MonoBehaviour
{
    [SerializeField] private CollisionDetection collisionDetection;
    [SerializeField] private GridUIElement uiToShow;
    [SerializeField] private GridUIInfo gridUIInfo;

    private void Awake()
    {
    }

    private void CollisionDetection_OnColliderEnter(object sender, Collider e)
    {
        if (e.gameObject == this.gameObject)
        {
            //DO SOMETHING
            //Probably show UI
            gridUIInfo.CurrentUIElement = GridUIElement.Building;
        }
    }
}