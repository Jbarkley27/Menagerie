using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementModule : MonoBehaviour
{

    public Camera mainCamera;
    public Rigidbody rb;
    public float moveSpeed;
    public LayerMask mouseLayerMask;
    public Transform debugPoint;
    public Transform rotationRott;
    public float lookSpeed;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
    }

    public void Transfer(Vector3 position)
    {
        rb.MovePosition(position);
        //gameObject.transform.position = position;
    }

    private void Update()
    {
        // Cast a ray from the mouse position
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mouseLayerMask))
        {
            // Get the point where the ray hits an object
            Vector3 targetPosition = hit.point;

            //targetPosition.y = 1f;

            debugPoint.position = targetPosition;

            // Calculate direction to look at the target position
            Vector3 lookDirection = targetPosition - rotationRott.position;

            var targetRotation = Quaternion.LookRotation(lookDirection);

            // Smoothly rotate towards the target point.
            rotationRott.rotation = Quaternion.Slerp(rotationRott.rotation, targetRotation, lookSpeed * Time.deltaTime);

            //rotationRott.rotation = Quaternion.Lerp(rotationRott.rotation, Quaternion.Euler(lookDirection), Time.deltaTime * lookSpeed);

            //// Ensure the direction isn't zero (to avoid errors)
            //if (lookDirection != Vector3.zero)
            //{
            //    // Create the rotation to look at the target position
            //    Quaternion lookRotation = Quaternion.LookRotation(lookDirection);

            //    // Rotate the rigidbody to face the target direction
            //    rb.MoveRotation(lookRotation);
            //}
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
    }


    public void Move()
    {
        Vector3 moveVector = new Vector3(InputManager.MoveInput.x, 0, InputManager.MoveInput.y);
        rb.AddForce(moveVector * moveSpeed * Time.deltaTime, ForceMode.VelocityChange);
    }
}
