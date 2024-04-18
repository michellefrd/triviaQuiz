using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using TigerForge.UniDB;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RecoverPasswordManager : MonoBehaviour
{
    [Header("Game objects")]
    [SerializeField] private TMP_InputField emailInputField;
    [SerializeField] private TMP_InputField codeInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private TMP_InputField confirmPasswordInputField;
    [SerializeField] private GameObject sendEmailForm;
    [SerializeField] private GameObject changePasswordForm;
    private UniDB.Trivia triviaDB;
    void Start()
    {
        sendEmailForm.SetActive(true);
        changePasswordForm.SetActive(false);
        triviaDB = new UniDB.Trivia();
    }
    
    public void VerifyEmail()
    {
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
                    }
                }
            );
    }
    
    public void GenerateAndSendCode(string email)
    {
        int code = Random.Range(1000, 9999);

        SendEmail(email,code);
        
        UpdateUserCode(email, code);
    }
    
    public void SendEmail(string recipientEmail, int verificationCode)
    {
        string senderEmail = "codcruzadosupp@gmail.com";
        string senderPassword = "gcbw hvrx tjrn rzdq";  // Asegúrate de usar aquí tu contraseña de aplicación

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
            sendEmailForm.SetActive(false);
            changePasswordForm.SetActive(true);
        }
        catch (System.Exception e)
        {
            Debug.LogError("No se pudo enviar el correo. Error: " + e.Message);
        }
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
    
    public void VerifyCodeAndUpdatePassword()
    {
        var users = triviaDB.GetTable_Users();
        _ = users
            .SelectOne()
            .WhereAND(users.C.email.Equal(emailInputField.text), users.C.code.Equal(int.Parse(codeInputField.text)))
            .Run(
                (UniDB.Trivia.Users.Record d, Info info) =>
                {
                    if (info.isOK && d != null)
                    {
                        UpdateUserPassword(emailInputField.text, passwordInputField.text);
                    }
                    else
                    {
                        Debug.LogWarning("Invalid code!");
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
                        OnLoadLogin();
                    }
                    else
                    {
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
