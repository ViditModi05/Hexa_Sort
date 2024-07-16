using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompleted : MonoBehaviour
{

    public void OpenScene()
    {
        SceneManager.LoadScene("Main Menu");
    }

}
