using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace ScrewJam.Auth
{

    public class ApiService : MonoBehaviour
    {
        private const string BASE_URL = "https://phteam.shop/api/";
        private const int    TIMEOUT_SECONDS = 20;

        private static ApiService _instance;
        public static ApiService Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("ApiService");
                    _instance = go.AddComponent<ApiService>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

       

        public void Login(string username, string password,
                          Action<LoginResponse> onSuccess,
                          Action<string> onError)
        {
            StartCoroutine(LoginRoutine(username, password, onSuccess, onError));
        }

        public void VerifyPurchase(string packageId, string receipt, string token,
                                   Action<VerifyPurchaseResponse> onSuccess,
                                   Action<string> onError)
        {
            StartCoroutine(VerifyPurchaseRoutine(packageId, receipt, token, onSuccess, onError));
        }

      

        private IEnumerator LoginRoutine(string username, string password,
                                         Action<LoginResponse> onSuccess,
                                         Action<string> onError)
        {
            var payload = new LoginRequest { username = username, password = password };
            string jsonBody = JsonUtility.ToJson(payload);
            string url = BASE_URL + "auth/login";

            Debug.Log($"[ApiService] POST {url} body={jsonBody}");

            using (UnityWebRequest req = BuildPostRequest(url, jsonBody, null))
            {
                yield return req.SendWebRequest();

                if (IsNetworkError(req))
                {
                    string err = $"Network error: {req.error}";
                    Debug.LogError($"[ApiService] Login failed. {err}");
                    SafeInvoke(onError, err);
                    yield break;
                }

                string responseText = req.downloadHandler != null ? req.downloadHandler.text : string.Empty;
                Debug.Log($"[ApiService] Login response code={req.responseCode} body={responseText}");

                if (req.responseCode < 200 || req.responseCode >= 300)
                {
                    string serverMessage = TryReadErrorMessage(responseText, $"Server error {req.responseCode}");
                    SafeInvoke(onError, serverMessage);
                    yield break;
                }

                LoginResponse parsed = null;
                try
                {
                    parsed = JsonUtility.FromJson<LoginResponse>(responseText);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[ApiService] Login response parse error: {ex.Message}");
                }

                if (parsed == null || string.IsNullOrEmpty(parsed.token))
                {
                    SafeInvoke(onError, "Invalid response from server.");
                    yield break;
                }

                SafeInvoke(onSuccess, parsed);
            }
        }

        private IEnumerator VerifyPurchaseRoutine(string packageId, string receipt, string token,
                                                  Action<VerifyPurchaseResponse> onSuccess,
                                                  Action<string> onError)
        {
          
            string jsonBody = BuildVerifyPurchaseJson(packageId, receipt);
            string url = BASE_URL + "purchases/verify";

            Debug.Log($"[ApiService] POST {url} body={jsonBody}");

            using (UnityWebRequest req = BuildPostRequest(url, jsonBody, token))
            {
                yield return req.SendWebRequest();

                if (IsNetworkError(req))
                {
                    string err = $"Network error: {req.error}";
                    Debug.LogError($"[ApiService] VerifyPurchase failed. {err}");
                    SafeInvoke(onError, err);
                    yield break;
                }

                string responseText = req.downloadHandler != null ? req.downloadHandler.text : string.Empty;
                Debug.Log($"[ApiService] VerifyPurchase response code={req.responseCode} body={responseText}");

                if (req.responseCode < 200 || req.responseCode >= 300)
                {
                    string serverMessage = TryReadErrorMessage(responseText, $"Server error {req.responseCode}");
                    SafeInvoke(onError, serverMessage);
                    yield break;
                }

                VerifyPurchaseResponse parsed = null;
                try
                {
                    parsed = JsonUtility.FromJson<VerifyPurchaseResponse>(responseText);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[ApiService] VerifyPurchase response parse error: {ex.Message}");
                }

                if (parsed == null) parsed = new VerifyPurchaseResponse { message = responseText };
                SafeInvoke(onSuccess, parsed);
            }
        }

    

        private static UnityWebRequest BuildPostRequest(string url, string jsonBody, string token)
        {
            var req = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
            byte[] raw = Encoding.UTF8.GetBytes(jsonBody ?? string.Empty);
            req.uploadHandler   = new UploadHandlerRaw(raw) { contentType = "application/json" };
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            req.SetRequestHeader("Accept",       "application/json");
            if (!string.IsNullOrEmpty(token))
            {
                req.SetRequestHeader("Authorization", "Bearer " + token);
            }
            req.timeout = TIMEOUT_SECONDS;
            return req;
        }

        private static bool IsNetworkError(UnityWebRequest req)
        {
#if UNITY_2020_1_OR_NEWER
            return req.result == UnityWebRequest.Result.ConnectionError ||
                   req.result == UnityWebRequest.Result.DataProcessingError;
#else
            return req.isNetworkError;
#endif
        }

        private static string TryReadErrorMessage(string responseText, string fallback)
        {
            if (string.IsNullOrEmpty(responseText)) return fallback;
            try
            {
                var err = JsonUtility.FromJson<ApiErrorResponse>(responseText);
                if (err != null && !string.IsNullOrEmpty(err.message)) return err.message;
            }
            catch { /* fall through */ }
            return fallback;
        }

       
        private static string BuildVerifyPurchaseJson(string packageId, string receipt)
        {
            var sb = new StringBuilder();
            sb.Append('{');
            sb.Append("\"packageId\":");
            sb.Append(EscapeJsonString(packageId));
            sb.Append(',');
            sb.Append("\"receipt\":");
            if (string.IsNullOrEmpty(receipt))
            {
                sb.Append("{}");
            }
            else
            {
                string trimmed = receipt.Trim();
                if (trimmed.StartsWith("{") || trimmed.StartsWith("["))
                {
                    sb.Append(trimmed); 
                }
                else
                {
                    sb.Append(EscapeJsonString(receipt)); 
                }
            }
            sb.Append('}');
            return sb.ToString();
        }

        private static string EscapeJsonString(string s)
        {
            if (s == null) return "null";
            var sb = new StringBuilder(s.Length + 2);
            sb.Append('"');
            foreach (char c in s)
            {
                switch (c)
                {
                    case '\\': sb.Append("\\\\"); break;
                    case '"':  sb.Append("\\\""); break;
                    case '\b': sb.Append("\\b");  break;
                    case '\f': sb.Append("\\f");  break;
                    case '\n': sb.Append("\\n");  break;
                    case '\r': sb.Append("\\r");  break;
                    case '\t': sb.Append("\\t");  break;
                    default:
                        if (c < 0x20) sb.AppendFormat("\\u{0:x4}", (int)c);
                        else          sb.Append(c);
                        break;
                }
            }
            sb.Append('"');
            return sb.ToString();
        }

        private static void SafeInvoke<T>(Action<T> cb, T value)
        {
            if (cb == null) return;
            try { cb.Invoke(value); }
            catch (Exception ex) { Debug.LogError($"[ApiService] Callback threw: {ex}"); }
        }
    }
}
