using System.Collections;
using System.Collections.Generic;
using TigerForge.UniDB;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SignUpManager : MonoBehaviour
{
    [Header("Game objects")] 
    [SerializeField] private TMP_InputField emailInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    private UniDB.Trivia triviaDB;
    
    // Start is called before the first frame update
    void Start()
    {
        triviaDB = new UniDB.Trivia();
    }
    
    // Update is called once per frame
    public void OnClickSignUp()
    {
        var users = triviaDB.GetTable_Users();
    
        _ = users
            .SelectOne()
            .Where(users.C.email.Equal(emailInputField.text))
            .Run(
                (UniDB.Trivia.Users.Record d, Info info) =>
                {
                    if (info.isOK)
                    {
                        if (info.hasData && d != null)  // Asegúrate de que d no es null
                        {
                            // Verificar si la contraseña es correcta
                            if (d.password == passwordInputField.text)
                            {
                                Debug.Log("Login successful: " + d.name + " " + d.last_name);
                                // Opcional: Guardar el nombre del usuario en PlayerPrefs a
                                PlayerPrefs.SetString("Email", d.email);
                                PlayerPrefs.Save();
                            }
                            else
                            {
                                Debug.LogWarning("Incorrect password!");
                            }
                        }
                        else
                        {
                            Debug.LogWarning("No user found with that email or user data is null!");
                        }
                    }
                    else
                    {
                        Debug.LogError("Database error: " + info.error);
                    }
                }
            );
    }

    
    public void OnClickGoBack()
    {
        SceneManager.LoadScene("LoginScene");
    }
}
