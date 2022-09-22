using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class BoundaryMovement : MonoBehaviour
{
    public Material material;

    // The code for this script is modified code from IGB283 workshop 5
    private bool beingDragged = false;

    // Transform properties
    public Vector3 position = new Vector3(0.0f, 0.0f, 0.0f);
    public Vector3 scale = new Vector3(1.0f, 1.0f, 1.0f);
    public float rotation = 0.0f;

    // Default mesh properties
    private Vector3[] objectVertices = new Vector3[] {
            new Vector3(-0.5f, -0.5f, 0.0f),   // 1
            new Vector3(0.5f, -0.5f, 0.0f),   // 2
            new Vector3(0.5f, 0.5f, 0.0f),   // 3
            new Vector3(-0.5f, 0.5f, 0.0f)   // 4
    };
    private Color[] vertexColours = new Color[] {
        Color.white, // 1
        Color.black, // 2
        Color.white, // 3
        Color.black,  // 4
    };
    private int[] triangleIndices = new int[] {
            0, 1, 2,    // 1
            0, 2, 3    // 2
    };

    // Collider properties
    private Vector2[] vertices2D;

    // Colour Properties
    public Color colour = Color.white;

    // Start is called before the first frame update
    void Start()
    {
        // Add a MeshFilter and MeshRenderer to the Empty GameObject
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();

        // Add Collider
        gameObject.AddComponent<PolygonCollider2D>();

        // Get the Mesh from the MeshFilter
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        // Set the material to the material we have selected
        GetComponent<MeshRenderer>().material = material;

        // Clear all vertex and index data from the mesh
        mesh.Clear();

        // Calculate the initial transformation matrix
        Matrix3x3 S = IGB283Transform.Scale(scale);
        Matrix3x3 R = IGB283Transform.Rotate(rotation);
        Matrix3x3 T = IGB283Transform.Translate(position);
        Matrix3x3 transformMatrix = T * R * S; // Order of operations: right -> left

        // Apply the initial transform and colour
        Vector3[] vertices = objectVertices;
        Color[] colours = vertexColours;
        vertices2D = new Vector2[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            // Transform
            vertices[i] = transformMatrix.MultiplyPoint(vertices[i]);

            // Transform for collider
            vertices2D[i] = new Vector2(vertices[i].x, vertices[i].y);

            // Colour
            colours[i] = colour;
        }

        // Set the mesh vertices and colours
        mesh.vertices = vertices;
        mesh.colors = colours;

        // Set triangle indicies
        mesh.triangles = triangleIndices;

        // Recalculate the bounding volume
        mesh.RecalculateBounds();

        // Set Collider
        GetComponent<PolygonCollider2D>().points = null;
        GetComponent<PolygonCollider2D>().points = vertices2D;
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
            //// Get the vertices and colours from the mesh
            Mesh mesh = GetComponent<MeshFilter>().mesh;
            Vector3[] vertices = mesh.vertices;
            Color[] colours = mesh.colors;

            #region Calculate Translation

            // Calculate next position
            Vector3 nextPosition = position;
            nextPosition.y = mousePosition.y;
            Vector3 deltaPosition = nextPosition - position;

            // Calculate translation matrices
            Matrix3x3 T = IGB283Transform.Translate(deltaPosition); // Translate to next position
            position = nextPosition;

            #endregion

            // Calculate the transformation matrix
            Matrix3x3 transformMatrix = T; // Order of operations: right -> left

            // Apply the transform and colour
            for (int i = 0; i < vertices.Length; i++)
            {
                // Transform
                vertices[i] = transformMatrix.MultiplyPoint(vertices[i]);
                
                // Transform for collider
                vertices2D[i] = new Vector2(vertices[i].x, vertices[i].y);
            }

            // Set the mesh vertices and colour
            mesh.vertices = vertices;
            mesh.colors = colours;

            // Set Collider
            GetComponent<PolygonCollider2D>().points = null;
            GetComponent<PolygonCollider2D>().points = vertices2D;
        }            
    }

    //Check if boundary objects are being selected
    void MouseOverAction ()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition);
        if (hitCollider && hitCollider.transform.CompareTag("Boundary"))
        {
            hitCollider.GetComponent<BoundaryMovement>().beingDragged = true;
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
