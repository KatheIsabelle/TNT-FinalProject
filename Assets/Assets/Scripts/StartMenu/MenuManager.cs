using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void StartMiniGame1() {
        SceneManager.LoadScene(1);
    }

    public void StartMiniGame2() {
        SceneManager.LoadScene(2);
    }
    public void StartMiniGame3() {
        SceneManager.LoadScene(3);
    }
    public void StartMiniGame4() {
        SceneManager.LoadScene(4);
    }
    public void StartMiniGame5() {
        SceneManager.LoadScene(5);
    }
}
