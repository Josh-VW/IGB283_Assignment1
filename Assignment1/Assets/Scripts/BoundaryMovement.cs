using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryMovement : MonoBehaviour
{
     // The code for this script is modified code from IGB283 workshop 5

    private Vector3 position;
    public bool beingDragged = false;
    public float speed = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        MouseClickAction();
        Move();
    }

    // Move the boundary objects
    void Move () 
    {
        // Find the mouse position in word space
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (beingDragged)
        {       
            position.y = mousePosition.y;
            transform.position = position;
        }            
    }

    //Check if boundary objects are being selected
    void MouseOverAction ()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition);
        if (hitCollider && hitCollider.transform.tag =="Boundary")
        {
            hitCollider.transform.gameObject.GetComponent<BoundaryMovement>().beingDragged = true;
        }

    }

    //Check if mouse button is held
    void MouseClickAction ()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            MouseOverAction();        
        } 
        else if (Input.GetMouseButtonUp(0)) 
        {
            beingDragged = false;
        }
    }

}
