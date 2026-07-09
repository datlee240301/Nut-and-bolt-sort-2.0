using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public static class LoginUIBootstrap
{
    private const string TARGET_SCENE = "Menu";
    private const string BUTTON_NAME  = "LoginBtn (auto)";
    private const string PANEL_NAME   = "LoginPanel (auto)";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Init()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        TryInject(SceneManager.GetActiveScene());
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TryInject(scene);
    }

    private static void TryInject(Scene scene)
    {
        if (!scene.IsValid() || scene.name != TARGET_SCENE) return;

        // QUAN TRỌNG: kiểm tra TOÀN scene trước khi tạo.
        // Scene Gameplay có 3 Canvas; bản cũ chỉ kiểm tra Canvas đầu tiên tìm thấy,
        // trong khi LoginPanel đã lưu nằm ở Canvas khác -> tưởng chưa có và sinh
        // thêm bộ nút/panel thứ hai ở vị trí mặc định (bug nút thừa góc trên-trái).
        foreach (var go in scene.GetRootGameObjects())
        {
            if (go.GetComponentInChildren<LoginPanel>(true) != null) return;
        }

        Canvas canvas = null;
        foreach (var go in scene.GetRootGameObjects())
        {
            canvas = go.GetComponentInChildren<Canvas>(true);
            if (canvas != null) break;
        }
        if (canvas == null)
        {
            Debug.LogWarning("[LoginUIBootstrap] No Canvas found in scene; skipping.");
            return;
        }

        var panel = LoginPanel.CreateRuntime(canvas.transform);
        panel.gameObject.name = PANEL_NAME;
        LoginPanel.CreateLoginButton(canvas.transform, BUTTON_NAME, panel);
    }
}
