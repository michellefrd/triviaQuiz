using System;
using System.Collections;
using System.Collections.Generic;
using TigerForge.UniDB;
using TMPro;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    public Transform leaderboardContent; // El contenido del ScrollView donde se agregarán los elementos del leaderboard
    public GameObject leaderboardItemPrefab; // El prefab del elemento del leaderboard
    private UniDB.Trivia triviaDB;
    public TMP_Text MyNameTxt;
    public TMP_Text MyPointsTxt;
    public GameObject[] MyRanking;
    public TMP_Text MyRank;

    // Llamado al iniciar para cargar el leaderboard
    private void Start()
    {
        triviaDB = new UniDB.Trivia();

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
        ClearChildrenOfParentContainer();
        var leaderboardTable = triviaDB.GetTable_Leaderboard();
        var users = triviaDB.GetTable_Users();
        string myEmail = PlayerPrefs.GetString("Email"); // Obtener el email del jugador actual
        bool found = false; // Variable para verificar si encontramos el email del jugador
        int rank = 1;

        _ = leaderboardTable
            .Select(leaderboardTable.C.email, leaderboardTable.C.score, users.C.name, users.C.last_name)
            .Join(Table.JoinType.Inner, users, leaderboardTable.C.email, users.C.email)
            .OrderBy(
                new Order { column = leaderboardTable.C.score, order = Order.Direction.DESC }
            )
            .Run((List<LeaderboardData> records, Info info) =>
            {
                if (info.isOK && records.Count > 0)
                {
                    foreach (var record in records)
                    {
                        var fullName = record.name + " " + record.last_name;
                        if (record.email == myEmail)
                        {
                            found = true;
                            MyNameTxt.text = "Mi puntuación"; // Mostrar "Mi puntuación" en lugar de nombre
                            MyPointsTxt.text = record.score + " pts";
                            MyRank.text = rank.ToString();
                            UpdateMyRankingImages(rank);
                        }

                        var leaderboardItemGO = Instantiate(leaderboardItemPrefab, leaderboardContent);
                        var leaderboardItem = leaderboardItemGO.GetComponent<LeaderboardItem>();

                        leaderboardItem.name.text = fullName; // Mostrar el nombre completo del jugador
                        leaderboardItem.points.text = record.score + " pts";
                        SetRankingImage(leaderboardItem, rank);
                        rank++;
                    }

                    if (!found) // Si no encontramos el email del jugador, añadirlo con 0 puntos
                    {
                        InsertPlayerRecord(myEmail);
                    }
                }
                else
                {
                    Debug.LogError("Error al cargar el leaderboard: " + info.error);
                }
            });
    }

    private void InsertPlayerRecord(string email)
    {
        var leaderboardTable = triviaDB.GetTable_Leaderboard();

        // Utilizando el formato correcto para insertar
        _ = leaderboardTable
            .Insert()
            .Data(
                leaderboardTable.C.email.Value(email),
                leaderboardTable.C.score.Value(0)
            )
            .Run((Info info) =>
            {
                if (info.isOK)
                {
                    Debug.Log("Inserted new leaderboard record for email: " + email);
                    LoadLeaderboard(); // Recargar el leaderboard para mostrar el nuevo registro
                }
                else
                {
                    Debug.LogWarning("Failed to insert new leaderboard record: " + info.error);
                }
            });
    }


    private void UpdateMyRankingImages(int myRank)
    {
        // Desactivar todas las imágenes de clasificación del jugador
        foreach (GameObject image in MyRanking)
        {
            image.SetActive(false);
        }

        // Mostrar la imagen de clasificación correspondiente para el primer, segundo y tercer lugar
        if (myRank >= 1 && myRank <= 3)
        {
            MyRanking[myRank - 1].SetActive(true); // Activar la imagen de clasificación para 1st, 2nd o 3rd
            MyRank.text = ""; // Limpiar el texto del rango ya que la imagen lo indica
        }
        else
        {
            MyRank.text = myRank.ToString(); // Mostrar el número de clasificación para rangos fuera del top 3
        }
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