using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using TigerForge.UniDB;
using ToastForUnity.Script.Core;
using ToastForUnity.Script.Enum;

public class SignUpManager : MonoBehaviour
{
    [Header("Game objects")]
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TMP_InputField lastNameInputField;
    [SerializeField] private TMP_InputField emailInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private TMP_InputField confirmPasswordInputField;
    [SerializeField] private Transform toastParent;

    private UniDB.Trivia triviaDB;
    private Color originalInputColor = Color.black;  // El color original de los campos de entrada
    private Color originaPlaceholderColor = new Color(120f / 255f, 192f / 255f, 255f / 255f);
    private Color errorColor = Color.red;  // Color para errores de entrada

    void Start()
    {
        triviaDB = new UniDB.Trivia();
        SubscribeInputFields();  // Suscribir los campos de entrada para resetear el color cuando cambian
    }

    private void SubscribeInputFields()
    {
        nameInputField.onValueChanged.AddListener(delegate { ResetInputFieldColor(nameInputField); });
        lastNameInputField.onValueChanged.AddListener(delegate { ResetInputFieldColor(lastNameInputField); });
        emailInputField.onValueChanged.AddListener(delegate { ResetInputFieldColor(emailInputField); });
        passwordInputField.onValueChanged.AddListener(delegate { ResetInputFieldColor(passwordInputField); });
        confirmPasswordInputField.onValueChanged.AddListener(delegate { ResetInputFieldColor(confirmPasswordInputField); });
    }

    private void ResetInputFieldColor(TMP_InputField inputField)
    {
        inputField.textComponent.color = originalInputColor;  // Restablecer el color del texto
        inputField.placeholder.color = originaPlaceholderColor;  // Restablecer el color del placeholder
    }

    public void OnClickRegister()
    {
        if (!IsInputValid())
        {
            return;
        }

        if (passwordInputField.text != confirmPasswordInputField.text)
        {
            Debug.LogWarning("Passwords do not match!");
            Toast.PopOut("Contraseñas no coinciden", ToastStatus.Danger,toastParent );
            passwordInputField.textComponent.color = errorColor;
            confirmPasswordInputField.textComponent.color = errorColor;
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
                            Debug.LogWarning("User already exists!");
                            Toast.PopOut("El usuario ya existe", ToastStatus.Warning,toastParent );
                            emailInputField.textComponent.color = errorColor;
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
                        SceneManager.LoadScene("LoginScene");
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
        bool isValid = true;
        
        if (string.IsNullOrWhiteSpace(nameInputField.text))
        {
            Debug.LogWarning("Name is required.");
            Toast.PopOut("Debes rellenar todos los campos", ToastStatus.Danger,toastParent );
            nameInputField.placeholder.color = errorColor;
            isValid = false;
        }
        if (string.IsNullOrWhiteSpace(lastNameInputField.text))
        {
            Debug.LogWarning("Last name is required.");
            Toast.PopOut("Debes rellenar todos los campos", ToastStatus.Danger,toastParent );
            
            lastNameInputField.placeholder.color = errorColor;
            isValid = false;
        }
        if (string.IsNullOrWhiteSpace(emailInputField.text))
        {
            Debug.LogWarning("Email is required.");
            Toast.PopOut("Debes rellenar todos los campos", ToastStatus.Danger,toastParent );
          
            emailInputField.placeholder.color = errorColor;
            isValid = false;
        }
        else if (!new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$").IsMatch(emailInputField.text))
        {
            Debug.LogWarning("Email format is incorrect.");
            Toast.PopOut("Formato incorrecto", ToastStatus.Warning,toastParent );
            emailInputField.textComponent.color = errorColor;
            isValid = false;
        }
        if (string.IsNullOrWhiteSpace(passwordInputField.text))
        {
            Debug.LogWarning("Password is required.");
            Toast.PopOut("Debes rellenar todos los campos", ToastStatus.Danger,toastParent );
            passwordInputField.placeholder.color = errorColor;
            
            isValid = false;
        }
        if (string.IsNullOrWhiteSpace(confirmPasswordInputField.text))
        {
            Debug.LogWarning("Confirm password is required.");
            Toast.PopOut("Debes rellenar todos los campos", ToastStatus.Danger,toastParent );
            confirmPasswordInputField.placeholder.color = errorColor;
            isValid = false;
        }
        
        return isValid;
    }

    public void OnClickGoBack()
    {
        SceneManager.LoadScene("LoginScene");
    }
}