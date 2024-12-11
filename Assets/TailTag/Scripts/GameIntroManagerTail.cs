using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameIntroManagerTail : MonoBehaviour {
    public GameObject rulesPanel;     // Painel com as regras
    public TMP_Text countdownText;    // Texto da contagem regressiva
    public Image startGameImage;      // Imagem que aparece ao final da contagem
    public Image imageTime;
    public Image imageStart;

    private float countdownTime = 5f; // Dura��o da contagem regressiva
    private bool gameStarted = false;

    private void Start() {
        // Configura a interface inicial
        rulesPanel.SetActive(true);           // Exibe as regras
        countdownText.gameObject.SetActive(false); // Esconde a contagem
        imageTime.gameObject.SetActive(false);
        imageStart.gameObject.SetActive(false);
        startGameImage.gameObject.SetActive(false); // Esconde a imagem inicial

        Time.timeScale = 0; // Pausa o jogo no in�cio
    }

    private void Update() {
        // Verifica se o jogador pressionou o bot�o para iniciar
        if (!gameStarted && Input.GetButtonDown("Submit")) // Bot�o configurado no Input Manager
        {
            StartCoroutine(StartGameSequence());
        }
    }

    // Corrotina para gerenciar a sequ�ncia do jogo
    private IEnumerator StartGameSequence() {
        // Esconde as regras e inicia a contagem
        rulesPanel.SetActive(false);
        imageTime.gameObject.SetActive(true);
        countdownText.gameObject.SetActive(true);

        // Inicia a contagem regressiva
        float currentTime = countdownTime;
        while (currentTime > 0) {
            countdownText.text = Mathf.CeilToInt(currentTime).ToString();
            currentTime -= Time.unscaledDeltaTime; // Decrementa o tempo com o tempo desacelerado
            yield return null; // Espera at� o pr�ximo frame
        }

        // Mostra a imagem "Abriu, partiu"
        countdownText.gameObject.SetActive(false); // Esconde o texto de contagem
        imageTime.gameObject.SetActive(false);
        imageStart.gameObject.SetActive(true);
        startGameImage.gameObject.SetActive(true);  // Exibe a imagem de in�cio

        // Espera 1,5 segundos com a imagem vis�vel
        yield return new WaitForSecondsRealtime(1.5f); // Usamos WaitForSecondsRealtime para ignorar o Time.timeScale

        // Inicia o jogo
        imageStart.gameObject.SetActive(false);
        startGameImage.gameObject.SetActive(false); // Esconde a imagem de in�cio
        Time.timeScale = 1; // Despausa o jogo

        gameStarted = true; // Define o jogo como iniciado
    }
}