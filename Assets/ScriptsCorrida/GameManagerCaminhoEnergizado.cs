using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerCaminhoEnergizado : MonoBehaviour
{
    public List<PlayerControllerCorridaCaminhoEnergizado> jogadores; // Lista de jogadores
    public Image TextoAbriuPartiu;
    public Text textoContagem; // Texto para exibir a contagem
    private bool jogoIniciado = false; // Controle se o jogo já começou
    private int jogadoresQueTerminaram = 0; // Contador de jogadores que terminaram a corrida
    public Text textoRanking; // Texto para exibir o ranking

    void Start()
    {
        // Inicializa a lista de jogadores
        jogadores = new List<PlayerControllerCorridaCaminhoEnergizado>(FindObjectsOfType<PlayerControllerCorridaCaminhoEnergizado>());

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

        while (contagem > 0)
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

        // Ativa o movimento da câmera
        Camera.main.GetComponent<CameraControllerCaminhoEnergizado>().AtivarMovimentoCamera();
    }

    public void JogadorTerminou()
    {
        jogadoresQueTerminaram++;

        // Verifica se todos os jogadores terminaram
        if (jogadoresQueTerminaram >= jogadores.Count)
        {
            // Finaliza o jogo
            Camera.main.GetComponent<CameraControllerCaminhoEnergizado>().PosicionarCameraFinal();
            MostrarRanking();
        }
    }

    private void MostrarRanking()
    {
        // Ordena os jogadores por pontuação (decrescente)
        var ranking = jogadores.OrderByDescending(jogador => jogador.pontuacao).ToList();

        // Monta o texto do ranking
        string texto = "Ranking Final:\n";
        for (int i = 0; i < ranking.Count; i++)
        {
            texto += $"{i + 1}. Jogador {ranking[i].playerID}: {ranking[i].pontuacao} pontos\n";
        }

        // Atualiza o texto e exibe o painel de ranking
        textoRanking.text = texto;
        textoRanking.gameObject.SetActive(true);
    }

    public bool JogoIniciado()
    {
        return jogoIniciado;
    }
}
