using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;


public class TerrainGenerator : MonoBehaviour
{
    public List<GameObject> spawnableObjects;
    public float spawnRadiusX = 100;
    public float spawnRadiusZ = 100;
    public uint spawnLimit = 500;
    public float distancing = (float)3.0;
    public float maxSize, minSize;

  

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

            var newObjectType = spawnableObjects.ElementAt(Random.Range(0, spawnableObjects.Count));
            var newPosition = new Vector3(
                Random.Range(spawnArea.x, spawnArea.x + spawnArea.width),
                position.y,
               Random.Range(spawnArea.y, spawnArea.y + spawnArea.height)
            );

            if (_spawnedObjectsPositions.Any(pos => Vector3.Distance(pos, newPosition) < distancing))
                continue;

            var size = Random.Range(minSize, maxSize);
            Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            var newObject = Instantiate(newObjectType, newPosition, rotation, transform);
            newObject.transform.localScale = new Vector3(size, size, size);
            _spawnedObjects.Enqueue(newObject);
            _spawnedObjectsPositions.Enqueue(newPosition);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating(nameof(Log), 0, 1);
        _spawnedObjects = new Queue<GameObject>();
        _spawnedObjectsPositions = new Queue<Vector3>();
        Generate();
    }
}
