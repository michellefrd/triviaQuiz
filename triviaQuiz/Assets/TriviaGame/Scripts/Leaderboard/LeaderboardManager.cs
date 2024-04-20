using System;
using System.Collections;
using System.Collections.Generic;
using TigerForge.UniDB;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    public Transform leaderboardContent; // El contenido del ScrollView donde se agregarán los elementos del leaderboard
    public GameObject leaderboardItemPrefab; // El prefab del elemento del leaderboard
    private UniDB.Trivia triviaDB;

    // Llamado al iniciar para cargar el leaderboard
    private void Start()
    {
        triviaDB = new UniDB.Trivia();
        ClearChildrenOfParentContainer();
        LoadLeaderboard();
    }


    private void ClearChildrenOfParentContainer()
    {
        foreach (Transform child in leaderboardContent)
        {
            Destroy(child.gameObject);
        }
    }

    public class LeaderboardData
    {
        public string email { get; set; }
        public int score { get; set; }
        public string name { get; set; }
        public string last_name { get; set; }
    }
    
    private void LoadLeaderboard()
    {
        var leaderboardTable = triviaDB.GetTable_Leaderboard();
        var users = triviaDB.GetTable_Users();

        int rank = 1;
        
        _ = leaderboardTable
            .Select(leaderboardTable.C.email, leaderboardTable.C.score, users.C.name, users.C.last_name)
            .Join(Table.JoinType.Inner, users, leaderboardTable.C.email, users.C.email)
            .OrderBy(
                new Order { column = leaderboardTable.C.score, order = Order.Direction.DESC  }
            )
            .Run((List<LeaderboardData> records, Info info) =>
            {
                if (info.isOK && records.Count > 0)
                {
                    Debug.Log("RECORD COUNTS: " + records.Count);
                    foreach (var record in records)
                    {
                        Debug.Log("Procesando registro: " + record.name);
                        
                        // Crear una instancia del prefab del elemento del leaderboard
                        var leaderboardItemGO = Instantiate(leaderboardItemPrefab, leaderboardContent);

                        // Obtener una referencia al componente LeaderboardItem del objeto instanciado
                        var leaderboardItem = leaderboardItemGO.GetComponent<LeaderboardItem>();

                        // Llenar el elemento del leaderboard con la información del registro actual
                        string fullName = record.name + " " + record.last_name; // Concatenar el nombre completo
                        leaderboardItem.name.text = fullName;
                        leaderboardItem.points.text = record.score.ToString();

                        // Mostrar la imagen de clasificación correspondiente
                        // Aquí debes establecer la clasificación adecuada según tu lógica de juego
                        SetRankingImage(leaderboardItem, rank);
                        
                        rank++;
                    }
                }
                else
                {
                    Debug.LogError("Error al cargar el leaderboard: " + info.error);
                }
            });
    }

    // Método para mostrar la imagen de clasificación correspondiente según la posición en el leaderboard
    private void SetRankingImage(LeaderboardItem leaderboardItem, int rank)
    {
        // Desactivar todas las imágenes de clasificación
        foreach (GameObject image in leaderboardItem.rankingImage)
        {
            image.SetActive(false);
        }

        // Mostrar la imagen de clasificación correspondiente según la posición
        if (rank == 1)
        {
            leaderboardItem.rank.text = "";
            leaderboardItem.rankingImage[0].SetActive(true); // Imagen de clasificación para el primer lugar
        }
        else if (rank == 2)
        {
            leaderboardItem.rank.text = "";
            leaderboardItem.rankingImage[1].SetActive(true); // Imagen de clasificación para el segundo lugar
        }
        else if (rank == 3)
        {
            leaderboardItem.rank.text = "";
            leaderboardItem.rankingImage[2].SetActive(true); // Imagen de clasificación para el tercer lugar
        }
        else
        {
            leaderboardItem.rank.text = rank.ToString();
        }
    }
}