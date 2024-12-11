using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("UI Settings")]
    public TMP_Text playerPoints;
    public TMP_Text player2Points;
    public TMP_Text player3Points;
    public TMP_Text player4Points;

    public GameObject levelCompleteUI;

    [Header("Game Settings")]
    public int maxPoints = 1000;

    private int player1Score = 0;
    private int player2Score = 0;
    private int player3Score = 0;
    private int player4Score = 0;

    [Header("WinScene Settings")]
    public float winWait = 5f;


    private void Start()
    {
        // Inicializa os textos na UI
        UpdateUI();
        EventManagerTimer.OnTimerStart();
    }


    public void WinScene()
    {
        EventManagerTimer.OnTimerStop();
        levelCompleteUI.SetActive(true);
        StartCoroutine(WaitForWinScene());
    }


    private IEnumerator WaitForWinScene()
    {
        Debug.Log("Aguardando cena de vitória...");
        yield return new WaitForSeconds(winWait);
        SceneManager.LoadScene(1);
    }


    public void AddPoints(string playerTag, int points)
    {
        switch (playerTag)
        {
            case "Player":
                player1Score = Mathf.Clamp(player1Score + points, 0, maxPoints);
                break;
            case "Player2":
                player2Score = Mathf.Clamp(player2Score + points, 0, maxPoints);
                break;
            case "Player3":
                player3Score = Mathf.Clamp(player3Score + points, 0, maxPoints);
                break;
            case "Player4":
                player4Score = Mathf.Clamp(player4Score + points, 0, maxPoints);
                break;
        }

        UpdateUI();
    }



    public void LosePoints(string playerTag, int points)
    {
        switch (playerTag)
        {
            case "Player":
                player1Score = Mathf.Clamp(player1Score - points, 0, maxPoints);
                break;
            case "Player2":
                player2Score = Mathf.Clamp(player2Score - points, 0, maxPoints);
                break;
            case "Player3":
                player3Score = Mathf.Clamp(player3Score - points, 0, maxPoints);
                break;
            case "Player4":
                player4Score = Mathf.Clamp(player4Score - points, 0, maxPoints);
                break;
        }

        UpdateUI();
    }



    private void UpdateUI()
    {
        playerPoints.text = $"Points: {player1Score}";
        player2Points.text = $"Points: {player2Score}";
        player3Points.text = $"Points: {player3Score}";
        player4Points.text = $"Points: {player4Score}";
    }
}
