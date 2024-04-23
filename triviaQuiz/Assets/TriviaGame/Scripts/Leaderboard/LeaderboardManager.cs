using System.Collections.Generic;
using TigerForge.UniDB;
using TMPro;
using UnityEngine;

/// <summary>
/// Gestiona el funcionamiento del leaderboard en la aplicación, mostrando las puntuaciones de los jugadores y manejando el registro de nuevos.
/// </summary>
public class LeaderboardManager : MonoBehaviour
{
    [Header("Leaderboard")]
    // Contenedor para los elementos del leaderboard en el ScrollView
    public Transform leaderboardContent;

    // Prefab para los elementos individuales del leaderboard
    public GameObject leaderboardItemPrefab;

    //Conexion base de datos
    private UniDB.Trivia triviaDB;

    [Header("My user")]
    // Texto para mostrar el nombre del jugador en el leaderboard
    public TMP_Text MyNameTxt;

    // Texto para mostrar los puntos del jugador
    public TMP_Text MyPointsTxt;

    // Array de GameObjects para mostrar gráficamente el rango del jugador
    public GameObject[] MyRanking;

    // Texto para mostrar el rango numérico del jugador
    public TMP_Text MyRank;

    /// <summary>
    /// Se ejecuta en el primer frame para inicializar el leaderboard.
    /// </summary>
    private void Start()
    {
        // Inicializa la conexión con la base de datos de trivia.
        triviaDB = new UniDB.Trivia();
        // Carga la información del leaderboard.
        LoadLeaderboard();
    }

    /// <summary>
    /// Elimina todos los hijos del contenedor del leaderboard para preparar la recarga de datos.
    /// </summary>
    private void ClearChildrenOfParentContainer()
    {
        foreach (Transform child in leaderboardContent)
        {
            // Destruye cada hijo para evitar duplicados al recargar.
            Destroy(child.gameObject); 
        }
    }

    /// <summary>
    /// Clase para manejar los datos del leaderboard.
    /// </summary>
    public class LeaderboardData
    {
        public string email { get; set; }
        public int score { get; set; }
        public string name { get; set; }
        public string last_name { get; set; }
    }

    /// <summary>
    /// Carga los datos del leaderboard de la base de datos y crea un elemento de UI para cada entrada.
    /// </summary>
    private void LoadLeaderboard()
    {
        // Limpia el contenedor antes de cargar nuevos datos.
        ClearChildrenOfParentContainer(); 
        var leaderboardTable = triviaDB.GetTable_Leaderboard();
        var users = triviaDB.GetTable_Users();
        // Email del jugador actual para identificación.
        string myEmail = PlayerPrefs.GetString("Email"); 
        bool found = false;
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
                            // Mostrar el texto personalizado para el usuario actual.
                            MyNameTxt.text = "Mi puntuación";
                            MyPointsTxt.text = record.score + " pts";
                            MyRank.text = rank.ToString();
                            MyRank.gameObject.SetActive(true);
                            // Actualizar imágenes de ranking según la posición.
                            UpdateMyRankingImages(rank); 
                        }

                        var leaderboardItemGO = Instantiate(leaderboardItemPrefab, leaderboardContent);
                        var leaderboardItem = leaderboardItemGO.GetComponent<LeaderboardItem>();

                        leaderboardItem.name.text =
                            fullName; // Mostrar el nombre completo del jugador en el leaderboard.
                        leaderboardItem.points.text = record.score + " pts";
                        // Establecer imagen de ranking visual.
                        SetRankingImage(leaderboardItem, rank); 
                        rank++;
                    }

                    if (!found) // Si no se encuentra al jugador, añadirlo con 0 puntos.
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

    /// <summary>
    /// Inserta un nuevo registro en la tabla del leaderboard para un jugador.
    /// </summary>
    /// <param name="email">Email del usuario a insertar en el leaderboard</param>
    private void InsertPlayerRecord(string email)
    {
        var leaderboardTable = triviaDB.GetTable_Leaderboard();

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
                    Debug.Log("Insertado nuevo email: " + email);
                    // Recargar el leaderboard para incluir el nuevo registro.
                    LoadLeaderboard(); 
                }
                else
                {
                    Debug.LogWarning("Fallo en la inserción del nuevo email: " + info.error);
                }
            });
    }

    /// <summary>
    /// Actualiza las imágenes de ranking para el jugador actual según su posición.
    /// </summary>
    /// <param name="myRank">Numero de posicion del jugador</param>
    private void UpdateMyRankingImages(int myRank)
    {
        foreach (GameObject image in MyRanking)
        {
            image.SetActive(false); // Desactiva todas las imágenes primero.
        }

        if (myRank >= 1 && myRank <= 3)
        {
            // Activar la imagen correspondiente al rango 1-3.
            MyRanking[myRank - 1].SetActive(true); 
            // No mostrar texto de rango si se muestra la imagen.
            MyRank.text = ""; 
        }
        else
        {
            // Mostrar el rango numérico si está fuera del top 3.
            MyRank.text = myRank.ToString(); 
            MyRank.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Configura la imagen de clasificación para cada entrada del leaderboard.
    /// </summary>
    /// <param name="leaderboardItem">Instancia del objeto leaderboard</param>
    /// <param name="rank">Número de posición del jugador</param>
    private void SetRankingImage(LeaderboardItem leaderboardItem, int rank)
    {
        foreach (GameObject image in leaderboardItem.rankingImage)
        {
            image.SetActive(false); // Desactivar todas las imágenes primero.
        }

        if (rank == 1)
        {
            leaderboardItem.rank.text = "";
            leaderboardItem.rankingImage[0].SetActive(true); // Imagen para el primer lugar.
        }
        else if (rank == 2)
        {
            leaderboardItem.rank.text = "";
            leaderboardItem.rankingImage[1].SetActive(true); // Imagen para el segundo lugar.
        }
        else if (rank == 3)
        {
            leaderboardItem.rank.text = "";
            leaderboardItem.rankingImage[2].SetActive(true); // Imagen para el tercer lugar.
        }
        else
        {
            leaderboardItem.rank.gameObject.SetActive(true);
            leaderboardItem.rank.text = rank.ToString(); // Mostrar el rango numérico para otros puestos.
        }
    }
}