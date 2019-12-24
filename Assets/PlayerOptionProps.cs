using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerOptionProps : MonoBehaviour
{
    public int numberOfPlayers;
    
    void Start() {
        GetComponent<Button>().onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick() {
        GameState.numOfPlayer = numberOfPlayers;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
