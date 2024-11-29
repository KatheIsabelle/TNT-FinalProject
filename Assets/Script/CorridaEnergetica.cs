using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CorridaEnergetica : MonoBehaviour
{
    // Botões disponíveis para cada jogador (mapeados para controle PS4)
    private string[] botoesDisponiveis = { "joystick button 1", "joystick button 2", "joystick button 0", "joystick button 3" };

    private string botaoCertoPlayer1, botaoCertoPlayer2, botaoCertoPlayer3, botaoCertoPlayer4;
    private int pontosPlayer1 = 0, pontosPlayer2 = 0, pontosPlayer3 = 0, pontosPlayer4 = 0;

    public Image iconeBotaoPlayer1, iconeBotaoPlayer2, iconeBotaoPlayer3, iconeBotaoPlayer4;
    public Sprite[] botoesSprites;
    public Transform player1, player2, player3, player4;
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
        yield return new WaitForSeconds(18f); // Tempo antes de desativar comandos

        canvas.gameObject.SetActive(false);
        comandosAtivos = false;

        float distanciaPlayer1 = pontosPlayer1 * 10f;
        float distanciaPlayer2 = pontosPlayer2 * 10f;
        float distanciaPlayer3 = pontosPlayer3 * 10f;
        float distanciaPlayer4 = pontosPlayer4 * 10f;

        Vector3 destino1 = player1.position + Vector3.right * distanciaPlayer1;
        Vector3 destino2 = player2.position + Vector3.right * distanciaPlayer2;
        Vector3 destino3 = player3.position + Vector3.right * distanciaPlayer3;
        Vector3 destino4 = player4.position + Vector3.right * distanciaPlayer4;

        float tempo = 0;
        while (tempo < 1f)
        {
            player1.position = Vector3.Lerp(player1.position, destino1, tempo);
            player2.position = Vector3.Lerp(player2.position, destino2, tempo);
            player3.position = Vector3.Lerp(player3.position, destino3, tempo);
            player4.position = Vector3.Lerp(player4.position, destino4, tempo);

            AtualizaCamera(player1, player2, player3, player4);
            tempo += Time.deltaTime * playerMoveSpeed;
            yield return null;
        }
    }

    void AtualizaCamera(Transform p1, Transform p2, Transform p3, Transform p4)
    {
        Transform lider = p1;
        if (p2.position.x > lider.position.x) lider = p2;
        if (p3.position.x > lider.position.x) lider = p3;
        if (p4.position.x > lider.position.x) lider = p4;

        Vector3 novaPosicao = new Vector3(lider.position.x, mainCamera.transform.position.y, mainCamera.transform.position.z);
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, novaPosicao, Time.deltaTime * cameraFollowSpeed);
    }
}
