using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PointManager : MonoBehaviour
{
    [Header("Player HP")]
    [SerializeField] private int playerMaxHealth = 3;
    [HideInInspector] public int playerHealth;

    [Header("GameObjects")]
    [SerializeField] private PlayerMovement player;
    [SerializeField] private Catch hook;
    [SerializeField] private Monster monster;
    [SerializeField] private FishSpawner rightFishSpawner;
    [SerializeField] private FishSpawner leftFishSpawner;

    // Points
    private int currentPoint = 0;
    private int highPoint = 0;

    [Header("UIs")]
    [SerializeField] private TextMeshProUGUI currentScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private GameObject restart;

    // Start is called before the first frame update
    void Start()
    {
        // Give the player health
        playerHealth = playerMaxHealth;
        // Get the highest point
        highPoint = PlayerPrefs.GetInt("HighPoint", 0);
    }

    // Update is called once per frame
    void Update()
    {
        PlayerDead();
        UpdateUI();
    }

    /// <summary>
    /// If the player dead stop the game and show the restart UI
    /// </summary>
    private void PlayerDead()
    {
        if (playerHealth <= 0)
        {
            monster.IsDead();
            rightFishSpawner.IsDead();
            leftFishSpawner.IsDead();
            player.IsDead();
            ShowRestartButton(true);
        }
    }

    /// <summary>
    /// Show or not the button and background
    /// </summary>
    /// <param name="show">Show or not the button and background</param>
    public void ShowRestartButton(bool show) 
    {
        restart.gameObject.SetActive(show);
    }

    /// <summary>
    /// Update the UI every seceon
    /// </summary>
    private void UpdateUI()
    {
        currentScoreText.text = "Current Score: " + currentPoint.ToString();
        highScoreText.text = "High Score: " + highPoint.ToString();
    }

    /// <summary>
    /// Increase the point when we catch some fish and save it
    /// </summary>
    /// <param name="point">Fish point</param>
    public void IncreaseScore(int point)
    {
        currentPoint += point;

        if (currentPoint > highPoint)
        {
            highPoint = currentPoint;

            PlayerPrefs.SetInt("HighPoint", highPoint);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// If we hit the restart button then restart the game
    /// </summary>
    public void Restart()
    {
        ResetCurrentScore();
        monster.Restart();
        player.Restart();
        rightFishSpawner.Restart();
        leftFishSpawner.Restart();
        ShowRestartButton(false);
        playerHealth = playerMaxHealth;
    }

    /// <summary>
    /// Reset the point when restart the game
    /// </summary>
    public void ResetCurrentScore()
    {
        currentPoint = 0;
    }
}
