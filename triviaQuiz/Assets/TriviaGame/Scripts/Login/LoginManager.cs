
using TigerForge.UniDB;
using UnityEngine;
using TMPro;
using ToastForUnity.Script.Core;
using ToastForUnity.Script.Enum;
using UnityEngine.SceneManagement;

/// <summary>
/// Este script controla la lógica de iniciar sesión en la aplicación
/// </summary>
public class LoginManager : MonoBehaviour
{
    [Header("InputFields")]
    // Campo de entrada para el correo electrónico
    [SerializeField]
    private TMP_InputField emailInputField;

    // Campo de entrada para la contraseña
    [SerializeField] private TMP_InputField passwordInputField;

    [Header("Toast de información")]
    // Objeto padre para los mensajes emergentes (toasts)
    [SerializeField]
    private Transform toastParent;

    // Base de datos Trivia de UniDB (conexión)
    private UniDB.Trivia triviaDB;

    // Color original del texto del placeholder
    private Color originalPlaceholderColor = new Color(120f / 255f, 192f / 255f, 255f / 255f);

    /// <summary>
    /// Start solo se llama una vez durante la vida del script.
    /// La diferencia entre Awake y Start es que Start solo se llama si la instancia del script está habilitada.
    /// Esto le permite retrasar cualquier código de inicialización hasta que sea realmente necesario.
    /// Awake siempre se llama antes de cualquier función de inicio.
    /// Esto le permite ordenar la inicialización de scripts
    /// </summary>
    void Start()
    {
        // Inicializa la base de datos Trivia
        triviaDB = new UniDB.Trivia();
        //Eliminar todas las preferencias del jugador guardadas en el dispositivo
        PlayerPrefs.DeleteAll();

        // Suscribir el método para restablecer el color de los campos al evento de cambio de texto
        emailInputField.onValueChanged.AddListener(delegate { ResetInputFieldColor(emailInputField); });
        passwordInputField.onValueChanged.AddListener(delegate { ResetInputFieldColor(passwordInputField); });
    }
    
    /// <summary>
    /// Este método cambia el color de los componentes del inputField pasado
    /// </summary>
    /// <param name="inputField">Input field a cambiar</param>
    private void ResetInputFieldColor(TMP_InputField inputField)
    {
        inputField.textComponent.color = Color.black; // Restablecer el color del texto del campo
        inputField.placeholder.color = originalPlaceholderColor; // Restablecer el color del placeholder
    }

    /// <summary>
    /// Este método es llamado desde el botón de inicio de sesión
    /// Comprueba que los campos esten llenos y que el usuario y contraseña coincidan con los de la base de datos
    /// </summary>
    public void OnClickLogin()
    {
        // Verificación de que los campos no esten vacíos
        if (string.IsNullOrWhiteSpace(emailInputField.text) || string.IsNullOrWhiteSpace(passwordInputField.text))
        {
            if (string.IsNullOrWhiteSpace(emailInputField.text))
            {
                // Cambiar color a rojo si el campo está vacío
                emailInputField.placeholder.color = Color.red; 
            }

            if (string.IsNullOrWhiteSpace(passwordInputField.text))
            {
                // Cambiar color a rojo si el campo está vacío
                passwordInputField.placeholder.color = Color.red; 
            }
            // Mostrar mensaje de error
            Toast.PopOut("Debes llenar todos los campos", ToastStatus.Danger, toastParent); 
            Debug.LogWarning("Debes llenar todos los campos");
            return;
        }

        //Instancia de la tabla usuarios
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
                            // Hash de la contraseña ingresada
                            var hashedInputPassword =
                                Helpers.HashPassword(passwordInputField.text); 
                            if (d.password == hashedInputPassword)
                            {
                                Debug.Log("Login correcto: " + d.name + " " + d.last_name);
                                // Guardar el correo en player prefs
                                PlayerPrefs.SetString("Email", d.email); 
                                // Guardar las preferencias
                                PlayerPrefs.Save(); 
                                // Cargar la siguiente escena (Menu)
                                SceneManager.LoadScene("MenuScene"); 
                            }
                            else
                            {
                                // Mostrar mensaje de contraseña incorrecta
                                Toast.PopOut("Contraseña incorrecta", ToastStatus.Danger,
                                    toastParent); 
                                Debug.LogWarning("Contraseña incorrecta");
                                // Cambiar color de texto a rojo
                                passwordInputField.textComponent.color = Color.red;
                            }
                        }
                        else
                        {
                            // Mostrar mensaje de usuario no encontrado
                            Toast.PopOut("Usuario no encontrado", ToastStatus.Danger,
                                toastParent); 
                            Debug.LogWarning("Usuario no encontrado");
                            emailInputField.textComponent.color = Color.red;
                        }
                    }
                    else
                    {
                        // Error de base de datos
                        Debug.LogError("Error en la base de datos: " + info.error); 
                        // Mostrar error de base de datos
                        Toast.PopOut("info.error", ToastStatus.Danger, toastParent); 
                    }
                }
            );
    }

    /// <summary>
    /// Este método se encarga de carga la escena del string que se pasa
    /// </summary>
    /// <param name="scene">Nombre de la escena a cargar</param>
    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

}