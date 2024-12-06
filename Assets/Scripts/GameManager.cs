using UnityEngine;
using TMPro;
/*
Script responsável por fazer o gerenciamento dos pontos, 
basicamente somando os pontos e limitando o valor máximo.
E ainda, enviando a pontuação para a UI do telão.
*/
public class GameManager : MonoBehaviour
{
    [Header("UI Settings")]
    public TMP_Text playerPoints;

    [Header("Game Settings")]
    public int maxPoints = 100;
    private int currentPoints = 0;

    public void AddPoints(int points)
    {
        // Soma os pontos e limita o valor máximo
        currentPoints = Mathf.Clamp(currentPoints + points, 0, maxPoints);

        // Atualiza o texto na UI
        playerPoints.text = $"Points: {currentPoints}";
        Debug.Log($"Pontuação Atual: {currentPoints}");
    }
}
