using System;
using UnityEngine;


public class Obstacles : MonoBehaviour
{
    public static Obstacles instance;
    public GameObject[] obstacles;

    public Transform bridgeSpawnerParent;
    
    public int lastInstantiatedObjectIndex = -1;
    
    
    
    public float timeBtwSpawn;
    public float startTimeBtwSpawn;
    public float decreaseTime;
    public float minTime = 0.65f;
    
    public LoopingBackground loopingBackground;


    private void Awake()
    {
        instance = this;
    }


    void Update()
    {
        if (GameOver.instance.gameOver == true)
        {
            timeBtwSpawn = 0;
            return;
        }
        if (timeBtwSpawn <= 0)
        {
           
            timeBtwSpawn = startTimeBtwSpawn;
            if (startTimeBtwSpawn > minTime)
            {
                SpawnNextObstacle();
                   
                startTimeBtwSpawn -= decreaseTime;
            }
          
        }
        else {
            timeBtwSpawn -= Time.deltaTime; 
        }
            
      
        
        
    }
    void SpawnNextObstacle()
    {
        
        lastInstantiatedObjectIndex = (lastInstantiatedObjectIndex + 1) % obstacles.Length;
            
        if (lastInstantiatedObjectIndex== 3 || lastInstantiatedObjectIndex== 5||lastInstantiatedObjectIndex== 10 || lastInstantiatedObjectIndex== 13 || lastInstantiatedObjectIndex== 16)
        {
                Instantiate(obstacles[lastInstantiatedObjectIndex ], bridgeSpawnerParent.transform);
        }
        else
        {
                Instantiate(obstacles[lastInstantiatedObjectIndex ], transform);
        }
            
        if ( (lastInstantiatedObjectIndex + 1) % obstacles.Length == 0)
        {
            Debug.Log("Increased speed");
            loopingBackground.speed += 0.05f;
        }        
    }
}
