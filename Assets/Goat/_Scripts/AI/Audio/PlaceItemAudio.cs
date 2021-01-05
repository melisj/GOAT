using System;
using UnityEngine;
using Goat.AI;

    public class PlaceItemAudio : AudioCue
    {
        [SerializeField] Worker worker;

        private void OnEnable()
        {
            worker.placeItem.eventHandler += Worker_onPlaceItem;
        }

        private void OnDisable()
        {
            worker.placeItem.eventHandler -= Worker_onPlaceItem;
        }

        private void Worker_onPlaceItem(object sender, EventArgs e)
        {
            PlayAudioCue();
        }
    }
