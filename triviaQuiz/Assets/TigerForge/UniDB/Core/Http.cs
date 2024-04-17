using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

namespace TigerForge.UniDB
{

    #region " HTTP ENGINE "

    public class HttpRequest
    {
        public enum Error
        {
            NONE,
            HTTP_ERROR,
            SYSTEM_ERROR,
            WORDPRESS_ERROR
        }

        /// <summary>
        /// True if the HTTP Request has been done correctly and there is a response (data).
        /// </summary>
        public bool isArrived = false;

        /// <summary>
        /// True if the HTTP Request is in error. Errors details are stored in the 'error' and 'errorType' properties.
        /// </summary>
        public bool hasError = false;

        /// <summary>
        /// The data string of the Server reply. Usually it's a JSON string that should be converted with JsonUtility.
        /// </summary>
        public string rawData = "";

        /// <summary>
        /// The HTTP response code.
        /// </summary>
        public long code = 0;

        /// <summary>
        /// The error message in case of HTTP failure. It can be both a Server message or a C# error.
        /// </summary>
        public string httpError = "";

        /// <summary>
        /// The kind of error: NONE default, HTTP_ERROR if Server error, SYSTEM_ERROR if C# error.
        /// </summary>
        public Error errorType = Error.NONE;

        /// <summary>
        /// Decode the data arrived from WordPress and convert it into an object of the given T type.
        /// <para>T : the data-type the WordPress reply has to be coverted to.</para>
        /// </summary>
        public T GetData<T>()
        {
            return JsonUtility.FromJson<T>(rawData);
        }

        /// <summary>
        /// The response from the API.
        /// </summary>
        public HTTP.Response response;
    }

    public class HTTP
    {
        public class Response
        {

            #region " Properties "
            /// <summary>
            /// The API data string.
            /// </summary>
            public string data = "";

            /// <summary>
            /// The API error message.
            /// </summary>
            public string error = "";

            /// <summary>
            /// The API status string.
            /// </summary>
            public string status = "";

            /// <summary>
            /// The API code string.
            /// </summary>
            public string code = "";

            /// <summary>
            /// The API query string.
            /// </summary>
            public string query = "";
            #endregion

            #region " Helpers "

            /// <summary>
            /// Return TRUE if the result is "OK".
            /// </summary>
            public bool IsOK()
            {
                return status == "OK";
            }

            /// <summary>
            /// Return TRUE if there is an error (result == 'ERROR' or error != '').
            /// </summary>
            public bool HasError()
            {
                return status == "ERROR" || error != "";
            }

            #endregion

            #region " Get Typed Data "

            /// <summary>
            /// Return the today date converted in a string suitable to be saved on Database (default format: 'yyyy-MM-dd H:mm:ss').
            /// <para>format (optional) : the format to use to get the string.</para>
            /// </summary>
            /// <returns></returns>
            public string GetDate(string format = "yyyy-MM-dd H:mm:ss")
            {
                return DateTime.Now.ToString(format);
            }

            /// <summary>
            /// Return the given date converted in a string suitable to be saved on Database (default format: 'yyyy-MM-dd H:mm:ss').
            /// <para>date : the date object to convert.</para>
            /// <para>format (optional) : the format to use to get the string.</para>
            /// </summary>
            /// <param name="date"></param>
            /// <param name="format"></param>
            /// <returns></returns>
            public string GetDate(DateTime date, string format = "yyyy-MM-dd H:mm:ss")
            {
                return date.ToString("yyyy-MM-dd H:mm:ss");
            }

            /// <summary>
            /// Return a DateTime object from the given date in string format.
            /// <para>date : the date as a string.</para>
            /// <para>cultureInfo (optional, 'en-US' by default): the country code the data is formatted to.</para>
            /// </summary>
            /// <param name="date"></param>
            /// <param name="cultureInfo"></param>
            /// <returns></returns>
            public DateTime GetDateFromString(string date, string cultureInfo = "en-US")
            {
                if (date == "") return default;

                try
                {
                    var ci = new System.Globalization.CultureInfo(cultureInfo);
                    return DateTime.Parse(date, ci);
                }
                catch (Exception)
                {
                    return default;
                }

            }

