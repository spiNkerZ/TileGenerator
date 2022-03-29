using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GeneratorManager : MonoBehaviour
{
    public GameObject tilePrefab;

    public ButtonManager uiManager;
    [SerializeField] CalculationValues calculate;
    [SerializeField] Transform tilesCreatedParent;
    [SerializeField] float distByFloor;

    private float squareOneTailValue;

    public List<Transform> tilesArray = new List<Transform>();
    Transform[,] tilesPosArray;

    Vector3 originalForwardTile,originalForwardParent;

     
    public Dictionary<Transform, TextMeshPro> tilesTextDict = new Dictionary<Transform, TextMeshPro>();

    public Dictionary<Transform,float> finishSquareValuesDict = new Dictionary<Transform, float>();
    public HashSet<Transform> tilesInAreaArray = new HashSet<Transform>();

    public List<Transform> tilesCalculatedArrayForInsperctor = new List<Transform>();
    
    [System.Serializable]
    public struct TailAndSqaureS
    {
        public Transform tail;
        public float sqaure;
        public List<Vector3> points;
    }

    public List<TailAndSqaureS> tailAndSqaureSArray = new List<TailAndSqaureS>();
    public Dictionary<Transform,List<Vector3>> pointsCalculateInAreaDict = new Dictionary<Transform, List<Vector3>>();

    private void Start()
    {
        FillFloor();

        originalForwardTile = tilePrefab.transform.forward;
        originalForwardParent = tilesCreatedParent.forward;

        squareOneTailValue = tilePrefab.transform.localScale.x * tilePrefab.transform.localScale.z;
        uiManager.SetSquareOneTileText(squareOneTailValue);
    }

    void FillFloor()
    {
        GenerateTile(uiManager.sliderSeamSize.value, 0, 0);
    }

    public void GenerateTile(float _seamSize, float _offset, float _angle)
    {
        CreateTile(_seamSize);
    }

    void CreateTile(float _seamSize)
    {
        int countTiles = Mathf.FloorToInt(calculate.floorSquare / calculate.tilePrefabSquare);
        int needCountRows = countTiles / 3;
        tilesPosArray = new Transform[needCountRows, needCountRows];

        for (int x = 0; x < needCountRows; x++)
        {
            for (int z = 0; z < needCountRows; z++)
            {
                GameObject spawnTile = Instantiate(tilePrefab, tilePrefab.transform.position, tilePrefab.transform.rotation, tilesCreatedParent);
                tilesArray.Add(spawnTile.transform);
                tilesTextDict.Add(spawnTile.transform, spawnTile.transform.GetChild(0).GetComponent<TextMeshPro>());
                finishSquareValuesDict.Add(spawnTile.transform, 0);
                tilesPosArray[x, z] = spawnTile.transform;
            }
        }

        UpdateTile(_seamSize, 0, 0);
        MakeParamsAfterGeneration();
    }

    public void UpdateTile(float _seamSize, float _offset, float _angle)
    {
        if (_seamSize > 0)
        {
            int countTiles = Mathf.FloorToInt(calculate.floorSquare / calculate.tilePrefabSquare);
            int needCountRows = countTiles / 3;
           
            for (int x = 0; x < needCountRows; x++)
            {
                for (int z = 0; z < needCountRows; z++)
                {
                    tilesPosArray[x, z].rotation = Quaternion.LookRotation(originalForwardTile);
                    tilesPosArray[x, z].position = tilePrefab.transform.position + new Vector3(x * -_seamSize * tilePrefab.transform.localScale.x, distByFloor, z * _seamSize * tilePrefab.transform.localScale.z);
                }
            }
        }

        if(_offset != 0)
        {   
                int countTiles = Mathf.FloorToInt(calculate.floorSquare / calculate.tilePrefabSquare);
                int needCountRows = countTiles / 3;

                for (int x = 0; x < needCountRows; x++)
                {
                if (x % 2 == 0)
                {
                    for (int z = 0; z < needCountRows; z++)
                    {
                        tilesPosArray[x, z].rotation = Quaternion.LookRotation(tilesPosArray[0, 0].forward);                        
                        tilesPosArray[x, z].position =  tilePrefab.transform.position + new Vector3(x * -uiManager.sliderSeamSize.value * tilePrefab.transform.localScale.x, distByFloor, z * uiManager.sliderSeamSize.value * tilePrefab.transform.localScale.z) + tilesPosArray[0, 0].forward * _offset;
                    }
                }
            }
        }

        if(_angle != 0)
        {
            tilesCreatedParent.rotation = Quaternion.Euler(tilesCreatedParent.rotation.x, _angle, tilesCreatedParent.rotation.z);
        }

        MakeParamsAfterGeneration();
    }


    void MakeParamsAfterGeneration()
    {
        int count = calculate.OverlapCheckCountsTiles();
        uiManager.SetCountsUIText(count);
        uiManager.SetCountsCalculateUIText(calculate.tilesCalculatedArray.Count);
        uiManager.SetMainSquare(calculate.CalculateAllSquare());
    } 

    public void SetActiveTMPTails(bool _active)
    {
        foreach (var item in tilesArray)
        {
            tilesTextDict[item].gameObject.SetActive(_active);
        }
    }

    private void Update()
    {
        if (tilesArray.Count > 0)
        {
            for (int i = 0; i < tilesArray.Count; i++)
            {
                calculate.CheckTilesOnInterections(tilesArray[i]);
            }
        }

        if (tilesInAreaArray.Count != tilesCalculatedArrayForInsperctor.Count)
        {
            tilesCalculatedArrayForInsperctor = new List<Transform>();
            tailAndSqaureSArray = new List<TailAndSqaureS>();

            foreach (var item in tilesInAreaArray)
            {
                tilesCalculatedArrayForInsperctor.Add(item);
                tailAndSqaureSArray.Add(new TailAndSqaureS {
                    tail = item,
                    sqaure = finishSquareValuesDict[item],
                    points = pointsCalculateInAreaDict[item]
                });
            }
        }
    }
}
