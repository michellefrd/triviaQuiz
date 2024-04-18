using System.Collections;
using System.Collections.Generic;
using TigerForge.UniDB;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    [Header("Game objects")]
    [SerializeField] private TMP_InputField emailInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    private UniDB.Trivia triviaDB;

    // Color original del placeholder
    private Color originalPlaceholderColor = new Color(120f / 255f, 192f / 255f, 255f / 255f);

    void Start()
    {
        triviaDB = new UniDB.Trivia();

        // Suscribir el método para restablecer el color de los campos al evento de cambio de texto
        emailInputField.onValueChanged.AddListener(delegate { ResetInputFieldColor(emailInputField); });
        passwordInputField.onValueChanged.AddListener(delegate { ResetInputFieldColor(passwordInputField); });
    }

    private void ResetInputFieldColor(TMP_InputField inputField)
    {
        inputField.textComponent.color = Color.black; // Restablecer el color del texto
        inputField.placeholder.color = originalPlaceholderColor; // Restablecer el color del placeholder
    }

    public void OnClickLogin()
    {
        if (string.IsNullOrWhiteSpace(emailInputField.text) || string.IsNullOrWhiteSpace(passwordInputField.text))
        {
            if (string.IsNullOrWhiteSpace(emailInputField.text))
            {
                emailInputField.placeholder.color = Color.red;
            }

            if (string.IsNullOrWhiteSpace(passwordInputField.text))
            {
                passwordInputField.placeholder.color = Color.red;
            }

            Debug.LogWarning("You must fill in all fields!");
            return;
        }

        var users = triviaDB.GetTable_Users();
        _ = users
            .SelectOne()
            .Where(users.C.email.Equal(emailInputField.text))
            .Run(
                (UniDB.Trivia.Users.Record d, Info info) =>
                {
                    if (info.isOK)
                    {
                        if (info.hasData && d != null)
                        {
                            if (d.password == passwordInputField.text)
                            {
                                Debug.Log("Login successful: " + d.name + " " + d.last_name);
                                PlayerPrefs.SetString("Email", d.email);
                                PlayerPrefs.Save();
                                SceneManager.LoadScene("CS_StartMenu");
                            }
                            else
                            {
                                Debug.LogWarning("Incorrect password!");
                                passwordInputField.textComponent.color = Color.red;
                                passwordInputField.placeholder.color = Color.red;
                            }
                        }
                        else
                        {
                            Debug.LogWarning("No user found with that email or user data is null!");
                            emailInputField.textComponent.color = Color.red;
                            emailInputField.placeholder.color = Color.red;
                        }
                    }
                    else
                    {
                        Debug.LogError("Database error: " + info.error);
                    }
                }
            );
    }

    public void OnClickSignUp()
    {
        SceneManager.LoadScene("SignUpScene");
    }
}
