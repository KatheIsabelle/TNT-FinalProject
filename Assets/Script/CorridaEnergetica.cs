using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CorridaEnergetica : MonoBehaviour
{
    private string[] botoesDisponiveis = { "joystick button 1", "joystick button 2", "joystick button 0", "joystick button 3" };

    private string botaoCertoPlayer1, botaoCertoPlayer2, botaoCertoPlayer3, botaoCertoPlayer4;
    private int pontosPlayer1 = 0, pontosPlayer2 = 0, pontosPlayer3 = 0, pontosPlayer4 = 0;

    public Image iconeBotaoPlayer1, iconeBotaoPlayer2, iconeBotaoPlayer3, iconeBotaoPlayer4;
    public Sprite[] botoesSprites;
    public Transform player1, player2, player3, player4;
    public Transform empty1, empty2, empty3, empty4;
    public Camera mainCamera;
    public Canvas canvas;

    public float playerMoveSpeed = 2f, cameraFollowSpeed = 5f;

    private bool aguardandoDelay = false, comandosAtivos = true;

    private void Start()
    {
        botaoCertoPlayer1 = botoesDisponiveis[Random.Range(0, botoesDisponiveis.Length)];
        botaoCertoPlayer2 = botoesDisponiveis[Random.Range(0, botoesDisponiveis.Length)];
        botaoCertoPlayer3 = botoesDisponiveis[Random.Range(0, botoesDisponiveis.Length)];
        botaoCertoPlayer4 = botoesDisponiveis[Random.Range(0, botoesDisponiveis.Length)];

        AtualizaIcone(iconeBotaoPlayer1, botaoCertoPlayer1);
        AtualizaIcone(iconeBotaoPlayer2, botaoCertoPlayer2);
        AtualizaIcone(iconeBotaoPlayer3, botaoCertoPlayer3);
        AtualizaIcone(iconeBotaoPlayer4, botaoCertoPlayer4);
    }

    void Update()
    {
        if (!comandosAtivos) return;

        if (Input.GetKeyDown(botaoCertoPlayer1)) AtualizaPontos(ref pontosPlayer1, iconeBotaoPlayer1, ref botaoCertoPlayer1);
        if (Input.GetKeyDown(botaoCertoPlayer2)) AtualizaPontos(ref pontosPlayer2, iconeBotaoPlayer2, ref botaoCertoPlayer2);
        if (Input.GetKeyDown(botaoCertoPlayer3)) AtualizaPontos(ref pontosPlayer3, iconeBotaoPlayer3, ref botaoCertoPlayer3);
        if (Input.GetKeyDown(botaoCertoPlayer4)) AtualizaPontos(ref pontosPlayer4, iconeBotaoPlayer4, ref botaoCertoPlayer4);

        if (!aguardandoDelay) StartCoroutine(MovimentaJogadoresAposDelay());
    }

    void AtualizaPontos(ref int pontos, Image icone, ref string botao)
    {
        pontos++;
        RandomizaBotaoParaJogador(ref botao, icone);
    }

    void RandomizaBotaoParaJogador(ref string botaoCerto, Image icone)
    {
        string novoBotao;
        do { novoBotao = botoesDisponiveis[Random.Range(0, botoesDisponiveis.Length)]; }
        while (novoBotao == botaoCerto);

        botaoCerto = novoBotao;
        AtualizaIcone(icone, botaoCerto);
    }

    void AtualizaIcone(Image icone, string botao)
    {
        int index = System.Array.IndexOf(botoesDisponiveis, botao);
        if (index >= 0 && index < botoesSprites.Length) icone.sprite = botoesSprites[index];
    }

    IEnumerator MovimentaJogadoresAposDelay()
    {
        aguardandoDelay = true;
        yield return new WaitForSeconds(5f); // Tempo antes de desativar comandos

        canvas.gameObject.SetActive(false);
        comandosAtivos = false;

        // Lista de jogadores com os seus empty objects
        List<(Transform jogador, Transform empty, int pontos)> jogadores = new List<(Transform, Transform, int)>
        {
            (player1, empty1, pontosPlayer1),
            (player2, empty2, pontosPlayer2),
            (player3, empty3, pontosPlayer3),
            (player4, empty4, pontosPlayer4)
        };

        // Ordena os jogadores pela pontuação
        jogadores.Sort((a, b) => b.pontos.CompareTo(a.pontos));

        // Define a posição final dos empty objects baseados na classificação
        for (int i = 0; i < jogadores.Count; i++)
        {
            Transform jogador = jogadores[i].jogador;
            Transform empty = jogadores[i].empty;

            // Define a posição final para os empty objects
            empty.position = new Vector3(
                jogador.position.x + (10 * (i + 1)), // Distância crescente baseada na classificação
                jogador.position.y,
                jogador.position.z
            );
        }

        // Move os jogadores e a câmera gradualmente
        bool movimentacaoCompleta = false;
        while (!movimentacaoCompleta)
        {
            movimentacaoCompleta = true;

            for (int i = 0; i < jogadores.Count; i++)
            {
                var (jogador, empty, _) = jogadores[i];

                // Movimenta cada jogador para seu waypoint (empty object)
                jogador.position = Vector3.MoveTowards(
                    jogador.position,
                    empty.position,
                    Time.deltaTime * playerMoveSpeed
                );

                if (Vector3.Distance(jogador.position, empty.position) > 0.01f)
                {
                    movimentacaoCompleta = false;
                }
            }

            // Atualiza a câmera para seguir o líder
            AtualizaCamera(jogadores[0].jogador);

            yield return null; // Espera um frame antes de continuar
        }
    }

    void AtualizaCamera(Transform jogadorLider)
    {
        // Alvo da posição da câmera
        Vector3 alvoPosicao = new Vector3(
            jogadorLider.position.x + 38, // Ajuste a câmera para ficar à frente do líder
            mainCamera.transform.position.y,
            mainCamera.transform.position.z
        );

        // Move a câmera suavemente em direção ao alvo
        mainCamera.transform.position = Vector3.MoveTowards(
            mainCamera.transform.position,
            alvoPosicao,
            Time.deltaTime * cameraFollowSpeed
        );
    }
}
