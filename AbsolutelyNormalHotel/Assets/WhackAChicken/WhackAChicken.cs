using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhackAChicken : MonoBehaviour
{
    private GameObject[,] whackSpots;
    private bool chickenActive;


    // Start is called before the first frame update
    void Start()
    {
        whackSpots = new GameObject[3, 3];
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                GameObject whackSpot = GameObject.CreatePrimitive(PrimitiveType.Cube);
                whackSpot.transform.SetParent(gameObject.transform, false);
                whackSpot.transform.localScale = new Vector3(.1f, .1f, .1f);
                whackSpot.transform.position = new Vector3(i, .5f, j);
                Material mat = whackSpot.GetComponent<MeshRenderer>().material;
                mat.color = Color.magenta;
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        currentTimeStep += Time.deltaTime;
        SpawnChicken();
        DespawnChicken();
    }

    int GetRandomIndex()
    {
        int rng = Random.Range(0, 2);
        return rng;
    }
    
    float lastTimestep = 0;
    float currentTimeStep = 0;
    private Vector2Int activeChicken;
    void SpawnChicken()
    {
        
        float rng = Random.Range(2, 3);
        
        if (!chickenActive)
        {
            if (currentTimeStep > rng + lastTimestep)
            {
                Debug.Log("Spawned a chicken at:" + currentTimeStep);
                activeChicken = new Vector2Int(GetRandomIndex(), GetRandomIndex());
                GameObject chicken = whackSpots[activeChicken.x, activeChicken.y];
                lastTimestep = currentTimeStep;
                chickenActive = true;
            }
        }

    }
    void DespawnChicken()
    {
        
        float rng = Random.Range(1, 2);
        
        if (chickenActive)
        {
            if (currentTimeStep > rng + lastTimestep)
            {
                Debug.Log("Despawned a chicken at:" + currentTimeStep);
                GameObject chicken = whackSpots[activeChicken.x, activeChicken.y];
                chicken.GetComponent<BoxCollider>().enabled = false;
                lastTimestep = currentTimeStep;
                chickenActive = false;
            }
        }

    }
}
