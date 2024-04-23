using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using TigerForge.UniDB;
using ToastForUnity.Script.Core;
using ToastForUnity.Script.Enum;

/// <summary>
/// Gestiona la funcionalidad del registro de usuarios en una aplicación.
/// </summary>
public class SignUpManager : MonoBehaviour
{
    [Header("InputFields Formulario")] [SerializeField]
    private TMP_InputField nameInputField;

    [SerializeField] private TMP_InputField lastNameInputField;
    [SerializeField] private TMP_InputField emailInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private TMP_InputField confirmPasswordInputField;

    [Header("Notificaciones")] [SerializeField]
    private Transform toastParent;

    private UniDB.Trivia triviaDB;

    // El color original del texto de los campos de entrada
    private Color originalInputColor = Color.black;

    // El color original del placeholder
    private Color originaPlaceholderColor = new Color(120f / 255f, 192f / 255f, 255f / 255f);

    // Color utilizado para indicar errores en los campos de entrada
    private Color errorColor = Color.red;

    void Start()
    {
        triviaDB = new UniDB.Trivia();
        // Método para suscribir los campos de entrada a un listener que resetea el color cuando cambian
        SubscribeInputFields();
    }

    /// <summary>
    /// Suscribe cada campo de entrada a un evento que restablece su color al cambiar su contenido.
    /// </summary>
    private void SubscribeInputFields()
    {
        nameInputField.onValueChanged.AddListener(delegate { ResetInputFieldColor(nameInputField); });
        lastNameInputField.onValueChanged.AddListener(delegate { ResetInputFieldColor(lastNameInputField); });
        emailInputField.onValueChanged.AddListener(delegate { ResetInputFieldColor(emailInputField); });
        passwordInputField.onValueChanged.AddListener(delegate { ResetInputFieldColor(passwordInputField); });
        confirmPasswordInputField.onValueChanged.AddListener(delegate
        {
            ResetInputFieldColor(confirmPasswordInputField);
        });
    }

    /// <summary>
    /// Restablece el color del texto y del placeholder del campo de entrada especificado.
    /// </summary>
    private void ResetInputFieldColor(TMP_InputField inputField)
    {
        inputField.textComponent.color = originalInputColor;
        inputField.placeholder.color = originaPlaceholderColor;
    }

    /// <summary>
    /// Gestiona el evento de clic en el botón de registro, validando la entrada y procesando el registro.
    /// </summary>
    public void OnClickRegister()
    {
        //Llamar al método que verifica que los valores sean válidos
        if (!IsInputValid())
        {
            return;
        }

        //Comprobar que las contraseñas coincidan
        if (passwordInputField.text != confirmPasswordInputField.text)
        {
            Debug.LogWarning("Contraseñas no coinciden");
            Toast.PopOut("Contraseñas no coinciden", ToastStatus.Danger, toastParent);
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
                            Debug.LogWarning("El usuario ya existe");
                            Toast.PopOut("El usuario ya existe", ToastStatus.Warning, toastParent);
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

    /// <summary>
    /// Registra un nuevo usuario en la base de datos.
    /// </summary>
    private void RegisterNewUser()
    {
        var hashedPassword = Helpers.HashPassword(passwordInputField.text);

        var users = triviaDB.GetTable_Users();
        _ = users
            .Insert()
            .Data(
                users.C.email.Value(emailInputField.text),
                users.C.password.Value(hashedPassword),
                users.C.name.Value(nameInputField.text),
                users.C.last_name.Value(lastNameInputField.text)
            )
            .Run(
                (Info info) =>
                {
                    if (info.isOK)
                    {
                        Debug.Log("Registro correcto. INFO ID:  " + info.id);
                        // Guardar el correo en player prefs
                        PlayerPrefs.SetString("Email", emailInputField.text); 
                        // Guardar las preferencias
                        PlayerPrefs.Save(); 
                        // Cargar la siguiente escena (Menu)
                        SceneManager.LoadScene("MenuScene");
                    }
                    else
                    {
                        Debug.LogError("Error en el registro: " + info.error);
                    }
                }
            );
    }

    /// <summary>
    /// Verifica que toda la información requerida esté correctamente llenada y válida.
    /// </summary>
    private bool IsInputValid()
    {
        bool isValid = true;

        // Comprobaciones para cada campo requerido y validación de formato de email
        if (string.IsNullOrWhiteSpace(nameInputField.text))
        {
            Debug.LogWarning("Nombre requerido.");
            Toast.PopOut("Debes rellenar todos los campos", ToastStatus.Danger, toastParent);
            nameInputField.placeholder.color = errorColor;
            isValid = false;
        }

        if (string.IsNullOrWhiteSpace(lastNameInputField.text))
        {
            Debug.LogWarning("Apellido Requerido");
            Toast.PopOut("Debes rellenar todos los campos", ToastStatus.Danger, toastParent);
            lastNameInputField.placeholder.color = errorColor;
            isValid = false;
        }

        if (string.IsNullOrWhiteSpace(emailInputField.text))
        {
            Debug.LogWarning("Email requerido.");
            Toast.PopOut("Debes rellenar todos los campos", ToastStatus.Danger, toastParent);
            emailInputField.placeholder.color = errorColor;
            isValid = false;
        }
        else if (!new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$").IsMatch(emailInputField.text))
        {
            Debug.LogWarning("Formato de email incorrecto.");
            Toast.PopOut("Formato incorrecto", ToastStatus.Warning, toastParent);
            emailInputField.textComponent.color = errorColor;
            isValid = false;
        }

        if (string.IsNullOrWhiteSpace(passwordInputField.text))
        {
            Debug.LogWarning("Contraseña requerida.");
            Toast.PopOut("Debes rellenar todos los campos", ToastStatus.Danger, toastParent);
            passwordInputField.placeholder.color = errorColor;
            isValid = false;
        }

        if (string.IsNullOrWhiteSpace(confirmPasswordInputField.text))
        {
            Debug.LogWarning("Confirmar contraseña requerida.");
            Toast.PopOut("Debes rellenar todos los campos", ToastStatus.Danger, toastParent);
            confirmPasswordInputField.placeholder.color = errorColor;
            isValid = false;
        }

        return isValid;
    }

    /// <summary>
    /// Vuelve a la escena de inicio de sesión.
    /// </summary>
    public void OnClickGoBack()
    {
        SceneManager.LoadScene("LoginScene");
    }
}