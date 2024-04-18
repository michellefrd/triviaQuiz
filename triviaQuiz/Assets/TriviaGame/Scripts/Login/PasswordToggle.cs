using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PasswordToggle : MonoBehaviour
{
    public TMP_InputField passwordInputField;
    public Button toggleButton;
    public GameObject visibleImage;
    public GameObject invisibleImage;
    private bool isPasswordVisible = false;


    public void TogglePasswordVisibility()
    {
        isPasswordVisible = !isPasswordVisible;

        if (isPasswordVisible)
        {
            passwordInputField.contentType = TMP_InputField.ContentType.Standard;
            passwordInputField.inputType = TMP_InputField.InputType.Standard;
            visibleImage.SetActive(true);
            invisibleImage.SetActive(false);
        }
        else
        {
            passwordInputField.contentType = TMP_InputField.ContentType.Password;
            passwordInputField.inputType = TMP_InputField.InputType.Password;
            visibleImage.SetActive(false);
            invisibleImage.SetActive(true);
        }

        passwordInputField.ForceLabelUpdate();
    }
}