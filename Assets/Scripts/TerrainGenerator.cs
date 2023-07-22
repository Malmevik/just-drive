using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class TerrainGenerator : MonoBehaviour
{
    public List<GameObject> spawnableObjects;
    public float spawnRadiusX = 10;
    public float spawnRadiusZ = 10;
    public uint spawnLimit = 10;
    public float distancing = (float)1.0;

    private Random _rand;

    private Queue<GameObject> _spawnedObjects;
    private Queue<Vector3> _spawnedObjectsPositions;

    private void Log()
    {
        Debug.Log($"types: {spawnableObjects.Count}, spawned: {_spawnedObjects.Count}");
    }

    private void Destruct()
    {
        foreach (var obj in _spawnedObjects)
        {
            Destroy(obj);
        }

        _spawnedObjects = new Queue<GameObject>();
        _spawnedObjectsPositions = new Queue<Vector3>();
    }

    private void Generate()
    {
        Destruct();

        var position = transform.position;

        var spawnArea = new Rect(
            position.x - spawnRadiusX / 2,
            position.z - spawnRadiusZ / 2,
            spawnRadiusX,
            spawnRadiusZ
        );

        const int maxAttempts = 10_000;
        var attempts = 0;

        while (_spawnedObjects.Count < spawnLimit)
        {
            attempts++;
            if (attempts >= maxAttempts)
            {
                Debug.Log("Attempts exhausted");
                break;
            }

            var newObjectType = spawnableObjects.ElementAt(_rand.NextInt(spawnableObjects.Count));
            var newPosition = new Vector3(
                _rand.NextFloat(spawnArea.x, spawnArea.x + spawnArea.width),
                position.y,
                _rand.NextFloat(spawnArea.y, spawnArea.y + spawnArea.height)
            );

            if (_spawnedObjectsPositions.Any(pos => Vector3.Distance(pos, newPosition) < distancing))
                continue;

            var newObject = Instantiate(newObjectType, newPosition, transform.rotation);
            _spawnedObjects.Enqueue(newObject);
            _spawnedObjectsPositions.Enqueue(newPosition);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating(nameof(Log), 0, 1);

        _rand = new Random(1);
        _spawnedObjects = new Queue<GameObject>();
        _spawnedObjectsPositions = new Queue<Vector3>();
        // InvokeRepeating(nameof(Generate), 0, (float)0.1);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Generate();
        }
    }
}
