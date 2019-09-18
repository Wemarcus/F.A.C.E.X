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
    public float rotationY = 0F;
    public float rotationX = 0f;

    public bool rotation = true;

    public GameObject final_mask;

    private void Start()
    {
        Cursor.visible = false;
    }

    void Update()
    {
        if (!final_mask.activeSelf)
        {
            if (rotation)
            {
                rotationX += Input.GetAxis("Mouse X") * sensitivityX;
                rotationX = Mathf.Clamp(rotationX, minimumX, maximumX);
                rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
                transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);

                if (Input.GetAxis("Mouse ScrollWheel") > 0f && GetComponent<Camera>().fieldOfView >= 20) // forward
                {
                    GetComponent<Camera>().fieldOfView -= 4;
                }
                else if (Input.GetAxis("Mouse ScrollWheel") < 0f && GetComponent<Camera>().fieldOfView <= 56) // backwards
                {
                    GetComponent<Camera>().fieldOfView += 4;
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                rotation = !rotation;
                Cursor.visible = !Cursor.visible;
            }
        }
    }
}
