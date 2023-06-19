using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerGerakanMouse))]
public class Controller : MonoBehaviour
{
    Camera kamera;
    PlayerGerakanMouse motor;
    // Start is called before the first frame update
    void Start()
    {
        kamera = Camera.main;
        motor = GetComponent<PlayerGerakanMouse>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = kamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                /* Debug.Log("Ini adalah "  + hit.collider.name + " " + hit.point);  */
                motor.GerakUntukPoint(hit.point);
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = kamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                /* Debug.Log("Ini adalah "  + hit.collider.name + " " + hit.point);  */
                motor.GerakUntukPoint(hit.point);
            }
        }
    }
}
