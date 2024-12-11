using System.Collections.Generic;
using UnityEngine;

public static class GameData
{
    // Lista para guardar as informações dos jogadores
    public static List<PlayerData> playerRankings = new List<PlayerData>();
}

// Classe para armazenar as informações de cada jogador
[System.Serializable]
public class PlayerData
{
    public string playerName;
    public int points;
    public GameObject playerObject;  // Adicionando a referência para o GameObject do jogador

    // Construtor
    public PlayerData(string playerName, int points, GameObject playerObject)
    {
        
        this.playerName = playerName;
        this.points = points;
        this.playerObject = playerObject;  // Inicializando o GameObject do jogador
        Debug.Log(playerObject);
    }
}

