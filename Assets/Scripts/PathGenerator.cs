using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class PathGenerator : MonoBehaviour
{
    public GameObject spawnableObject;
    public GameObject start;
    public GameObject end;

    public Vector3 startPosition;
    public Vector3 endPosition;

    public float spawnRadiusX = 100;
    public float spawnRadiusZ = 100;
    public uint spawnLimit = 3;
    public float distancing = 80f;

    private List<GameObject> _spawnedObjects;
    private List<Vector3> _spawnedObjectsPositions;

    private Random _rand;

    private float _minX;
    private float _maxX;
    private float _minZ;
    private float _maxZ;

    void UpdateArea()
    {
        var p = transform.position;

        _minX = p.x - spawnRadiusX;
        _maxX = p.x + spawnRadiusX;
        _minZ = p.y - spawnRadiusZ;
        _maxZ = p.y + spawnRadiusZ;
    }

    void InitObjects()
    {
        _spawnedObjects = new List<GameObject>();
        _spawnedObjectsPositions = new List<Vector3>();
        _spawnedObjectsPositions.Add(startPosition);
        _spawnedObjectsPositions.Add(endPosition);

        UpdateArea();
    }

    void DestructPoints()
    {
        _spawnedObjects.ForEach(Destroy);
        InitObjects();
    }

    void GeneratePoint()
    {
        UpdateArea();

        var newPosition = new Vector3(
            _rand.NextFloat(_minX, _maxX),
            transform.position.y,
            _rand.NextFloat(_minZ, _maxZ)
        );

        if (_spawnedObjectsPositions.Any(pos => Vector3.Distance(pos, newPosition) < distancing))
            return;

        _spawnedObjects.Add(Instantiate(spawnableObject, newPosition, transform.rotation));
        _spawnedObjectsPositions.Add(newPosition);
    }

    void GeneratePoints()
    {
        DestructPoints();

        uint maxAttempts = spawnLimit * 10;
        var attempts = 0;

        while (_spawnedObjects.Count < spawnLimit)
        {
            attempts++;
            if (attempts >= maxAttempts)
            {
                Debug.Log("Attempts exhausted");
                break;
            }

            GeneratePoint();
        }
    }

    GameObject[] GetClosestPath()
    {
        // https://stackoverflow.com/a/71131746
        return _spawnedObjects.Where(n => n)
            .OrderBy(n => (n.transform.position - start.transform.position).sqrMagnitude)
            .ToArray();
    }

    void DrawClosestPath()
    {
        var path = GetClosestPath();

        var first = true;
        GameObject prev = null;
        foreach (var o in path)
        {
            if (first)
            {
                first = false;
                Debug.DrawLine(start.transform.position, o.transform.position, Color.green);
            }
            else
            {
                Debug.DrawLine(prev.transform.position, o.transform.position, Color.green);
            }
            prev = o;
        }

        if (prev != null) Debug.DrawLine(prev.transform.position, end.transform.position, Color.green);
    }


    // Start is called before the first frame update
    void Start()
    {
        _rand = new Random(1);

        InitObjects();

        start.transform.Translate(new Vector3(_minX, transform.position.y, _minZ));
        end.transform.Translate(new Vector3(_maxX, transform.position.y, _maxZ));

        start.SetActive(true);
        end.SetActive(true);
        spawnableObject.SetActive(true);
        spawnableObject.transform.Translate(new Vector3(1_000_000, 1_000_000, 1_000_000));

        GeneratePoints();
        DrawClosestPath();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateArea();

        if (Input.GetKey(KeyCode.Space))
        {
            GeneratePoints();
            DrawClosestPath();
            Debug.Break();
        }
    }
}