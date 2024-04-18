using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TigerForge.UniDB;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RegisterManager : MonoBehaviour
{
    [Header("Game objects")]
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TMP_InputField lastNameInputField;
    [SerializeField] private TMP_InputField emailInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private TMP_InputField confirmPasswordInputField;
    private UniDB.Trivia triviaDB;

    void Start()
    {
        triviaDB = new UniDB.Trivia();
    }

    public void OnClickRegister()
    {
        if (!IsInputValid())
        {
            Debug.LogWarning("Please check your input and fill all fields correctly!");
            return;
        }

        if (passwordInputField.text != confirmPasswordInputField.text)
        {
            Debug.LogWarning("Passwords do not match!");
            passwordInputField.textComponent.color = Color.red;
            confirmPasswordInputField.textComponent.color = Color.red;
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
                        if (d != null && info.hasData)
                        {
                            Debug.Log(emailInputField.text);
                            Debug.LogWarning("User already exists!");
                            emailInputField.textComponent.color = Color.red;
                        }
                        else
                        {
                            RegisterNewUser();
                        }
                    }
                    else
                    {
                        Debug.LogError("Database error: " + info.error);
                    }
                }
            );
    }


    private void RegisterNewUser()
    {
        var users = triviaDB.GetTable_Users();
        _ = users
            .Insert()
            .Data(
                users.C.email.Value(emailInputField.text),
                users.C.password.Value(passwordInputField.text),
                users.C.name.Value(nameInputField.text),
                users.C.last_name.Value(lastNameInputField.text)
            )
            .Run(
                (Info info) =>
                {
                    if (info.isOK)
                    {
                        Debug.Log("Registration successful! Inserted record ID: " + info.id);
                        // Aquí podrías redirigir a otra escena o limpiar los campos
                    }
                    else
                    {
                        Debug.LogError("Registration failed: " + info.error);
                    }
                }
            );
    }

    private bool IsInputValid()
    {
        Regex emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        if (string.IsNullOrWhiteSpace(nameInputField.text) || string.IsNullOrWhiteSpace(lastNameInputField.text) ||
            string.IsNullOrWhiteSpace(emailInputField.text) || string.IsNullOrWhiteSpace(passwordInputField.text) ||
            string.IsNullOrWhiteSpace(confirmPasswordInputField.text))
        {
            return false;
        }
        if (!emailRegex.IsMatch(emailInputField.text))
        {
            emailInputField.textComponent.color = Color.red;
            return false;
        }
        return true;
    }
    
    public void OnClickGoBack()
    {
        SceneManager.LoadScene("LoginScene");
    }
}
