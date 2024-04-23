using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Clase que contiene métodos que se utilizan múltiples veces
/// </summary>
public static class Helpers
{
    /// <summary>
    /// Método utilizado para hashear la contraseña
    /// </summary>
    /// <param name="password">Contraseña a hashear</param>
    public static string HashPassword(string password)
    {
        using (var sha256 = System.Security.Cryptography.SHA256.Create())
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash); // Convertir el hash a string para almacenamiento
        }
    }
   
}
