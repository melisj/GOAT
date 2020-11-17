using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float yaw;
    [SerializeField] private float pitch;
    [SerializeField] private float rotSmoothTime;
    private Vector3 currentRotation;
    [SerializeField] private float sensitivity;
    [SerializeField] private Vector2 pitchMinMax;
    [SerializeField] private Transform target;
    [SerializeField] private float cameraMoveSpeed;
    [SerializeField] private Vector3 offSet;
    [SerializeField] private float distanceFromTarget;
    private Vector3 rotSmoothVel;

    private void LateUpdate()
    {
        RotateCamera();
        MoveCamera();
    }

    private void RotateCamera()
    {
        float inputX = Input.GetAxis("Mouse X");
        float inputY = Input.GetAxis("Mouse Y");

        yaw += inputX * sensitivity;
        pitch -= inputY * sensitivity;

        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

        //Quaternion localRot = Quaternion.Euler(pitch, yaw, 0);
        //transform.rotation = localRot;

        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotSmoothVel, rotSmoothTime);
        transform.eulerAngles = currentRotation;
    }

    private void MoveCamera()
    {
        //float step = cameraMoveSpeed * Time.deltaTime;
        //transform.position = Vector3.MoveTowards(transform.position, target.position + offSet, step);
        transform.position = target.position - transform.forward * distanceFromTarget;
        //offSet = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * sensitivity, Vector3.up) * offSet;
        //transform.position = target.position + offSet;
        //transform.LookAt(target.position + offSet);
    }
}