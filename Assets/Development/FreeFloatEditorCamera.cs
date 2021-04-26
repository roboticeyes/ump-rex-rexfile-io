/*
 * Copyright (c) 2020 Robotic Eyes GmbH software
 *
 * THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
 * PARTICULAR PURPOSE.
 *
 */

using UnityEngine;

public class FreeFloatEditorCamera : MonoBehaviour
{
    [SerializeField] 
    private float movementSpeed = 2f;
    [SerializeField] 
    private Camera playerCamera;
    [SerializeField] 
    private bool lockYMovement = true;

    private void Awake()
    {
#if !UNITY_EDITOR
        Destroy (this);
#endif
        // playerCamera.transform.position = new Vector3 (0f, 1f, 0f);
    }

    void Update()
    {
#if !UNITY_EDITOR
        return;
#endif
        if (Input.GetKey (KeyCode.LeftAlt) || Input.GetKey (KeyCode.LeftControl) || Input.GetKey (KeyCode.LeftShift) || Input.GetMouseButton (0))
        {
            return;
        }

        movementSpeed = Mathf.Max (movementSpeed += 0.2f * Input.GetAxis ("Mouse ScrollWheel"), 0.2f);

        var mouseX = Input.GetAxis ("Mouse X");
        var mouseY = -1f * Input.GetAxis ("Mouse Y");

        playerCamera.transform.eulerAngles += new Vector3 (mouseY, mouseX * 2, 0f);

        var movement = playerCamera.transform.forward * Input.GetAxis ("Vertical") + playerCamera.transform.right * Input.GetAxis ("Horizontal");

        if (lockYMovement)
        {
            movement.y = 0f;
        }

        movement.Normalize ();
        movement *= movementSpeed * Time.deltaTime * 3f;

        playerCamera.transform.position += movement;
    }
}