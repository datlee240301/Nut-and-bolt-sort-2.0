using System.Collections;
using ScrewJam.Auth;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class LoginPanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform     _bodyTransform;
    [SerializeField] private TMP_InputField    _usernameInput;
    [SerializeField] private TMP_InputField    _passwordInput;
    [SerializeField] private Button            _loginButton;
    [SerializeField] private Button            _closeButton;
    [SerializeField] private TextMeshProUGUI   _messageText;   
    [SerializeField] private GameObject        _loadingIndicator;
    [SerializeField] private TextMeshProUGUI   _loginButtonLabel;

    private readonly float _timeAnim = 0.4f;
    private bool _isAnimating;
    private bool _isRequesting;



    private void OnEnable()
    {
        if (AuthManager.IsLoggedIn())
        {
            //MainUIManager.Instance.PopupOpened = false;
            gameObject.SetActive(false);
            return;
        }

        _isAnimating = false;
        _isRequesting = false;
        SetMessage(string.Empty, false);
        SetLoading(false);
        PlayAnimOpen();
        //MainUIManager.Instance.PopupOpened = true;
    }

    private void Awake()
    {
        // Panel lưu sẵn trong scene có thể còn tham chiếu sprite built-in của
        // Editor (UI/Skin/Background.psd) -> render đen trên build thiết bị.
        // Chỉ gỡ đúng sprite built-in (Background/UISprite), không đụng sprite
        // do bạn tự gán từ Assets.
        foreach (var img in GetComponentsInChildren<Image>(true))
        {
            if (img.sprite != null &&
                (img.sprite.name == "Background" || img.sprite.name == "UISprite"))
            {
                img.sprite = null;
            }
        }

        if (_loginButton != null)
        {
            _loginButton.onClick.RemoveAllListeners();
            _loginButton.onClick.AddListener(OnClickLogin);
        }
        if (_closeButton != null)
        {
            _closeButton.onClick.RemoveAllListeners();
            _closeButton.onClick.AddListener(OnClickClose);
        }
    }


    public void Show()
    {
        if (AuthManager.IsLoggedIn()) return;
        gameObject.SetActive(true);
    }

    public void OnClickClose()
    {
        if (_isRequesting) return; 
        //if (AudioManager.Instance != null) AudioManager.PlaySound("Click");
        PlayAnimClose();
    }

    public void OnClickLogin()
    {
        if (_isRequesting) return;
        if (AuthManager.IsLoggedIn())
        {
            PlayAnimClose();
            return;
        }

        //if (AudioManager.Instance != null) AudioManager.PlaySound("Click");

        string username = _usernameInput != null ? _usernameInput.text?.Trim() : null;
        string password = _passwordInput != null ? _passwordInput.text         : null;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            SetMessage("Please enter username and password.", true);
            return;
        }

    
        SetMessage("Logging in...", false);
        SetLoading(true);
        _isRequesting = true;

        ApiService.Instance.Login(username, password,
            onSuccess: response =>
            {
                _isRequesting = false;
                SetLoading(false);
                AuthManager.SaveLogin(response);
                SetMessage("Welcome, " + response.username + "!", false);
                StartCoroutine(CloseAfterDelay(0.6f));
            },
            onError: errorMsg =>
            {
                _isRequesting = false;
                SetLoading(false);
                SetMessage(string.IsNullOrEmpty(errorMsg) ? "Login failed." : errorMsg, true);
            });
    }

   

    private IEnumerator CloseAfterDelay(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        PlayAnimClose();
    }

    private void SetMessage(string text, bool isError)
    {
        if (_messageText == null) return;
        _messageText.text = text ?? string.Empty;
        _messageText.color = isError ? new Color(0.85f, 0.2f, 0.2f, 1f)
                                      : new Color(0.15f, 0.55f, 0.2f, 1f);
    }

    private void SetLoading(bool isLoading)
    {
        if (_loadingIndicator != null) _loadingIndicator.SetActive(isLoading);
        if (_loginButton != null)      _loginButton.interactable = !isLoading;
        if (_closeButton != null)      _closeButton.interactable = !isLoading;
        if (_loginButtonLabel != null) _loginButtonLabel.text    = isLoading ? "..." : "Login";
    }

    private void PlayAnimOpen()
    {
        if (_bodyTransform == null) return;
        _bodyTransform.DOKill();
        _bodyTransform.localScale = Vector3.zero;
        _bodyTransform.DOScale(Vector3.one, _timeAnim).SetEase(Ease.OutBack);
    }

    private void PlayAnimClose()
    {
        if (_isAnimating) return;
        _isAnimating = true;
        if (_bodyTransform == null)
        {
            FinishClose();
            return;
        }
        _bodyTransform.DOKill();
        _bodyTransform.localScale = Vector3.one;
        _bodyTransform.DOScale(Vector3.zero, _timeAnim).SetEase(Ease.InBack)
            .OnComplete(FinishClose);
    }

    private void FinishClose()
    {
        //MainUIManager.Instance.PopupOpened = false;
        gameObject.SetActive(false);
        _isAnimating = false;
    }

   

    public static LoginPanel CreateRuntime(Transform parent)
    {
        // Root
        var root = new GameObject("LoginPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        root.transform.SetParent(parent, false);
        var rootRect  = root.GetComponent<RectTransform>();
        var dim       = root.GetComponent<Image>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;
        dim.color = new Color(0f, 0f, 0f, 0.65f);
        dim.raycastTarget = true;

        // Body
        var body = new GameObject("Body", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        body.transform.SetParent(root.transform, false);
        var bodyRect = body.GetComponent<RectTransform>();
        bodyRect.anchorMin = new Vector2(0.5f, 0.5f);
        bodyRect.anchorMax = new Vector2(0.5f, 0.5f);
        bodyRect.pivot     = new Vector2(0.5f, 0.5f);
        bodyRect.sizeDelta = new Vector2(700, 800);
        var bodyImg = body.GetComponent<Image>();
        bodyImg.color = new Color(1f, 0.97f, 0.88f, 1f); 
        bodyImg.sprite = UIBackgroundSprite(); 

        // Title
        var title = CreateTMP(body.transform, "Title", "LOGIN", 64,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
            new Vector2(0, -120), new Vector2(600, 80));
        title.color = new Color(0.95f, 0.4f, 0.15f, 1f);
        title.alignment = TextAlignmentOptions.Center;
        title.fontStyle = FontStyles.Bold;

        // Username label
        CreateTMP(body.transform, "UsernameLabel", "Username", 32,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
            new Vector2(-220, -230), new Vector2(220, 40)).color = new Color(0.25f, 0.25f, 0.25f);

        // Username input
        var usernameInput = CreateInputField(body.transform, "UsernameInput",
            new Vector2(0, -290), new Vector2(580, 80),
            "Enter username", false);

        // Password label
        CreateTMP(body.transform, "PasswordLabel", "Password", 32,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
            new Vector2(-220, -380), new Vector2(220, 40)).color = new Color(0.25f, 0.25f, 0.25f);

        // Password input
        var passwordInput = CreateInputField(body.transform, "PasswordInput",
            new Vector2(0, -440), new Vector2(580, 80),
            "Enter password", true);

        // Message text
        var messageText = CreateTMP(body.transform, "MessageText", string.Empty, 26,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
            new Vector2(0, -510), new Vector2(580, 60));
        messageText.alignment = TextAlignmentOptions.Center;

        // Login button
        var loginBtn = CreateButton(body.transform, "LoginButton", "Login",
            new Vector2(0, -620), new Vector2(420, 110),
            new Color(0.95f, 0.4f, 0.15f, 1f));
        var loginBtnLabel = loginBtn.GetComponentInChildren<TextMeshProUGUI>();

        // Close button
        var closeBtn = CreateButton(body.transform, "CloseButton", "X",
            new Vector2(310, -50), new Vector2(80, 80),
            new Color(0.85f, 0.25f, 0.2f, 1f));
        var closeRect = (closeBtn.transform as RectTransform);
        closeRect.anchorMin = new Vector2(1f, 1f);
        closeRect.anchorMax = new Vector2(1f, 1f);
        closeRect.pivot     = new Vector2(1f, 1f);
        closeRect.anchoredPosition = new Vector2(-20, -20);

        // Loading indicator (simple rotating image)
        var loading = new GameObject("Loading", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        loading.transform.SetParent(body.transform, false);
        var loadingRect = loading.GetComponent<RectTransform>();
        loadingRect.anchorMin = new Vector2(0.5f, 0.5f);
        loadingRect.anchorMax = new Vector2(0.5f, 0.5f);
        loadingRect.pivot     = new Vector2(0.5f, 0.5f);
        loadingRect.sizeDelta = new Vector2(60, 60);
        loadingRect.anchoredPosition = new Vector2(0, -700);
        var loadingImg = loading.GetComponent<Image>();
        loadingImg.color = new Color(0.95f, 0.4f, 0.15f, 1f);
        loading.AddComponent<RotateForever>();
        loading.SetActive(false);

        // Hook up component
        var panel = root.AddComponent<LoginPanel>();
        panel._bodyTransform     = bodyRect;
        panel._usernameInput     = usernameInput;
        panel._passwordInput     = passwordInput;
        panel._loginButton       = loginBtn;
        panel._closeButton       = closeBtn;
        panel._messageText       = messageText;
        panel._loadingIndicator  = loading;
        panel._loginButtonLabel  = loginBtnLabel;

        loginBtn.onClick.AddListener(panel.OnClickLogin);
        closeBtn.onClick.AddListener(panel.OnClickClose);

        root.SetActive(false);
        return panel;
    }

  

    private static TextMeshProUGUI CreateTMP(Transform parent, string name, string text, float size,
                                             Vector2 anchorMin, Vector2 anchorMax,
                                             Vector2 anchoredPos, Vector2 sizeDelta)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer));
        go.transform.SetParent(parent, false);
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot     = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta = sizeDelta;
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.color = Color.black;
        tmp.alignment = TextAlignmentOptions.Left;
        tmp.enableWordWrapping = true;
        return tmp;
    }

    private static TMP_InputField CreateInputField(Transform parent, string name,
                                                   Vector2 anchoredPos, Vector2 sizeDelta,
                                                   string placeholder, bool isPassword)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(parent, false);
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot     = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta = sizeDelta;

        var bg = go.GetComponent<Image>();
        bg.color = Color.white;
        bg.sprite = UIBackgroundSprite();

        // Text area (clipping mask container)
        var textArea = new GameObject("TextArea", typeof(RectTransform), typeof(RectMask2D));
        textArea.transform.SetParent(go.transform, false);
        var textAreaRect = textArea.GetComponent<RectTransform>();
        textAreaRect.anchorMin = Vector2.zero;
        textAreaRect.anchorMax = Vector2.one;
        textAreaRect.offsetMin = new Vector2(20, 10);
        textAreaRect.offsetMax = new Vector2(-20, -10);

        // Placeholder
        var placeholderGo = new GameObject("Placeholder", typeof(RectTransform), typeof(CanvasRenderer));
        placeholderGo.transform.SetParent(textArea.transform, false);
        var phRect = placeholderGo.GetComponent<RectTransform>();
        phRect.anchorMin = Vector2.zero;
        phRect.anchorMax = Vector2.one;
        phRect.offsetMin = Vector2.zero;
        phRect.offsetMax = Vector2.zero;
        var phTmp = placeholderGo.AddComponent<TextMeshProUGUI>();
        phTmp.text = placeholder;
        phTmp.fontSize = 32;
        phTmp.color = new Color(0.5f, 0.5f, 0.5f, 0.7f);
        phTmp.alignment = TextAlignmentOptions.MidlineLeft;
        phTmp.fontStyle = FontStyles.Italic;

        // Text
        var textGo = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer));
        textGo.transform.SetParent(textArea.transform, false);
        var txtRect = textGo.GetComponent<RectTransform>();
        txtRect.anchorMin = Vector2.zero;
        txtRect.anchorMax = Vector2.one;
        txtRect.offsetMin = Vector2.zero;
        txtRect.offsetMax = Vector2.zero;
        var textTmp = textGo.AddComponent<TextMeshProUGUI>();
        textTmp.text = string.Empty;
        textTmp.fontSize = 34;
        textTmp.color = new Color(0.1f, 0.1f, 0.1f, 1f);
        textTmp.alignment = TextAlignmentOptions.MidlineLeft;

        var input = go.AddComponent<TMP_InputField>();
        input.textViewport = textAreaRect;
        input.textComponent = textTmp;
        input.placeholder = phTmp;
        input.lineType = TMP_InputField.LineType.SingleLine;
        if (isPassword)
        {
            input.contentType = TMP_InputField.ContentType.Password;
            input.inputType   = TMP_InputField.InputType.Password;
            input.asteriskChar = '*';
        }
        else
        {
            input.contentType = TMP_InputField.ContentType.Alphanumeric;
        }
        return input;
    }

    private static Button CreateButton(Transform parent, string name, string label,
                                       Vector2 anchoredPos, Vector2 sizeDelta, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(parent, false);
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot     = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta = sizeDelta;

        var img = go.GetComponent<Image>();
        img.color = color;
        img.sprite = UIBackgroundSprite();
        img.type = Image.Type.Sliced;

        var btn = go.AddComponent<Button>();
        var colors = btn.colors;
        colors.highlightedColor = new Color(color.r * 1.1f, color.g * 1.1f, color.b * 1.1f, 1f);
        colors.pressedColor     = new Color(color.r * 0.85f, color.g * 0.85f, color.b * 0.85f, 1f);
        btn.colors = colors;

        // Label
        var lblGo = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer));
        lblGo.transform.SetParent(go.transform, false);
        var lblRect = lblGo.GetComponent<RectTransform>();
        lblRect.anchorMin = Vector2.zero;
        lblRect.anchorMax = Vector2.one;
        lblRect.offsetMin = Vector2.zero;
        lblRect.offsetMax = Vector2.zero;
        var lblTmp = lblGo.AddComponent<TextMeshProUGUI>();
        lblTmp.text = label;
        lblTmp.fontSize = 40;
        lblTmp.color = Color.white;
        lblTmp.alignment = TextAlignmentOptions.Center;
        lblTmp.fontStyle = FontStyles.Bold;
        return btn;
    }

  
    public static GameObject CreateLoginButton(Transform parent, string name, LoginPanel panel)
    {
        var btnGo = new GameObject(name,
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        btnGo.transform.SetParent(parent, false);
        var rect = btnGo.GetComponent<RectTransform>();
        // Top-left, below the existing top HUD so we don't overlap.
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot     = new Vector2(0f, 1f);
        rect.anchoredPosition = new Vector2(40, -260);
        rect.sizeDelta = new Vector2(320, 110);

        var img = btnGo.GetComponent<Image>();
        img.color = new Color(0.95f, 0.4f, 0.15f, 1f);
        var bgSprite = UIBackgroundSprite();
        if (bgSprite != null)
        {
            img.sprite = bgSprite;
            img.type = Image.Type.Sliced;
        }

        var btn = btnGo.GetComponent<Button>();
        var colors = btn.colors;
        colors.highlightedColor = new Color(1f, 0.55f, 0.25f, 1f);
        colors.pressedColor     = new Color(0.75f, 0.3f, 0.1f, 1f);
        btn.colors = colors;

        // Label
        var lblGo = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer));
        lblGo.transform.SetParent(btnGo.transform, false);
        var lblRect = lblGo.GetComponent<RectTransform>();
        lblRect.anchorMin = Vector2.zero;
        lblRect.anchorMax = Vector2.one;
        lblRect.offsetMin = Vector2.zero;
        lblRect.offsetMax = Vector2.zero;
        var lbl = lblGo.AddComponent<TextMeshProUGUI>();
        lbl.text = AuthManager.IsLoggedIn() ? AuthManager.GetUsername() : "Login";
        lbl.fontSize = 40;
        lbl.color = Color.white;
        lbl.alignment = TextAlignmentOptions.Center;
        lbl.fontStyle = FontStyles.Bold;
        lbl.enableAutoSizing = true;
        lbl.fontSizeMin = 18;
        lbl.fontSizeMax = 40;

        // Attach the live binder (also keeps the label fresh on login/logout).
        var binder = btnGo.AddComponent<LoginButtonBinder>();
        binder.Setup(btn, lbl, panel);

        return btnGo;
    }

    private static Sprite UIBackgroundSprite()
    {
        // Trước đây dùng Resources.GetBuiltinResource<Sprite>("UI/Skin/Background.psd").
        // Asset đó CHỈ tồn tại trong Unity Editor, không được đóng gói vào build
        // Android/iOS -> trên thiết bị texture bị thiếu nên Image render MÀU ĐEN
        // (trong Editor có texture nên thấy màu cam = tint cam x texture).
        // Trả về null để Image vẽ khối màu phẳng — đồng nhất giữa Editor và build.
        return null;
    }
}

