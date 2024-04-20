using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using TigerForge.UniDB;
using TMPro;
using ToastForUnity.Script.Core;
using ToastForUnity.Script.Enum;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RecoverPasswordManager : MonoBehaviour
{
    [Header("Game objects")]
    [SerializeField] private TMP_InputField emailInputField;
    [SerializeField] private TMP_InputField codeInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private TMP_InputField confirmPasswordInputField;
    [SerializeField] private TMP_InputField emailToChangeInputField;
    [SerializeField] private GameObject sendEmailForm;
    [SerializeField] private GameObject changePasswordForm;
    [SerializeField] private Transform toastParent;
    private UniDB.Trivia triviaDB;
    private Color originalInputColor = Color.black;  // Color original de los campos de entrada
    private Color originaPlaceholderColor = new Color(120f / 255f, 192f / 255f, 255f / 255f);
    private Color errorColor = Color.red;  // Color para indicar error

    void Start()
    {
        triviaDB = new UniDB.Trivia();
        sendEmailForm.SetActive(true);
        changePasswordForm.SetActive(false);
        SubscribeInputFields();  // Suscribir campos para resetear colores
    }

    private void SubscribeInputFields()
    {
        emailInputField.onValueChanged.AddListener(delegate { ResetInputFieldColor(emailInputField); });
        codeInputField.onValueChanged.AddListener(delegate { ResetInputFieldColor(codeInputField); });
        passwordInputField.onValueChanged.AddListener(delegate { ResetInputFieldColor(passwordInputField); });
        confirmPasswordInputField.onValueChanged.AddListener(delegate { ResetInputFieldColor(confirmPasswordInputField); });
    }

    private void ResetInputFieldColor(TMP_InputField inputField)
    {
        inputField.textComponent.color = originalInputColor;
        inputField.placeholder.color = originaPlaceholderColor; 
    }

    public void VerifyEmail()
    {
        if (string.IsNullOrWhiteSpace(emailInputField.text))
        {
            Debug.LogWarning("Email is required.");
            emailInputField.textComponent.color = errorColor;
            Toast.PopOut("Email requerido", ToastStatus.Danger,toastParent );
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
                        Debug.LogWarning("Email not found!");
                        emailInputField.textComponent.color = errorColor;
                        Toast.PopOut("Usuario no encontrado", ToastStatus.Danger,toastParent );
                    }
                }
            );
    }

    public void GenerateAndSendCode(string email)
    {
        int code = Random.Range(1000, 9999);
        SendEmail(email, code);
        UpdateUserCode(email, code);
    }

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
                        
                        Debug.Log("Code updated in the database. Affected rows: " + info.affectedRows);
                    }
                    else
                    {
                        Debug.LogError("Failed to update code: " + info.error);
                    }
                }
            );
    }
    public void SendEmail(string recipientEmail, int verificationCode)
    {
        string senderEmail = "codcruzadosupp@gmail.com";
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
            Toast.PopOut("Código enviado", ToastStatus.Success,toastParent );
            sendEmailForm.SetActive(false);
            changePasswordForm.SetActive(true);
            emailToChangeInputField.text = recipientEmail;
        }
        catch (System.Exception e)
        {
            Debug.LogError("No se pudo enviar el correo. Error: " + e.Message);
            Toast.PopOut("No se pudo enviar el código", ToastStatus.Danger,toastParent );
        }
    }

    public void VerifyCodeAndUpdatePassword()
    {
        if (string.IsNullOrWhiteSpace(codeInputField.text) || string.IsNullOrWhiteSpace(passwordInputField.text) || string.IsNullOrWhiteSpace(confirmPasswordInputField.text))
        {
            Debug.LogWarning("All fields must be filled.");
            Toast.PopOut("Debes llenar todos los campos", ToastStatus.Danger,toastParent );
            if (string.IsNullOrWhiteSpace(codeInputField.text)) codeInputField.textComponent.color = errorColor;
            if (string.IsNullOrWhiteSpace(passwordInputField.text)) passwordInputField.textComponent.color = errorColor;
            if (string.IsNullOrWhiteSpace(confirmPasswordInputField.text)) confirmPasswordInputField.textComponent.color = errorColor;
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
            .WhereAND(users.C.email.Equal(emailToChangeInputField.text), users.C.code.Equal(int.Parse(codeInputField.text)))
            .Run(
                (UniDB.Trivia.Users.Record d, Info info) =>
                {
                    if (info.isOK && d != null)
                    {
                        UpdateUserPassword(emailToChangeInputField.text, passwordInputField.text);
                    }
                    else
                    {
                        Debug.LogWarning("Invalid code!");
                        Toast.PopOut("Código inválido", ToastStatus.Danger,toastParent );
                        codeInputField.textComponent.color = errorColor;
                    }
                }
            );
    }

    public void UpdateUserPassword(string email, string newPassword)
    {
        var users = triviaDB.GetTable_Users();
        _ = users
            .Update()
            .Data(
                users.C.password.Value(newPassword)
            )
            .Where(users.C.email.Equal(email))
            .Run(
                (Info info) =>
                {
                    if (info.isOK)
                    {
                        Debug.Log("Password updated successfully. Affected rows: " + info.affectedRows);
                        Toast.PopOut("Contraseña actualizada", ToastStatus.Success,toastParent );
                        OnLoadLogin();
                    }
                    else
                    {
                        Toast.PopOut("Error actualizando contraseña", ToastStatus.Danger, toastParent);
                        Debug.LogError("Failed to update password: " + info.error);
                    }
                }
            );
    }

    public void OnLoadLogin()
    {
        SceneManager.LoadScene("LoginScene");
    }
}