using DG.Tweening;
using Goat.CameraControls;
using UnityEngine;

namespace Goat.Farming
{
    public class ResourcePackMover : MonoBehaviour
    {
        [SerializeField] private Vector3[] tubePoints;
        [SerializeField] private float durationPerTube;

        public void Setup(Vector3[] wayPoints)
        {
            tubePoints = wayPoints;
            transform.DOPath(tubePoints, durationPerTube * tubePoints.Length);
        }
    }
}