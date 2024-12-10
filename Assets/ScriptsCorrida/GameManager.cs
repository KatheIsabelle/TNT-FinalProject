using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public List<PlayerControllerCorrida> jogadores; // Lista de jogadores
    public Image TextoAbriuPartiu;
    public Text textoContagem; // Texto para exibir a contagem
    private bool jogoIniciado = false; // Controle se o jogo já começou

    void Start()
    {
        // Inicializa a lista de jogadores
        jogadores = new List<PlayerControllerCorrida>(FindObjectsOfType<PlayerControllerCorrida>());

        // Desativa o movimento dos jogadores
        foreach (var jogador in jogadores)
        {
            jogador.SetMovimentoAtivo(false);
        }

        // Começa a contagem regressiva
        StartCoroutine(ContagemRegressiva());
    }

    private IEnumerator ContagemRegressiva()
    {
        int contagem = 3;

        while (contagem > 1)
        {
            textoContagem.text = contagem.ToString();
            yield return new WaitForSeconds(1f);
            contagem--;
        }
        textoContagem.gameObject.SetActive(false); // Esconde o texto

        // Quando a contagem termina
        TextoAbriuPartiu.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);

        TextoAbriuPartiu.gameObject.SetActive(false);
        jogoIniciado = true;

        // Ativa o movimento dos jogadores
        foreach (var jogador in jogadores)
        {
            jogador.SetMovimentoAtivo(true);
        }
    }

    public bool JogoIniciado()
    {
        return jogoIniciado;
    }
}
