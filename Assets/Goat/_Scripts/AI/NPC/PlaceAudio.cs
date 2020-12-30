using System;
using UnityEngine;

namespace Goat.AI
{
    public class PlaceAudio : AudioCue
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
}