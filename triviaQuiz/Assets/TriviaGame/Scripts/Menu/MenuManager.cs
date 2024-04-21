using System.Collections;
using System.Collections.Generic;
using TigerForge.UniDB;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    private UniDB.Trivia triviaDB;
    public Transform categoryButtonParent; // Parent object to hold instantiated buttons
    public GameObject categoryButtonPrefab; // Prefab for category buttons

    // Start is called before the first frame update
    void Start()
    {
        triviaDB = new UniDB.Trivia();
        OnLoadCategories();
        CheckPlayerScore();
    }

    private void OnLoadCategories()
    {
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
                            GameObject newButton = Instantiate(categoryButtonPrefab, categoryButtonParent);
                            newButton.name = "CategoryButton_" + d.name; // Naming the button for easier identification
                            CategoryButton buttonScript = newButton.GetComponent<CategoryButton>();
                            if (buttonScript != null)
                            {
                                buttonScript.categoryNameTxt.text = d.name;
                                buttonScript.categoryID = (int)d.code;
                            }
                            else
                            {
                                Debug.LogError("CategoryButton script not found on the instantiated prefab!");
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("Error: " + info.error);
                    }
                }
            );
    }

    private void CheckPlayerScore()
    {
        var leaderboard = triviaDB.GetTable_Leaderboard();
        string playerEmail = PlayerPrefs.GetString("Email"); // Obtener el email del jugador actual

        // Consultar la tabla leaderboard para verificar si existe el email
        _ = leaderboard
            .SelectOne(leaderboard.C.email, leaderboard.C.score) // Ensure to select the score as well
            .Where(leaderboard.C.email.Equal(playerEmail))
            .Run((UniDB.Trivia.Leaderboard.Record record, Info info) =>
            {
                if (info.isOK)
                {
                    if (info.hasData && record != null)
                    {
                        PlayerPrefs.SetFloat("HighScore", record.score); 
                        Debug.Log("Player already in leaderboard with score: " + record.score);
                    }
                    else
                    {
                        // Si no se encuentra el email, insertar el registro con 0 puntos
                        InsertPlayerRecord(playerEmail);
                        PlayerPrefs.SetFloat("HighScore", 0);
                    }
                }
                else
                {
                    Debug.LogError("Error checking player score: " + info.error);
                }
            });
    }

private void InsertPlayerRecord(string email)
{
    var leaderboardTable = triviaDB.GetTable_Leaderboard();

    // Insertar el nuevo registro en la tabla del leaderboard con 0 puntos
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
            }
            else
            {
                Debug.LogWarning("Failed to insert new leaderboard record: " + info.error);
            }
        });
}
}