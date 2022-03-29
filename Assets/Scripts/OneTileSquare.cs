using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OneTileSquare : MonoBehaviour
{
    [SerializeField] GameObject tilePrefab;
    [SerializeField] TextMeshPro textMesh;

    List<Vector3> pointsArray = new List<Vector3>();

    private void Awake()
    {
        FillPointsArray(tilePrefab.transform);
    }

    private void Update()
    {
        CalculateSquare();
    }

    void FillPointsArray(Transform _tail)
    {
        pointsArray.Add(_tail.position + (_tail.forward + -_tail.right) * (_tail.localScale.x / 2));
        pointsArray.Add(_tail.position + (_tail.forward + _tail.right) * (_tail.localScale.x / 2));
        pointsArray.Add(_tail.position + (_tail.right + -_tail.forward) * (_tail.localScale.x / 2));
        pointsArray.Add(_tail.position + (-_tail.right + -_tail.forward) * (_tail.localScale.x / 2));
    }

    void CalculateSquare()
    {
        for (int i = 0; i < pointsArray.Count; i++)
        {
            DebugExtension.DebugWireSphere(pointsArray[i], Color.green, 0.3f, 1);
            Debug.Log("Points ID " + i + " POS " + pointsArray[i]);
        }

       Debug.Log(Vector3.Distance(pointsArray[0], pointsArray[1]));
        textMesh.text = CalculateSquarePoints(pointsArray).ToString("#.##");
    }

    public float CalculateSquarePoints(List<Vector3> vertices)
    {
        var count = vertices.Count;
        if (count < 3)
            return 0f;

        var firstVertex = vertices[0];
        var lastIndex = count - 1;
        var lastVertex = vertices[lastIndex];
        var area = lastVertex.x * firstVertex.z - firstVertex.x * lastVertex.z;
        for (var i = 0; i < lastIndex; i++)
        {
            var currentVertex = vertices[i];
            var nextVertex = vertices[i + 1];
            area += currentVertex.x * nextVertex.z - nextVertex.x * currentVertex.z;
        }

        return Mathf.Abs(area * 0.5f);
    }

}
