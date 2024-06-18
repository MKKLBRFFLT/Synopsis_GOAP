using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointPickup : MonoBehaviour
{
    private GameManager gameManager;

    void Start()
    {
        
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision detected with: " + other.name); 
        if (other.CompareTag("Player"))
        {
            
            Debug.Log("Player collided with point");
            CollectPoint();
        }
    }

    private void CollectPoint()
    {
        
        gameManager.CollectPoint();

        
        Debug.Log("Point collected!");
        Destroy(gameObject);
    }
}