using UnityEngine;

namespace ScrewJam.Auth
{
    /// <summary>
    /// Persists and exposes the current user's authentication state via PlayerPrefs.
    /// Keys are intentionally namespaced (auth_*) to avoid collisions with the rest
    /// of the game's ResourceManager keys.
    /// </summary>
    public static class AuthManager
    {
        public const string KEY_TOKEN       = "auth_token";
        public const string KEY_USERNAME    = "username";
        public const string KEY_USER_ID     = "user_id";
        public const string KEY_USER_ROLE   = "user_role";
        public const string KEY_IS_LOGGED   = "is_logged_in";

        /// <summary>Fired whenever the login state changes (login or logout).</summary>
        public static System.Action OnLoginStateChanged;

        // Token giả do phiên bản login-cục-bộ cũ lưu lại. Nếu còn trong
        // PlayerPrefs của máy test, coi như CHƯA đăng nhập và dọn sạch để
        // người chơi đăng nhập lại bằng tài khoản thật.
        private const string LEGACY_LOCAL_TOKEN = "local-login-token";

        public static bool IsLoggedIn()
        {
            if (GetAuthToken() == LEGACY_LOCAL_TOKEN)
            {
                Logout(); // dọn token giả còn sót từ bản cũ
                return false;
            }
            return PlayerPrefs.GetInt(KEY_IS_LOGGED, 0) == 1 &&
                   !string.IsNullOrEmpty(GetAuthToken());
        }

        public static string GetAuthToken()
        {
            return PlayerPrefs.GetString(KEY_TOKEN, string.Empty);
        }

        public static string GetUsername()
        {
            return PlayerPrefs.GetString(KEY_USERNAME, string.Empty);
        }

        public static string GetUserId()
        {
            return PlayerPrefs.GetString(KEY_USER_ID, string.Empty);
        }

        public static string GetUserRole()
        {
            return PlayerPrefs.GetString(KEY_USER_ROLE, string.Empty);
        }

        public static void SaveLogin(LoginResponse response)
        {
            if (response == null) return;
            PlayerPrefs.SetString(KEY_TOKEN,    response.token    ?? string.Empty);
            PlayerPrefs.SetString(KEY_USERNAME, response.username ?? string.Empty);
            PlayerPrefs.SetString(KEY_USER_ID,  response._id      ?? string.Empty);
            PlayerPrefs.SetString(KEY_USER_ROLE, response.role    ?? string.Empty);
            PlayerPrefs.SetInt(KEY_IS_LOGGED, 1);
            PlayerPrefs.Save();
            OnLoginStateChanged?.Invoke();
        }

        public static void Logout()
        {
            PlayerPrefs.DeleteKey(KEY_TOKEN);
            PlayerPrefs.DeleteKey(KEY_USERNAME);
            PlayerPrefs.DeleteKey(KEY_USER_ID);
            PlayerPrefs.DeleteKey(KEY_USER_ROLE);
            PlayerPrefs.SetInt(KEY_IS_LOGGED, 0);
            PlayerPrefs.Save();
            OnLoginStateChanged?.Invoke();
        }
    }
}
