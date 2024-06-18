using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text pointsText;
    private int totalPoints;
    private int pointsCollected;

    void Start()
    {
        // Find alle point
        GameObject[] points = GameObject.FindGameObjectsWithTag("Point");
        totalPoints = points.Length-1;
        pointsCollected = 0;

        // Updater counter
        UpdatePointsText();
    }

    public void CollectPoint()
    {
        pointsCollected++;
        UpdatePointsText();

        if (pointsCollected >= totalPoints)
        {
            WinGame();
        }
    }

    void UpdatePointsText()
    {
        pointsText.text = "Points Left: " + (totalPoints - pointsCollected);
    }

    void WinGame()
    {
        
        pointsText.text = "You Win!";
        // Stopper spillet
        Time.timeScale = 0f;
    }

    public void GameOver()
    {
        // Display game over message
        pointsText.text = "Game Over";
        // Optionally stop the game
        Time.timeScale = 0f;
    }
}