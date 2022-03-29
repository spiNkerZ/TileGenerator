using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculationValues : MonoBehaviour
{
    [SerializeField] GeneratorManager generatorManager;

    [SerializeField] Transform floor,wall,overlapDetection;

    [SerializeField] LayerMask tileMask;

    public float floorSquare,tilePrefabSquare;

    public HashSet<Transform> tilesCalculatedArray = new HashSet<Transform>();

    public List<Vector3> pointsArray = new List<Vector3>();

    private void Awake()
    {
        CalculateStartValues();
    }

    void CalculateStartValues()
    {
        floorSquare = floor.localScale.x * floor.localScale.z;
        tilePrefabSquare = generatorManager.tilePrefab.transform.localScale.x * generatorManager.tilePrefab.transform.localScale.z;
    }
    
    public int OverlapCheckCountsTiles()
    {
        return generatorManager.tilesCalculatedArrayForInsperctor.Count;
    }

    public void CheckTilesOnInterections(Transform _tail)
    {
       
        float squareNow = _tail.localScale.x * _tail.localScale.z;
        Vector3[] points = new Vector3[8];
        Vector3[] directions = new Vector3[8];
        bool[] interesctResultArray = new bool[8];
        float[] distanceInteractionsArray = new float[8];
        Ray[] raysArray = new Ray[8];
        Bounds boundsBox = new Bounds(floor.position, new Vector3(floor.localScale.x, wall.localScale.y, floor.localScale.z) * 1.02f);
        List<Vector3> contactsPoints = new List<Vector3>();
        generatorManager.finishSquareValuesDict[_tail] = tilePrefabSquare;

        points[0] = _tail.position + (_tail.forward + -_tail.right) * (_tail.localScale.x / 2);
        points[1] = _tail.position + (_tail.forward + _tail.right) * (_tail.localScale.x / 2);
        points[2] = _tail.position + (_tail.right + -_tail.forward) * (_tail.localScale.x / 2);
        points[3] = _tail.position + (-_tail.right + -_tail.forward) * (_tail.localScale.x / 2);
        points[4] = points[0];
        points[5] = points[1];
        points[6] = points[2];
        points[7] = points[3]; 

        directions[0] = points[1] - points[0];
        directions[1] = points[2] - points[1];
        directions[2] = points[1] - points[2];
        directions[3] = points[0] - points[3];
        directions[4] = points[3] - points[0];
        directions[5] = points[0] - points[1];
        directions[6] = points[1] - points[2];
        directions[7] = points[2] - points[3];

        for (int i = 0; i < raysArray.Length; i++)
        {
            raysArray[i] = new Ray(points[i], directions[i]);
            interesctResultArray[i] = boundsBox.IntersectRay(raysArray[i], out distanceInteractionsArray[i]);
        }
        int countPointsContains = 0;
        for (int i = 0; i < points.Length; i++)
        {
            if (boundsBox.Contains(points[i]))
            {
                countPointsContains++;
                if (countPointsContains >= 2)
                {
                    generatorManager.tilesInAreaArray.Add(_tail);
                    if (!generatorManager.pointsCalculateInAreaDict.ContainsKey(_tail))
                    {
                        generatorManager.pointsCalculateInAreaDict.Add(_tail, new List<Vector3>());
                    }
                    break;
                }
            }
            else
            {
                if (i == points.Length - 1)
                {
                    if (generatorManager.tilesInAreaArray.Contains(_tail))
                    {
                        generatorManager.tilesInAreaArray.Remove(_tail);
                        generatorManager.pointsCalculateInAreaDict.Remove(_tail);
                    }
                }
            }
        }
        for (int i = 0; i < interesctResultArray.Length; i++)
        {
            if (distanceInteractionsArray[i] > 0 && interesctResultArray[i] && distanceInteractionsArray[i] < _tail.localScale.x)
            {
                tilesCalculatedArray.Add(_tail);
                pointsArray.Add(raysArray[i].origin + raysArray[i].direction * distanceInteractionsArray[i]);

                for (int c = 0; c < points.Length; c++)
                {
                    if (boundsBox.Contains(points[c]))
                    {
                        contactsPoints.Add(points[c]);
                    }
                }
                if (pointsArray.Count >= 2)
                {
                    if (generatorManager.uiManager.toggleGIZMO.isOn)
                    {
                        Debug.DrawLine(pointsArray[0], pointsArray[1], Color.blue, 1);
                    }
                    if (contactsPoints.Count >= 2)
                    {
                        Vector3 point1 = contactsPoints[0];
                        Vector3 point2 = contactsPoints[1];
                        Vector3 point3 = pointsArray[0];
                        Vector3 point4 = pointsArray[1];

                        List<Vector3> pointsToCalculate = new List<Vector3>();
                        
                        pointsToCalculate.Add(point1);
                        pointsToCalculate.Add(point2);
                        pointsToCalculate.Add(point3);
                        pointsToCalculate.Add(point4);

                        pointsToCalculate.Sort(new PointSorting(new Vector2(_tail.position.x, _tail.position.z)));

                        generatorManager.pointsCalculateInAreaDict[_tail] = pointsToCalculate;

                        if (generatorManager.uiManager.toggleGIZMO.isOn)
                        {
                            DebugExtension.DebugWireSphere(pointsToCalculate[0], Color.yellow, 0.25f, 1);
                            DebugExtension.DebugWireSphere(pointsToCalculate[1], Color.red, 0.25f, 1);
                            DebugExtension.DebugWireSphere(pointsToCalculate[2], Color.magenta, 0.25f, 1);
                            DebugExtension.DebugWireSphere(pointsToCalculate[3], Color.cyan, 0.25f, 1);
                        }

                        squareNow = CalculateSquarePoints(pointsToCalculate);

                    }
                }

                generatorManager.finishSquareValuesDict[_tail] = squareNow;

                if (generatorManager.uiManager.toggleGIZMO.isOn)
                {
                    DebugExtension.DebugBounds(boundsBox, Color.yellow, 1);
                    Debug.DrawRay(raysArray[i].origin, raysArray[i].direction * _tail.localScale.x, Color.green, 1);
                }
            }
            else
            {
                if (tilesCalculatedArray.Contains(_tail))
                {
                    tilesCalculatedArray.Remove(_tail);
                }
            }

        }

        generatorManager.tilesTextDict[_tail].text = squareNow.ToString("#.##");
        pointsArray = new List<Vector3>();
     
    }
    public float CalculateSquarePoints(List<Vector3> pointsArray)
    {
        var count = pointsArray.Count;
        if (count < 3)
            return 0f;

        var firstVertex = pointsArray[0];
        var lastIndex = count - 1;
        var lastVertex = pointsArray[lastIndex];
        var area = lastVertex.x * firstVertex.z - firstVertex.x * lastVertex.z;
        for (var i = 0; i < lastIndex; i++)
        {
            var currentVertex = pointsArray[i];
            var nextVertex = pointsArray[i + 1];
            area += currentVertex.x * nextVertex.z - nextVertex.x * currentVertex.z;
        }
        return Mathf.Abs(area * 0.5f);
    }
    public float CalculateAllSquare()
    {
        float amount = 0;

        foreach (var item in generatorManager.tilesCalculatedArrayForInsperctor)
        {
            amount += generatorManager.finishSquareValuesDict[item];
        }

        return amount;
    }


}
