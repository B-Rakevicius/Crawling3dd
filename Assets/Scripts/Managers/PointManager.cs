using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointManager : MonoBehaviour
{
    public TextMeshProUGUI thisnx;
    private int currentPointsnx = 0;
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
        Thisnx();
    }
    public void Thisnx()
    {
        currentPointsnx++;
        thisnx.text = "Score " + currentPointsnx; 
    }
    void Start()
    {
        
    }
    void Update()
    {
        
    }
}
