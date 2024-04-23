using TMPro;
using UnityEngine;

/// <summary>
/// Controlador para alternar la visibilidad de la contraseña en un campo de entrada.
/// </summary>
public class PasswordToggle : MonoBehaviour
{
    // Campo de entrada de contraseña.
    public TMP_InputField passwordInputField;

    // Imagen que indica que la contraseña es visible.
    public GameObject visibleImage;

    // Imagen que indica que la contraseña está oculta.

    public GameObject invisibleImage;
    
    // Indicador de si la contraseña está siendo mostrada o no.

    private bool isPasswordVisible = false;

    /// <summary>
    /// Alterna la visibilidad de la contraseña entre oculta y visible.
    /// </summary>
    public void TogglePasswordVisibility()
    {
        // Cambia el estado de visibilidad de la contraseña.
        isPasswordVisible = !isPasswordVisible;

        // Si la contraseña debe ser visible.
        if (isPasswordVisible)
        {
            // Establece el tipo de contenido y el tipo de entrada para mostrar la contraseña como texto normal.
            passwordInputField.contentType = TMP_InputField.ContentType.Standard;
            passwordInputField.inputType = TMP_InputField.InputType.Standard;

            // Activa la imagen que indica visibilidad y desactiva la que indica invisibilidad.
            visibleImage.SetActive(true);
            invisibleImage.SetActive(false);
        }
        else
        {
            // Establece el tipo de contenido y el tipo de entrada para ocultar la contraseña.
            passwordInputField.contentType = TMP_InputField.ContentType.Password;
            passwordInputField.inputType = TMP_InputField.InputType.Password;

            // Desactiva la imagen que indica visibilidad y activa la que indica invisibilidad.
            visibleImage.SetActive(false);
            invisibleImage.SetActive(true);
        }

        // Fuerza la actualización del etiquetado del campo de entrada para reflejar los cambios.
        passwordInputField.ForceLabelUpdate();
    }
}