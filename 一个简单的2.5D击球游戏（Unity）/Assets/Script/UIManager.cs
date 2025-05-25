using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI level;
    [SerializeField] private Button start;
    private GameManager gameManager;
    private int levelNum = 0;
    [SerializeField] private TextMeshProUGUI par;
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        levelNum = gameManager.GetLevel();
    }

    void Update()
    {
        int parScore = gameManager.ParGet();
        level.text = string.Format("Level {0}", levelNum);
        par.text = string.Format("Par: {0}", parScore);
    }

    public void NextLevel()
    {
        levelNum = gameManager.GetLevel();
    }
}
