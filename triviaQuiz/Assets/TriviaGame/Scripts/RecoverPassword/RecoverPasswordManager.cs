using System.Net;
using System.Net.Mail;
using TigerForge.UniDB;
using TMPro;
using ToastForUnity.Script.Core;
using ToastForUnity.Script.Enum;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

/// <summary>
/// Gestiona la recuperación de contraseña, incluyendo verificación de correo y actualización de contraseña.
/// </summary>
public class RecoverPasswordManager : MonoBehaviour
{
    [Header("InputFields: enviar código")]
    // Campo para ingresar el email
    [SerializeField]
    private TMP_InputField emailInputField;

    [Header("InputFields: cambiar contraseña")]
    // Campo para ingresar el código de verificación
    [SerializeField] private TMP_InputField codeInputField;

    // Campo para ingresar la nueva contraseña
    [SerializeField] private TMP_InputField passwordInputField;

    // Campo para confirmar la nueva contraseña
    [SerializeField] private TMP_InputField confirmPasswordInputField;

    // Campo para mostrar el email al cambiar contraseña
    [SerializeField] private TMP_InputField emailToChangeInputField;

    // Formulario para enviar el email
    [SerializeField] private GameObject sendEmailForm;

    // Formulario para cambiar la contraseña
    [SerializeField] private GameObject changePasswordForm;

    [Header("Información")]
    // Objeto padre para los mensajes emergentes (toasts)
    [SerializeField] private Transform toastParent;

    // Base de datos
    private UniDB.Trivia triviaDB;

    // Color original de los textos de entrada
    private Color originalInputColor = Color.black;

    // Color original de los placeholders
    private Color originaPlaceholderColor = new Color(120f / 255f, 192f / 255f, 255f / 255f);

    // Color para indicar errores
    private Color errorColor = Color.red;

    void Start()
    {
        triviaDB = new UniDB.Trivia();
        // Activa el formulario de envío de email
        sendEmailForm.SetActive(true); 
        // Desactiva el formulario de cambio de contraseña
        changePasswordForm.SetActive(false);
        // Suscribe los campos para restablecer colores
        SubscribeInputFields(); 
    }

    /// <summary>
    /// Suscribe los campos de entrada a un método que restablece su color cuando cambian.
    /// </summary>
    private void SubscribeInputFields()
    {
        emailInputField.onValueChanged.AddListener(delegate { ResetInputFieldColor(emailInputField); });
        codeInputField.onValueChanged.AddListener(delegate { ResetInputFieldColor(codeInputField); });
        passwordInputField.onValueChanged.AddListener(delegate { ResetInputFieldColor(passwordInputField); });
        confirmPasswordInputField.onValueChanged.AddListener(delegate
        {
            ResetInputFieldColor(confirmPasswordInputField);
        });
    }

    /// <summary>
    /// Restablece el color del texto y del placeholder de un campo de entrada.
    /// </summary>
    /// <param name="inputField">InputField afectado</param>
    private void ResetInputFieldColor(TMP_InputField inputField)
    {
        inputField.textComponent.color = originalInputColor;
        inputField.placeholder.color = originaPlaceholderColor;
    }

    /// <summary>
    /// Verifica la existencia del email y si existe, inicia el proceso para enviar un código de verificación.
    /// </summary>
    public void VerifyEmail()
    {
        if (string.IsNullOrWhiteSpace(emailInputField.text))
        {
            Debug.LogWarning("Email requerido.");
            emailInputField.textComponent.color = errorColor;
            Toast.PopOut("Email requerido", ToastStatus.Danger, toastParent);
            return;
        }

        var users = triviaDB.GetTable_Users();
        _ = users
            .SelectOne()
            .Where(users.C.email.Equal(emailInputField.text))
            .Run(
                (UniDB.Trivia.Users.Record d, Info info) =>
                {
                    if (info.isOK && d != null)
                    {
                        GenerateAndSendCode(d.email);
                    }
                    else
                    {
                        Debug.LogWarning("Email no encontrado");
                        emailInputField.textComponent.color = errorColor;
                        Toast.PopOut("Usuario no encontrado", ToastStatus.Danger, toastParent);
                    }
                }
            );
    }

    /// <summary>
    /// Genera un código aleatorio y lo envía al email proporcionado.
    /// </summary>
    /// <param name="email">Email al que enviar el código</param>
    public void GenerateAndSendCode(string email)
    {
        int code = Random.Range(1000, 9999); // Genera un código entre 1000 y 9999
        SendEmail(email, code);
        UpdateUserCode(email, code);
    }