public class RotateForever : MonoBehaviour
{
    [SerializeField] private float speed = -240f;
    private void Update()
    {
        transform.Rotate(0f, 0f, speed * Time.unscaledDeltaTime);
    }
}


public class LoginButtonBinder : MonoBehaviour
{
    [SerializeField] private Button           _button;
    [SerializeField] private TextMeshProUGUI  _label;
    [SerializeField] private LoginPanel       _panel;

    public void Setup(Button button, TextMeshProUGUI label, LoginPanel panel)
    {
        _button = button;
        _label  = label;
        _panel  = panel;
        if (_button != null)
        {
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(OnClick);
        }
        RefreshLabel();
    }

    private void OnEnable()
    {
        // Nút lưu trong scene có thể còn tham chiếu sprite built-in của Editor
        // -> đen trên build. Chỉ gỡ đúng sprite built-in, về khối màu phẳng (cam).
        var img = GetComponent<Image>();
        if (img != null && img.sprite != null &&
            (img.sprite.name == "Background" || img.sprite.name == "UISprite"))
        {
            img.sprite = null;
        }

        ScrewJam.Auth.AuthManager.OnLoginStateChanged -= RefreshLabel;
        ScrewJam.Auth.AuthManager.OnLoginStateChanged += RefreshLabel;
        RefreshLabel();

        if (_button != null && _button.onClick.GetPersistentEventCount() == 0)
        {
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(OnClick);
        }
    }

    private void OnDisable()
    {
        ScrewJam.Auth.AuthManager.OnLoginStateChanged -= RefreshLabel;
    }

    private void OnClick()
    {
        if (ScrewJam.Auth.AuthManager.IsLoggedIn()) return;
        //if (AudioManager.Instance != null) AudioManager.PlaySound("Click");
        if (_panel != null) _panel.Show();
    }

    private void RefreshLabel()
    {
        if (_label == null) return;
        _label.text = ScrewJam.Auth.AuthManager.IsLoggedIn()
            ? ScrewJam.Auth.AuthManager.GetUsername()
            : "Login";
    }
}
