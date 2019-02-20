using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookAround : MonoBehaviour
{
    public float sensitivityX = 15F;
    public float sensitivityY = 15F;
    public float minimumX = -90F;
    public float maximumX = 90F;
    public float minimumY = -60F;
    public float maximumY = 60F;
    float rotationY = 0F;
    float rotationX = 0f;

    private void Start()
    {
        Cursor.visible = false;
    }

    void Update()
    {
        rotationX += Input.GetAxis("Mouse X") * sensitivityX;
        rotationX = Mathf.Clamp(rotationX, minimumX, maximumX);
        rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
        rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
        transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
    }
}
