using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Score : MonoBehaviour
{
    [Header("Settings")]
    public TextMeshProUGUI scoreText;
    public int score = 0;
    public int maxScore;
    public GameObject levelCompleted;
    
    void Start()
    {
        score = 0;
    }

    public void AddScore(int newScore)
    {
        score += newScore;
    }

    public void UpdateScore()
    {
        scoreText.text = "Score " + score;
    }
    
    // Update is called once per frame
    void Update()
    {
       UpdateScore(); 
       if(score >= maxScore)
       {
         StartCoroutine(OpenScene());
       }

    }
    IEnumerator OpenScene()
    {
       yield return new WaitForSeconds(0.5f);
       levelCompleted.SetActive(true);
    }
}
