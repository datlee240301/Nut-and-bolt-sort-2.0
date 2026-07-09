#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Editor-only auto-injector.
///
/// When the user opens the "Home" scene in the Editor (or starts the Editor
/// with Home already open), this script:
///   1. Finds the main Canvas
///   2. Spawns a "LoginBtn (auto)" GameObject (Button + TMP label)
///   3. Spawns a "LoginPanel (auto)" GameObject built by LoginPanel.CreateRuntime,
///      with the LoginPanel script attached and fields wired
///   4. Wires the button's OnClick to LoginPanel.Show via a runtime listener
///   5. Marks the scene dirty so the user can save it
///
/// Idempotent: if either object already exists, nothing happens.
///
/// Also exposes a menu item under "Tools/ColorFruitJam/Inject Login UI Now"
/// in case the user wants to trigger it on demand.
/// </summary>
[InitializeOnLoad]
public static class LoginUISceneInjector
{
    private const string TARGET_SCENE = "Menu";
    private const string BUTTON_NAME = "LoginBtn (auto)";
    private const string PANEL_NAME = "LoginPanel (auto)";

    static LoginUISceneInjector()
    {
        EditorSceneManager.sceneOpened -= OnSceneOpened;
        EditorSceneManager.sceneOpened += OnSceneOpened;

        // Handle the case where Home is already the active scene when the
        // Editor finishes a domain reload.
        EditorApplication.delayCall += () =>
        {
            var scene = SceneManager.GetActiveScene();
            TryInject(scene, autoSave: false);
        };
    }

    private static void OnSceneOpened(Scene scene, OpenSceneMode mode)
    {
        TryInject(scene, autoSave: false);
    }

    [MenuItem("Tools/screwjam/Inject Login UI Now")]
    private static void InjectFromMenu()
    {
        var scene = SceneManager.GetActiveScene();
        if (scene.name != TARGET_SCENE)
        {
            EditorUtility.DisplayDialog("Inject Login UI",
                $"Please open the '{TARGET_SCENE}' scene first.", "OK");
            return;
        }

        TryInject(scene, autoSave: true);
    }

    private static void TryInject(Scene scene, bool autoSave)
    {
        if (!scene.IsValid() || scene.name != TARGET_SCENE) return;

        // Kiểm tra TOÀN scene (mọi Canvas) — scene Gameplay có nhiều Canvas,
        // chỉ check canvas đầu tiên sẽ inject trùng bộ thứ hai.
        bool panelExistsAnywhere = false;
        foreach (var go in scene.GetRootGameObjects())
        {
            if (go.GetComponentInChildren<LoginPanel>(true) != null)
            {
                panelExistsAnywhere = true;
                break;
            }
        }
        if (panelExistsAnywhere) return; // đã có Login UI trong scene

        // Find the main Canvas in the scene.
        Canvas canvas = null;
        foreach (var go in scene.GetRootGameObjects())
        {
            canvas = go.GetComponentInChildren<Canvas>(true);
            if (canvas != null) break;
        }

        if (canvas == null)
        {
            Debug.LogWarning("[LoginUISceneInjector] No Canvas found in scene.");
            return;
        }

        bool buttonExists = canvas.transform.Find(BUTTON_NAME) != null;
        bool panelExists = canvas.GetComponentInChildren<LoginPanel>(true) != null;
        if (buttonExists && panelExists) return; // already injected

        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Inject Login UI");

        // --- Panel ---
        LoginPanel panel = canvas.GetComponentInChildren<LoginPanel>(true);
        if (panel == null)
        {
            panel = LoginPanel.CreateRuntime(canvas.transform);
            panel.gameObject.name = PANEL_NAME;
            Undo.RegisterCreatedObjectUndo(panel.gameObject, "Create LoginPanel");
        }

        // --- Button ---
        if (!buttonExists)
        {
            var btnGo = LoginPanel.CreateLoginButton(canvas.transform, BUTTON_NAME, panel);
            Undo.RegisterCreatedObjectUndo(btnGo, "Create LoginBtn");
        }

        EditorSceneManager.MarkSceneDirty(scene);
        Undo.CollapseUndoOperations(undoGroup);

        Debug.Log($"[LoginUISceneInjector] Injected Login UI into '{scene.name}'. " +
                  "Save the scene to keep the changes.");

        if (autoSave)
        {
            EditorSceneManager.SaveScene(scene);
        }
    }
}
#endif