            /// <summary>
            /// Return the data in the given format.
            /// </summary>
            public T GetData<T>()
            {
                var obj = (object)data;
                if (obj is T)
                {
                    return (T)obj;
                }
                try
                {
                    return (T)Convert.ChangeType(obj, typeof(T));
                }
                catch (InvalidCastException e)
                {
                    Debug.LogWarning(e.Message + ": " + typeof(T));
                    return default(T);
                }
            }

            #endregion

        }

        public class PromiseData
        {
            public string type = "";
            public string url = "";
            public string data = "";
            public System.Action<HttpRequest> doneCallback = null;
            public System.Action<HttpRequest> errorCallback = null;
        }
        private PromiseData promise;

        public class Package
        {
            public string dbID;
            public string dbTable;

            public string what;
            public string where;
            public string data;
            public string join;
            public string orderby;
            public string limit;
            public string extra;
            public string column;
        }


        static string Token = "";
        static string BaseURL = "";
        static string Key = "";

        public HTTP(string token, string baseURL, string key)
        {
            Token = token;
            BaseURL = baseURL;
            Key = key;
        }

        public HTTP Post(string url)
        {
            promise = new PromiseData { type = UnityWebRequest.kHttpVerbPOST, url = BaseURL + url + ".php" };
            return this;
        }
        public HTTP Get(string url)
        {
            promise = new PromiseData { type = UnityWebRequest.kHttpVerbGET, url = BaseURL + url };
            return this;
        }

        public HTTP Data(Package data)
        {
            string jsonData;
            try
            {
                jsonData = JsonUtility.ToJson(data);
            }
            catch (Exception)
            {
                jsonData = null;
            }
            promise.data = jsonData;
            return this;
        }

        public HTTP Done(System.Action<HttpRequest> callBack)
        {
            promise.doneCallback = callBack;
            return this;
        }
        public HTTP Error(System.Action<HttpRequest> callBack)
        {
            promise.errorCallback = callBack;
            return this;
        }

        public async UniTask Call()
        {
            var result = await Http(promise.url, promise.data, promise.type);
            if (result.hasError) promise.errorCallback?.Invoke(result); else promise.doneCallback?.Invoke(result);
        }


