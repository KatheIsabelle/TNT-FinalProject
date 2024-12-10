using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameIntroManager : MonoBehaviour
{
    public GameObject rulesPanel;     // Painel com as regras
    public TMP_Text countdownText;    // Texto da contagem regressiva

    private float countdownTime = 5f; // Duração da contagem regressiva
    private bool isCountingDown = false;

    private void Start() {
        // Configura a interface inicial
        rulesPanel.SetActive(true);   // Exibe as regras
        countdownText.gameObject.SetActive(false); // Esconde a contagem

        Time.timeScale = 0; // Pausa o jogo no início
    }

    private void Update() {
        // Verifica se o jogador pressionou o botão para iniciar
        if (!isCountingDown && Input.GetButtonDown("Submit")) // Botão configurado no Input Manager
        {
            StartCountdown();
        }

        // Realiza a contagem regressiva
        if (isCountingDown) {
            countdownTime -= Time.unscaledDeltaTime; // Usa tempo desacelerado
            if (countdownTime > 0) {
                countdownText.text = Mathf.CeilToInt(countdownTime).ToString();
            } else {
                StartGame(); // Inicia o jogo ao término da contagem
            }
        }
    }

    private void StartCountdown() {
        // Esconde o painel de regras
        rulesPanel.SetActive(false);

        // Exibe o texto da contagem
        countdownText.gameObject.SetActive(true);

        // Inicia a contagem regressiva
        isCountingDown = true;
    }

    private void StartGame() {
        // Finaliza a contagem e inicia o jogo
        isCountingDown = false;
        countdownText.gameObject.SetActive(false);
        Time.timeScale = 1; // Despausa o jogo
    }
}
