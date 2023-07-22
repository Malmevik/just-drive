using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Random = UnityEngine.Random;

public class ExampleScript : MonoBehaviour
{
    public int meaningOfLife;
    [SerializeField] private List<GameObject> spawns;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var previousPosition = transform.position;
        var newPosition = new Vector3(previousPosition.x + meaningOfLife * Time.deltaTime, previousPosition.y, previousPosition.z);
        if (Input.GetKey(KeyCode.Space))
        {
            transform.position = newPosition;
            var i = Random.Range(0, spawns.Count);
            Instantiate(spawns[i], newPosition, transform.rotation) ;
        }
    }
}
 