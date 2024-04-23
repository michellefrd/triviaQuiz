using System.Collections.Generic;
using UnityEngine;
using TigerForge.UniDB;

/// <summary>
/// Gestiona la interfaz del menú de categorías y el manejo de las puntuaciones del jugador.
/// </summary>
public class MenuManager : MonoBehaviour
{
    // Instancia de la base de datos de Trivia.
    private UniDB.Trivia triviaDB; 

    [Header("Transform")]
    // Objeto padre para sostener los botones de categorías instanciados.
    public Transform categoryButtonParent;

    [Header("Prefab")]
    // Prefab para los botones de categorías.
    public GameObject categoryButtonPrefab;

    /// <summary>
    /// Start es llamado antes de la primera actualización del frame.
    /// </summary>
    void Start()
    {
        // Inicializa la base de datos.
        triviaDB = new UniDB.Trivia(); 
        // Carga las categorías desde la base de datos.
        OnLoadCategories(); 
        // Verifica la puntuación del jugador.
        CheckPlayerScore(); 
    }

    /// <summary>
    /// Carga las categorías desde la base de datos y crea un botón para cada una en la interfaz de usuario.
    /// </summary>
    private void OnLoadCategories()
    {
        // Accede a la tabla de categorías.
        var categories = triviaDB.GetTable_Categories(); 
        _ = categories
            .Select()
            .Run(
                (List<UniDB.Trivia.Categories.Record> data, Info info) =>
                {
                    if (info.isOK && data != null)
                    {
                        foreach (var d in data)
                        {
                            // Instancia un nuevo botón.
                            GameObject newButton = Instantiate(categoryButtonPrefab, categoryButtonParent); 
                            // Asigna un nombre al botón para facilitar su identificación.
                            newButton.name = "CategoryButton_" + d.name; 
                            // Accede al script del botón.
                            CategoryButton buttonScript = newButton.GetComponent<CategoryButton>(); 
                            if (buttonScript != null)
                            {
                                // Establece el nombre de la categoría en el botón.
                                buttonScript.categoryNameTxt.text = d.name; 
                                // Establece el código de la categoría.
                                buttonScript.categoryID = (int)d.code; 
                            }
                            else
                            {
                                Debug.LogError("CategoryButton no encontrado");
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("Error: " + info.error); // Reporta un error si la carga falla.
                    }
                }
            );
    }

    /// <summary>
    /// Verifica la puntuación del jugador actual en la base de datos y la actualiza si es necesario.
    /// </summary>
    private void CheckPlayerScore()
    {
        // Accede a la tabla del leaderboard.
        var leaderboard = triviaDB.GetTable_Leaderboard(); 
        // Obtiene el email del jugador guardado en las preferencias.
        string playerEmail = PlayerPrefs.GetString("Email");
        Debug.Log(playerEmail);
        
        // Consulta la tabla del leaderboard para verificar si existe un registro con el email del jugador.
        _ = leaderboard
            .SelectOne(leaderboard.C.email, leaderboard.C.score)
            .Where(leaderboard.C.email.Equal(playerEmail))
            .Run((UniDB.Trivia.Leaderboard.Record record, Info info) =>
            {
                if (info.isOK)
                {
                    if (info.hasData && record != null)
                    {
                        // Actualiza la puntuación máxima guardada.
                        PlayerPrefs.SetFloat("HighScore", record.score); 
                        Debug.Log("Usuario encontrado en la base de datos, puntuación: " + record.score);
                    }
                    else
                    {
                        // Si el jugador no está en la tabla, se inserta un nuevo registro con 0 puntos.
                        InsertPlayerRecord(playerEmail);
                        PlayerPrefs.SetFloat("HighScore", 0);
                    }
                }
                else
                {
                    // Reporta un error si la consulta falla.
                    Debug.LogError("Error consultando al jugador: " + info.error); 
                }
            });
    }

    /// <summary>
    /// Inserta un nuevo registro en el leaderboard para un jugador con 0 puntos.
    /// </summary>
    /// <param name="email">Email del jugador a insertar</param>
    private void InsertPlayerRecord(string email)
    {
        // Accede a la tabla del leaderboard.
        var leaderboardTable = triviaDB.GetTable_Leaderboard(); 

        // Inserta un nuevo registro con el email del jugador y una puntuación de 0.
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
                    // Informa que el registro se ha insertado correctamente.
                    Debug.Log("Insertado nuevo registro en leaderboard para email: " + email); 
                }
                else
                {
                    // Reporta un error si la inserción falla.
                    Debug.LogWarning("Fallo intentando insertar email: " + info.error); 
                }
            });
    }
}
