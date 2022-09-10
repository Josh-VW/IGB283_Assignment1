using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class SpikeDisc : MonoBehaviour
{
    public Material material;

    // Inital transform properties
    private Vector3 position = new Vector3(0.0f, 0.0f, 1.0f);
    public Vector3 scale = new Vector3(1.0f, 1.0f, 1.0f);
    public float rotation = 0.0f;

    // Bouncing properties
    public GameObject startObject;
    public GameObject endObject;
    private Vector3 startPoint = new Vector3(5.0f, 5.0f, 1.0f);
    private Vector3 endPoint = new Vector3(-5.0f, -5.0f, 1.0f);
    public float speed = 5.0f;

    // Define object vertices
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
    // Define object vertex colours
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
    // Define triangle vertices
    private int[] triangleVertices = new int[] {
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

        // Set the mesh vertices
        mesh.vertices = objectVertices;

        // Set the colour of each mesh vertex
        mesh.colors = vertexColours;

        // Set triangle indicies
        mesh.triangles = triangleVertices;

        // Recalculate the bounding volume
        mesh.RecalculateBounds();

        // Get the initial 'bounce' objects and bounds
        startPoint = startObject.transform.position;
        endPoint = endObject.transform.position;
        position = startPoint;

        // Get the initial transformation matrix
        Matrix3x3 S = IGB283Transform.Scale(scale);
        Matrix3x3 R = IGB283Transform.Rotate(rotation);
        Matrix3x3 T = IGB283Transform.Translate(position);
        Matrix3x3 transformMatrix = T * R * S;
        
        // Apply the initial transform
        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = transformMatrix.MultiplyPoint(vertices[i]);
        }

        // Set the mesh vertices
        mesh.vertices = vertices;

        // Recalculate the bounding volume
        mesh.RecalculateBounds();
    }

    // Update is called once per frame
    void Update()
    {
        //// Get the vertices from the mesh
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;

        // Update 'bounce' bounds
        startPoint = startObject.transform.position;
        endPoint = endObject.transform.position;

        // Calculate next position
        Vector3 destinationVector = endPoint - position;
        Vector3 directionVector = destinationVector / destinationVector.magnitude;
        Vector3 deltaPosition = directionVector * speed * Time.deltaTime;
        if (destinationVector.magnitude <= deltaPosition.magnitude)
        {
            deltaPosition = destinationVector;

            // Reverse direction ('Bounce')
            (startObject, endObject) = (endObject, startObject);
        }

        // Get the transformation matrix
        Matrix3x3 T = IGB283Transform.Translate(deltaPosition);
        Matrix3x3 RT = IGB283Transform.Translate(position);
        Matrix3x3 R = IGB283Transform.Rotate(rotation * Time.deltaTime);
        Matrix3x3 RT2 = IGB283Transform.Translate(-position);
        Matrix3x3 transformMatrix = T * RT * R * RT2;

        // Apply the transform
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = transformMatrix.MultiplyPoint(vertices[i]);
        }
        position += deltaPosition;

        // Set the mesh vertices
        mesh.vertices = vertices;
    }
}
