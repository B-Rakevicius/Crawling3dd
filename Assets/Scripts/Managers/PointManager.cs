using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointManager : MonoBehaviour
{
    public TextMeshProUGUI slainScore ,pointScore, enemyBuffer;
    public EnemySpawner enemyspawner;
    public int currentSlain = -1, currentPoints = 0;
    public static PointManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
        addSlain();
    }
    public void addPoint()
    {
        currentPoints++;
        pointScore.text = "Points " + currentPoints;
    }
    public void addPoint(int amount)
    {
        currentPoints += amount;
        pointScore.text = "Points " + currentPoints;
    }
    public void addSlain()
    {
        currentSlain++;
        enemyspawner.totalEnemies--;
        slainScore.text = "Slain " + currentSlain;
    }
    void Start()
    {
        
    }
    void Update()
    {
        enemyBuffer.text = "Buffer " + enemyspawner.spawnBuffer;
    }
}
