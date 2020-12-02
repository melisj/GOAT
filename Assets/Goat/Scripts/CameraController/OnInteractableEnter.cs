using Goat;
using Goat.Grid.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnInteractableEnter : MonoBehaviour
{
    [SerializeField] private CollisionDetection collisionDetection;
    [SerializeField] private GridUIElement uiToShow;

    private void Awake()
    {
    }

    private void CollisionDetection_OnColliderEnter(object sender, Collider e)
    {
        if (e.gameObject == this.gameObject)
        {
            //DO SOMETHING
            //Probably show UI
            GridUIManager.Instance.ShowNewUI(uiToShow);
        }
    }
}