        /// <summary>
        /// Perform an asynchronouse HTTP request and releases an HttpResponse.
        /// <para>url : the HTTP URL to call.</para>
        /// <para>content (optional) : a package of data to send to the HTTP URL. It must be an object. This object is converted to JSON.</para>
        /// <para>callMethod (optional, default POST) : the communication method to use (GET, POST, etc.).</para>
        /// </summary>
        private static async UniTask<HttpRequest> Http(string url, string content = null, string callMethod = UnityWebRequest.kHttpVerbPOST)
        {                       
            var restPackage = (content == null) ? "" : content;

            var httpRequest = new UnityWebRequest();
            
            if (Token == null) Debug.LogError("[UniDB] Token error.");
            if (url == "") Debug.LogError("[UniDB] URL error.");

            using (httpRequest)
            {
                try
                {
                    httpRequest.url = url;
                    httpRequest.method = callMethod;
                    httpRequest.downloadHandler = new DownloadHandlerBuffer();
                    httpRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(restPackage));

                    httpRequest.SetRequestHeader("Content-Type", "application/json");
                    httpRequest.SetRequestHeader("Accept", "application/json");
                    httpRequest.SetRequestHeader("Token", Token);

                    await httpRequest.SendWebRequest();

                    var data = httpRequest.downloadHandler.text;

                    if (httpRequest.isDone && httpRequest.downloadHandler.isDone)
                    {
                        var decoded = EncryptionHelper.Decrypt(Key, data);
                        var response = JsonHelper.Parse<Response>(decoded);
                        return new HttpRequest { response = response, rawData = decoded, code = httpRequest.responseCode, httpError = "", isArrived = true, hasError = false, errorType = HttpRequest.Error.NONE };
                    }
                    else
                    {
                        return new HttpRequest { response = null, rawData = data, code = httpRequest.responseCode, httpError = httpRequest.error, isArrived = false, hasError = true, errorType = HttpRequest.Error.HTTP_ERROR };
                    }
                }
                catch(Exception e)
                {
                    return new HttpRequest { response = null, rawData = "", code = httpRequest.responseCode, httpError = e.Message, isArrived = false, hasError = true, errorType = HttpRequest.Error.SYSTEM_ERROR };
                }

            }
        }

    }

    #endregion



    #region " ENCRYPTION / DECRYPTION "

    /// <summary>
    /// The system tool for encrypting, decrypting, encode, and decode strings.
    /// </summary>
    public static class EncryptionHelper
    {
        public static Encoding encoder = Encoding.UTF8;

        /// <summary>
        /// Convert the given string into an encrypted string.
        /// <para>password : a 32 digits password.</para>
        /// <para>plainText : the text to convert.</para>
        /// </summary>
        public static string Encrypt(string password, string plainText)
        {
            if (plainText == "") return "";

            Aes encryptor = Aes.Create();
            encryptor.Mode = CipherMode.CBC;
            encryptor.Key = Encoding.ASCII.GetBytes(password);
            encryptor.IV = Encoding.ASCII.GetBytes(password.Substring(0, 16));

            MemoryStream memoryStream = new MemoryStream();
            ICryptoTransform aesEncryptor = encryptor.CreateEncryptor();

            CryptoStream cryptoStream = new CryptoStream(memoryStream, aesEncryptor, CryptoStreamMode.Write);

            byte[] plainBytes = encoder.GetBytes(plainText);

            cryptoStream.Write(plainBytes, 0, plainBytes.Length);
            cryptoStream.FlushFinalBlock();

            byte[] cipherBytes = memoryStream.ToArray();

            memoryStream.Close();
            cryptoStream.Close();

            string cipherText = Convert.ToBase64String(cipherBytes, 0, cipherBytes.Length);
            return cipherText;
        }

        /// <summary>
        /// Decrypt the given string into the original, clear-text string.
        /// <para>password : a 32 digits password.</para>
        /// <para>cipherText : the code to convert back to plain text.</para>
        /// </summary>
        public static string Decrypt(string password, string cipherText)
        {
            if (cipherText == "") return "";

            Aes encryptor = Aes.Create();
            encryptor.Mode = CipherMode.CBC;
            encryptor.Key = Encoding.ASCII.GetBytes(password);
            encryptor.IV = Encoding.ASCII.GetBytes(password.Substring(0, 16));

            MemoryStream memoryStream = new MemoryStream();
            ICryptoTransform aesDecryptor = encryptor.CreateDecryptor();

            CryptoStream cryptoStream = new CryptoStream(memoryStream, aesDecryptor, CryptoStreamMode.Write);

            string plainText = String.Empty;

            try
            {
                byte[] cipherBytes = Convert.FromBase64String(cipherText);

                cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);
                cryptoStream.FlushFinalBlock();

                byte[] plainBytes = memoryStream.ToArray();

                plainText = encoder.GetString(plainBytes, 0, plainBytes.Length);
            }
            catch (Exception e)
            {
                Debug.LogError("Something goes wrong with the Dencryption system.\n" + e.Message);
                if (Database.debugMode) Debug.Log("<color='orange'>[UniDB] ERROR (click this warning and see what's happening)</color>\n" + cipherText);
            }
            finally
            {
                memoryStream.Close();
                cryptoStream.Close();
            }

            return plainText;
        }

        /// <summary>
        /// Return the given string hashed into a SHA256 string.
        /// </summary>
        public static string Sha256(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        /// <summary>
        /// Convert a string into a Base64 String.
        /// </summary>
        public static string StringToBase64(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// Convert a Base64 String to a string.
        /// </summary>
        public static string Base64ToString(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

    }

    #endregion


    #region " JSON HELPERS "

    public class JsonHelper
    {
        /// <summary>
        /// Convert a JSON string into an object of type T (string -> object)
        /// </summary>
        public static T Parse<T>(string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        /// <summary>
        /// Return a STRING which represents an Object converted to JSON.
        /// </summary>
        public static string Stringify(object jsonObject)
        {
            return JsonConvert.SerializeObject(jsonObject);
        }
    }

    #endregion

}
