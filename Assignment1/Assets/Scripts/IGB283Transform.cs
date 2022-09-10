using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IGB283Transform
{
    // Matrix for rotation about the origin
    public static Matrix3x3 Rotate(float angle)
    {
        // Create new matrix
        Matrix3x3 matrix = new Matrix3x3();

        // Set the rows of the matrix
        matrix.SetRow(0, new Vector3(Mathf.Cos(angle), -Mathf.Sin(angle), 0.0f));
        matrix.SetRow(1, new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0.0f));
        matrix.SetRow(2, new Vector3(0.0f, 0.0f, 1.0f));

        return matrix;
    }

    // Matrix for translation
    public static Matrix3x3 Translate(Vector3 offset)
    {
        // Create new matrix
        Matrix3x3 matrix = new Matrix3x3();

        // Set the rows of the matrix
        matrix.SetRow(0, new Vector3(1.0f, 0.0f, offset.x));
        matrix.SetRow(1, new Vector3(0.0f, 1.0f, offset.y));
        matrix.SetRow(2, new Vector3(0.0f, 0.0f, 1.0f));

        return matrix;
    }

    // Matrix for scaling
    public static Matrix3x3 Scale(Vector3 factor)
    {
        // Create new matrix
        Matrix3x3 matrix = new Matrix3x3();

        // Set the rows of the matrix
        matrix.SetRow(0, new Vector3(factor.x, 0.0f, 0.0f));
        matrix.SetRow(1, new Vector3(0.0f, factor.y, 0.0f));
        matrix.SetRow(2, new Vector3(0.0f, 0.0f, 1.0f));

        return matrix;
    }
}
