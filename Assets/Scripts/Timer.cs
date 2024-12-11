using UnityEngine;
using TMPro;
using System;

public class Timer : MonoBehaviour
{
    private TMP_Text timerText;

    private enum TimerType { Countdown, Stopwatch }
    [SerializeField] private TimerType timerType;
    [SerializeField] private float timeToDisplay = 60.0f;

    private bool isRunning;

    private GameManager gameManager;

    void Awake()
    {
        timerText = GetComponent<TMP_Text>();
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnEnable()
    {
        EventManagerTimer.TimerStart += EventManagerTimerOnTimerStart;
        EventManagerTimer.TimerStop += EventManagerTimerOnTimerStop;
        EventManagerTimer.TimerUpdate += EventManagerTimerOnTimerUpdate;
    }

    private void OnDisable()
    {
        EventManagerTimer.TimerStart -= EventManagerTimerOnTimerStart;
        EventManagerTimer.TimerStop -= EventManagerTimerOnTimerStop;
        EventManagerTimer.TimerUpdate -= EventManagerTimerOnTimerUpdate;
    }

    private void EventManagerTimerOnTimerStart() => isRunning = true;

    private void EventManagerTimerOnTimerStop() => isRunning = false;

    private void EventManagerTimerOnTimerUpdate(float value) => timeToDisplay += value;

    private void Update()
    {
        if (!isRunning) return;

        // Atualiza o tempo com base no tipo de timer
        timeToDisplay += timerType == TimerType.Countdown ? -Time.deltaTime : Time.deltaTime;

        // Verifica se o tempo acabou (para contagem regressiva)
        if (timerType == TimerType.Countdown && timeToDisplay <= 0.0f)
        {
            timeToDisplay = 0.0f; // Garante que o tempo não fique negativo
            timerText.text = "00:00:00"; // Atualiza a UI
            EventManagerTimer.OnTimerStop(); // Para o timer
            gameManager.WinScene(); // Notifica o GameManager para carregar a cena
            return;
        }

        // Formata o tempo em minutos e segundos
        TimeSpan timeSpan = TimeSpan.FromSeconds(timeToDisplay);
        timerText.text = timeSpan.ToString(@"mm\:ss\:ff");
    }
}