    /// <summary>
    /// Actualiza el código de verificación en la base de datos para el usuario correspondiente.
    /// </summary>
    /// <param name="email">Email al que actualizar código</param>
    /// <param name="code">Código nuevo</param>
    public void UpdateUserCode(string email, int code)
    {
        var users = triviaDB.GetTable_Users();
        _ = users
            .Update()
            .Data(
                users.C.code.Value(code)
            )
            .Where(users.C.email.Equal(email))
            .Run(
                (Info info) =>
                {
                    if (info.isOK)
                    {
                        Debug.Log("Código actualizado. Filas afectadas: " + info.affectedRows);
                    }
                    else
                    {
                        Debug.LogError("Error actualizando el código: " + info.error);
                    }
                }
            );
    }

    /// <summary>
    /// Envía un email con el código de verificación al usuario.
    /// </summary>
    /// <param name="recipientEmail">Email del usuario</param>
    /// <param name="verificationCode">Código de verificación</param>
    public void SendEmail(string recipientEmail, int verificationCode)
    {
        // Email del remitente
        string senderEmail = "codcruzadosupp@gmail.com"; 
        // Contraseña del remitente
        string senderPassword = "gcbw hvrx tjrn rzdq"; 

        SmtpClient client = new SmtpClient("smtp.gmail.com", 587)
        {
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(senderEmail, senderPassword)
        };

        MailMessage mailMessage = new MailMessage
        {
            From = new MailAddress(senderEmail),
            Subject = "Recuperación de Contraseña",
            Body = $"Tu código de verificación es: {verificationCode}",
            IsBodyHtml = true,
        };
        mailMessage.To.Add(recipientEmail);

        try
        {
            client.Send(mailMessage);
            Debug.Log("Correo enviado exitosamente.");
            Toast.PopOut("Código enviado", ToastStatus.Success, toastParent);
            sendEmailForm.SetActive(false);
            changePasswordForm.SetActive(true);
            emailToChangeInputField.text = recipientEmail;
        }
        catch (System.Exception e)
        {
            Debug.LogError("No se pudo enviar el correo. Error: " + e.Message);
            Toast.PopOut("No se pudo enviar el código", ToastStatus.Danger, toastParent);
        }
    }

    /// <summary>
    /// Verifica el código ingresado y, si es correcto, procede a actualizar la contraseña.
    /// </summary>
    public void VerifyCodeAndUpdatePassword()
    {
        if (string.IsNullOrWhiteSpace(codeInputField.text) || string.IsNullOrWhiteSpace(passwordInputField.text) ||
            string.IsNullOrWhiteSpace(confirmPasswordInputField.text))
        {
            Debug.LogWarning("Campos vacíos");
            Toast.PopOut("Debes llenar todos los campos", ToastStatus.Danger, toastParent);
            if (string.IsNullOrWhiteSpace(codeInputField.text)) codeInputField.textComponent.color = errorColor;
            if (string.IsNullOrWhiteSpace(passwordInputField.text)) passwordInputField.textComponent.color = errorColor;
            if (string.IsNullOrWhiteSpace(confirmPasswordInputField.text))
                confirmPasswordInputField.textComponent.color = errorColor;
            return;
        }

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
            .WhereAND(users.C.email.Equal(emailToChangeInputField.text),
                users.C.code.Equal(int.Parse(codeInputField.text)))
            .Run(
                (UniDB.Trivia.Users.Record d, Info info) =>
                {
                    if (info.isOK && d != null)
                    {
                        UpdateUserPassword(emailToChangeInputField.text, passwordInputField.text);
                    }
                    else
                    {
                        Debug.LogWarning("Código inválido!");
                        Toast.PopOut("Código inválido", ToastStatus.Danger, toastParent);
                        codeInputField.textComponent.color = errorColor;
                    }
                }
            );
    }

    /// <summary>
    /// Actualiza la contraseña del usuario en la base de datos.
    /// </summary>
    public void UpdateUserPassword(string email, string newPassword)
    {
        var hashedPassword = Helpers.HashPassword(newPassword); // Hashear la nueva contraseña
        var users = triviaDB.GetTable_Users();
        _ = users
            .Update()
            .Data(
                users.C.password.Value(hashedPassword) // Usar la contraseña hasheada
            )
            .Where(users.C.email.Equal(email))
            .Run(
                (Info info) =>
                {
                    if (info.isOK)
                    {
                        Debug.Log("Contraseña actualizada. Filas afectadas: " + info.affectedRows);
                        Toast.PopOut("Contraseña actualizada", ToastStatus.Success, toastParent);
                        OnLoadLogin();
                    }
                    else
                    {
                        Toast.PopOut("Error actualizando contraseña", ToastStatus.Danger, toastParent);
                        Debug.LogError("Error actualizando contraseña: " + info.error);
                    }
                }
            );
    }

    /// <summary>
    /// Carga la escena de inicio de sesión.
    /// </summary>
    public void OnLoadLogin()
    {
        SceneManager.LoadScene("LoginScene");
    }
}