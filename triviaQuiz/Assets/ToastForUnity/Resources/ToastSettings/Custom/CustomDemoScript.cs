using ToastForUnity.Script.Core;
using UnityEngine;

public class CustomDemoScript : MonoBehaviour
{
    public void CallCustomToast()
    {
        Toast.PopOut<CustomToastView>("MyCustomToast", new CustomModel()
        {
            Content = "New Custom Toast"
        });
    }
}
