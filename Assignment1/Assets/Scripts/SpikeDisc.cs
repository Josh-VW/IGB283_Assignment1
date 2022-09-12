using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class SpikeDisc : MonoBehaviour
{
    public Material material;

    // Transform properties
    public Vector3 position;
    public Vector3 scale;
    public Vector3 scaleBound1 = new Vector3(1.0f, 1.0f, 1.0f);
    public Vector3 scaleBound2 = new Vector3(0.5f, 2.0f, 1.0f);
    public float rotation = 0.0f;
    public float rotationSpeed = 0.0f;

    // Bounding properties
    public GameObject boundingObjectA;
    public GameObject boundingObjectB;
    private Vector3 pointA;
    private Vector3 pointB;
    public float speed = 5.0f;
    private bool directionForward = true;

    // Colour properties
    public Color colour1 = Color.black;
    public Color colour2 = Color.white;

    // Default object properties
    private Vector3[] objectVertices = new Vector3[] {
            new Vector3(0.0f, 0.0f, 0.0f),   // 1
            new Vector3(0.0f, 0.7f, 0.0f),   // 2
            new Vector3(0.3f, 0.6f, 0.0f),   // 3
            new Vector3(0.8f, 0.8f, 0.0f),   // 4
            new Vector3(0.6f, 0.3f, 0.0f),   // 5
            new Vector3(0.7f, 0.0f, 0.0f),   // 6
            new Vector3(0.6f, -0.3f, 0.0f),  // 7
            new Vector3(0.8f, -0.8f, 0.0f),  // 8
            new Vector3(0.3f, -0.6f, 0.0f),  // 9
            new Vector3(0.0f, -0.7f, 0.0f),  // 10
            new Vector3(-0.3f, -0.6f, 0.0f), // 11
            new Vector3(-0.8f, -0.8f, 0.0f), // 12
            new Vector3(-0.6f, -0.3f, 0.0f), // 13
            new Vector3(-0.7f, 0.0f, 0.0f),  // 14
            new Vector3(-0.6f, 0.3f, 0.0f),  // 15
            new Vector3(-0.8f, 0.8f, 0.0f),  // 16
            new Vector3(-0.3f, 0.6f, 0.0f)   // 17
    };
    private Color[] vertexColours = new Color[] {
        Color.gray, // 1
        Color.gray, // 2
        Color.gray, // 3
        Color.red,  // 4
        Color.gray, // 5
        Color.gray, // 6
        Color.gray, // 7
        Color.blue,  // 8
        Color.gray, // 9
        Color.gray, // 10
        Color.gray, // 11
        Color.yellow,  // 12
        Color.gray, // 13
        Color.gray, // 14
        Color.gray, // 15
        Color.green,  // 16
        Color.gray  // 17
    };
    private int[] triangleIndices = new int[] {
            0, 1, 2,    // 1
            0, 2, 4,    // 2
            2, 3, 4,    // 3
            0, 4, 5,    // 4
            0, 5, 6,    // 5
            0, 6, 8,    // 6
            6, 7, 8,    // 7
            0, 8, 9,    // 8
            0, 9, 10,   // 9
            0, 10, 12,  // 10
            10, 11, 12, // 11
            0, 12, 13,  // 12
            0, 13, 14,  // 13
            0, 14, 16,  // 14
            14, 15, 16, // 15
            0, 16, 1    // 16
    };

    // Start is called before the first frame update
    void Start()
    {
        // Add a MeshFilter and MeshRenderer to the Empty GameObject
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();

        // Get the Mesh from the MeshFilter
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        // Set the material to the material we have selected
        GetComponent<MeshRenderer>().material = material;

        // Clear all vertex and index data from the mesh
        mesh.Clear();

        // Get the initial position and bounding points
        GetBounds();
        position = (directionForward ? pointA : pointB);

        // Get the initial scale
        scale = (directionForward ? scaleBound1 : scaleBound2);

        // Calculate the initial transformation matrix
        Matrix3x3 S = IGB283Transform.Scale(scale);
        Matrix3x3 R = IGB283Transform.Rotate(rotation);
        Matrix3x3 T = IGB283Transform.Translate(position);
        Matrix3x3 transformMatrix = T * R * S; // Order of operations: right -> left

        // Get the initial colour
        Color colour = (directionForward ? colour1 : colour2);

        // Apply the initial transform and colour
        Vector3[] vertices = objectVertices;
        Color[] colours = vertexColours;
        for (int i = 0; i < vertices.Length; i++)
        {
            // Transform
            vertices[i] = transformMatrix.MultiplyPoint(vertices[i]);

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
    }

    // Update is called once per frame
    void Update()
    {
        //// Get the vertices and colours from the mesh
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        Color[] colours = mesh.colors;

        // Update bounding points
        GetBounds();

        #region Calculate Translation

        // Calculate next position
        Vector3 destinationPoint = (directionForward ? pointB : pointA);
        Vector3 destinationVector = destinationPoint - position;
        Vector3 directionVector = destinationVector / destinationVector.magnitude;
        Vector3 deltaPosition = directionVector * speed * Time.deltaTime;
        if (destinationVector.magnitude <= deltaPosition.magnitude)
        {
            // Move to destination
            deltaPosition = destinationVector;

            // Reverse direction ('Bounce')
            directionForward = !directionForward;
        }

        // Calculate translation matrices
        Matrix3x3 TI = IGB283Transform.Translate(-position); // Translate to origin
        position += deltaPosition;
        Matrix3x3 T = IGB283Transform.Translate(position); // Translate to next position

        #endregion

        // Calculate the path position quotient from point A to B
        Vector3 pathVector = pointB - pointA;
        Vector3 pathPositionVector = position - pointA;
        float pathQuotient = pathPositionVector.magnitude / pathVector.magnitude;

        #region Calculate Scale

        // Calculate the scale factor to next scale
        Vector3 nextScale = Vector3.Lerp(scaleBound1, scaleBound2, pathQuotient);
        if (nextScale[0] == 0 || nextScale[1] == 0) scale = nextScale; // Prevent divide by zero operation (nullifies this operation for this frame)
        Vector3 scaleFactor = new Vector3(nextScale[0] / scale[0], nextScale[1] / scale[1], 1);

        // Calculate scale matrix
        Matrix3x3 S = IGB283Transform.Scale(scaleFactor);
        scale = nextScale;

        #endregion

        #region Calculate Rotation

        // Calculate next rotation
        float deltaRotation = rotationSpeed * Time.deltaTime;

        // Calculate rotation matrices
        Matrix3x3 RI = IGB283Transform.Rotate(-rotation); // Rotate to initial rotation
        rotation += deltaRotation;
        rotation %= 2 * Mathf.PI; // From last revolution
        Matrix3x3 R = IGB283Transform.Rotate(rotation); // Rotate to next rotation

        #endregion

        // Calculate the transformation matrix
        Matrix3x3 transformMatrix = T * R * S * RI * TI; // Order of operations: right -> left

        // Calculate the colour
        Color colour = Color.Lerp(colour1, colour2, pathQuotient);

        // Apply the transform and colour
        for (int i = 0; i < vertices.Length; i++)
        {
            // Transform
            vertices[i] = transformMatrix.MultiplyPoint(vertices[i]);

            // Colour
            colours[i] = colour;
        }

        // Set the mesh vertices and colour
        mesh.vertices = vertices;
        mesh.colors = colours;
    }

    // Get bounding points from bounding objects
    private void GetBounds()
    {
        pointA = boundingObjectA.transform.position;
        pointB = boundingObjectB.transform.position;
    }
}
