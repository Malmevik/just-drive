using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class TerrainGenerator : MonoBehaviour
{
    public List<GameObject> spawnableObjects;
    public float spawnRadiusX;
    public float spawnRadiusY;

    private Random _rand;

    private Queue<GameObject> _spawnedObjects;

    private void Log()
    {
        Debug.Log($"types: {spawnableObjects.Count}, spawned: {_spawnedObjects.Count}");
    }

    private void GenerateStep()
    {
        var position = transform.position;

        var spawnArea = new Rect(
            position.x - spawnRadiusX / 2,
            position.y - spawnRadiusY / 2,
            spawnRadiusX,
            spawnRadiusY
        );

        var newObjectType = spawnableObjects.ElementAt(_rand.NextInt(spawnableObjects.Count));
        var newPosition = new Vector3(
            _rand.NextFloat(spawnArea.x, spawnArea.x + spawnArea.width),
            _rand.NextFloat(spawnArea.y, spawnArea.y + spawnArea.height),
            position.z
        );

        var newObject = Instantiate(newObjectType, newPosition, transform.rotation);
        _spawnedObjects.Enqueue(newObject);

        if (_spawnedObjects.Count >= 10)
        {
            var removedObject = _spawnedObjects.Dequeue();
            Destroy(removedObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _rand = new Random(1);
        _spawnedObjects = new Queue<GameObject>();
        InvokeRepeating(nameof(Log), 0, 1);
        InvokeRepeating(nameof(GenerateStep), 0, (float)0.1);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
