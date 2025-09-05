using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PushNotificator : MonoBehaviour
{
    public GameObject alertPanel;
    public Text alertTitle;
    public Text alertMessage;
    public Button alertOkButton;

    public Button subscribeButton;
    public Button unsubscribeButton;
    public Button enableServerCommunicationButton;
    public Button disableServerCommunicationButton;
    public Button getServerCommunicationStatusButton;
    public InputField setStringTagKeyInputField;
    public InputField setStringTagValueInputField;
    public Button setStringTagButton;
    public Button setUserIdButton;
    public InputField userIdInputField;
    public Button callPostEventButton;
    public InputField postEventNameInputField;
    public Button registerEmailButton;
    public InputField registerEmailInputField;
    public Button setLanguageButton;
    public InputField setLanguageInputField;
    public Button getPushTokenButton;
    public Button getHWIDButton;

    void Start()
    {
        Pushwoosh.ApplicationCode = "XXXXX-XXXXX";
        Pushwoosh.FcmProjectNumber = "XXXXXXXXXXXX";
        Pushwoosh.Instance.OnRegisteredForPushNotifications += OnRegisteredForPushNotifications;
        Pushwoosh.Instance.OnFailedToRegisteredForPushNotifications += OnFailedToRegisteredForPushNotifications;
        Pushwoosh.Instance.OnPushNotificationsReceived += OnPushNotificationsReceived;
        Pushwoosh.Instance.OnPushNotificationsOpened += OnPushNotificationsOpened;

        subscribeButton.onClick.AddListener(OnSubscribe);
        unsubscribeButton.onClick.AddListener(OnUnsubscribe);
        enableServerCommunicationButton.onClick.AddListener(OnEnableServerCommunication);
        disableServerCommunicationButton.onClick.AddListener(OnDisableServerCommunication);
        getServerCommunicationStatusButton.onClick.AddListener(OnGetServerCommunicationStatus);
        setStringTagButton.onClick.AddListener(OnSetStringTagButton);
        userIdInputField.onSubmit.AddListener(OnSetUserIdFromInputField);
        setUserIdButton.onClick.AddListener(OnSetUserIdFromButton);
        postEventNameInputField.onSubmit.AddListener(OnCallPostEventFromInputField);
        callPostEventButton.onClick.AddListener(OnCallPostEventFromButton);
        registerEmailInputField.onSubmit.AddListener(OnCallRegisterEmailFromInputField);
        registerEmailButton.onClick.AddListener(OnCallRegisterEmailFromButton);
        setLanguageInputField.onSubmit.AddListener(OnSetLanguageFromInputField);
        setLanguageButton.onClick.AddListener(OnSetLanguageFromButton);
        getPushTokenButton.onClick.AddListener(OnGetPushTokenButton);
        getHWIDButton.onClick.AddListener(OnGetHWIDButton);

        if (alertPanel != null)
        {
            alertPanel.SetActive(false);
            alertOkButton.onClick.AddListener(CloseAlert);
        }
    }

    void OnRegisteredForPushNotifications(string token)
    {
        Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, "Received token: \n{0}", token);
        ShowAlert("Device registered", "Received token: \n" + token);
    }

    void OnFailedToRegisteredForPushNotifications(string error)
    {
        Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, "Error ocurred while registering to push notifications: \n{0}", error);
        ShowAlert("Device registration error", error);
    }

    public void OnSubscribe()
    {
        Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, "OnSubscribe was called");
        Pushwoosh.Instance.RegisterForPushNotifications();
    }

    public void OnUnsubscribe()
    {
        Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, "OnUnSubscribe was called");
        Pushwoosh.Instance.UnregisterForPushNotifications();
        string token = Pushwoosh.Instance.PushToken;
        ShowAlert("Device unregistered", "Push token: \n" + token);
    }

    void OnPushNotificationsReceived(string payload)
    {
        Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, "Received push notificaiton: \n{0}", payload);
    }

    void OnPushNotificationsOpened(string payload)
    {
        Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, "Opened push notificaiton: \n{0}", payload);
    }

    public void OnEnableServerCommunication()
    {
        if (!Pushwoosh.Instance.IsCommunicationEnabled())
        {
            Pushwoosh.Instance.SetCommunicationEnabled(true);
            Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, "Pushwoosh communication is enabled: {0}", Pushwoosh.Instance.IsCommunicationEnabled());
            ShowAlert("Server communication status", "Pushwoosh communication is enabled: " + Pushwoosh.Instance.IsCommunicationEnabled());
        }
        else
        {
            Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, "Pushwoosh communication is enabled: {0}", Pushwoosh.Instance.IsCommunicationEnabled());
            ShowAlert("Server communication status", "Pushwoosh communication is enabled: " + Pushwoosh.Instance.IsCommunicationEnabled());
        }
    }

    public void OnDisableServerCommunication()
    {
        if (Pushwoosh.Instance.IsCommunicationEnabled())
        {
            Pushwoosh.Instance.SetCommunicationEnabled(false);
            Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, "Pushwoosh communication is enabled: {0}", Pushwoosh.Instance.IsCommunicationEnabled());
            ShowAlert("Server communication status", "Pushwoosh communication is enabled: " + Pushwoosh.Instance.IsCommunicationEnabled());
        }
        else
        {
            Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, "Pushwoosh communication is enabled: {0}", Pushwoosh.Instance.IsCommunicationEnabled());
            ShowAlert("Server communication status", "Pushwoosh communication is enabled: " + Pushwoosh.Instance.IsCommunicationEnabled());
        }
    }

    public void OnGetServerCommunicationStatus()
    {
        bool serverCommunicationStatus = Pushwoosh.Instance.IsCommunicationEnabled();
        Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, "Pushwoosh communication is enabled: {0}", serverCommunicationStatus);
        ShowAlert("Server communication status", "Pushwoosh communication is enabled: " + serverCommunicationStatus);
    }

    public void OnSetStringTagButton()
    {
        string key = setStringTagKeyInputField.text;
        string value = setStringTagValueInputField.text;
        if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
        {
            Pushwoosh.Instance.SetStringTag(key, value);
            ShowAlert("String tag set", "Tag key: " + key + "\nTag value: " + value);
        }
        else
        {
            Debug.LogFormat(LogType.Warning, LogOption.NoStacktrace, null, "Key or value input field is empty");
            ShowAlert("Warning", "Key or value input field is empty");
        }
    }

    public void OnSetUserIdFromInputField(string inputText)
    {
        if (!string.IsNullOrEmpty(inputText))
        {
            Pushwoosh.Instance.SetUserId(inputText);
            ShowAlert("User ID set", "User ID: " + inputText);
        }
        else
        {
            Debug.LogFormat(LogType.Warning, LogOption.NoStacktrace, null, "User ID input is empty");
            ShowAlert("Warning", "User ID input field is empty");
        }
    }

    public void OnSetUserIdFromButton()
    {
        string userId = userIdInputField.text;
        if (!string.IsNullOrEmpty(userId))
        {
            Pushwoosh.Instance.SetUserId(userId);
            ShowAlert("User ID set", "User ID: " + userId);
        }
        else
        {
            Debug.LogFormat(LogType.Warning, LogOption.NoStacktrace, null, "User ID input field is empty");
            ShowAlert("Warning", "User ID input field is empty");
        }
    }

    public void OnCallPostEventFromInputField(string inputText)
    {
        if (!string.IsNullOrEmpty(inputText))
        {
            Pushwoosh.Instance.PostEvent(inputText, new Dictionary<string, object>() { { "test", "test" } });
            ShowAlert("Post event sent", "Post event name: " + inputText);
        }
        else
        {
            Debug.LogFormat(LogType.Warning, LogOption.NoStacktrace, null, "Post event input field is empty");
            ShowAlert("Warning", "Post event input field is empty");
        }
    }

    public void OnCallPostEventFromButton()
    {
        string postEventName = postEventNameInputField.text;
        if (!string.IsNullOrEmpty(postEventName))
        {
            Pushwoosh.Instance.PostEvent(postEventName, new Dictionary<string, object>() { { "test", "test" } });
            ShowAlert("Post event sent", "Post event name: " + postEventName);
        }
        else
        {
            Debug.LogFormat(LogType.Warning, LogOption.NoStacktrace, null, "Post event input field is empty");
            ShowAlert("Warning", "Post event input field is empty");
        }
    }

    public void OnCallRegisterEmailFromInputField(string inputText)
    {
        if (!string.IsNullOrEmpty(inputText))
        {
            Pushwoosh.Instance.SetEmail(inputText);
            ShowAlert("Email set", "Email " + inputText);
        }
        else
        {
            Debug.LogFormat(LogType.Warning, LogOption.NoStacktrace, null, "Email input field is empty");
            ShowAlert("Warning", "Email input field is empty");
        }
    }

    public void OnCallRegisterEmailFromButton()
    {
        string email = registerEmailInputField.text;
        if (!string.IsNullOrEmpty(email))
        {
            Pushwoosh.Instance.SetEmail(email);
            ShowAlert("Email set", "Email:  " + email);
        }
        else
        {
            Debug.LogFormat(LogType.Warning, LogOption.NoStacktrace, null, "Email input field is empty");
            ShowAlert("Warning", "Email input field is empty");
        }
    }

    public void OnSetLanguageFromInputField(string inputText)
    {
        if (!string.IsNullOrEmpty(inputText))
        {
            Pushwoosh.Instance.SetLanguage(inputText);
            ShowAlert("Language set:", "Language value: " + inputText);
        }
        else
        {
            Debug.LogFormat(LogType.Warning, LogOption.NoStacktrace, null, "Language input field is empty");
            ShowAlert("Warning", "Language input field is empty");
        }
    }

    public void OnSetLanguageFromButton()
    {
        string language = setLanguageInputField.text;
        if (!string.IsNullOrEmpty(language))
        {
            Pushwoosh.Instance.SetLanguage(language);
            ShowAlert("Language set", "Language value: " + language);
        }
        else
        {
            Debug.LogFormat(LogType.Warning, LogOption.NoStacktrace, null, "Language input field is empty");
            ShowAlert("Warning", "Language input field is empty");
        }
    }

    public void OnGetPushTokenButton()
    {
        string token = Pushwoosh.Instance.PushToken;
        if (!string.IsNullOrEmpty(token))
        {
            Debug.LogFormat(LogType.Warning, LogOption.NoStacktrace, null, "Push token: {0}", token);
            ShowAlert("Push token", token);
        }
        else
        {
            Debug.LogFormat(LogType.Warning, LogOption.NoStacktrace, null, "Push token is empty");
            ShowAlert("Push token", "Push token is empty");
        }
    }

    public void OnGetHWIDButton()
    {
        string hwid = Pushwoosh.Instance.HWID;
        if (!string.IsNullOrEmpty(hwid))
        {
            Debug.LogFormat(LogType.Warning, LogOption.NoStacktrace, null, "HWID: {0}", hwid);
            ShowAlert("HWID", hwid);
        }
        else
        {
            Debug.LogFormat(LogType.Warning, LogOption.NoStacktrace, null, "HWID is empty");
            ShowAlert("HWID is empty", hwid);
        }
    }

    public void ShowAlert(string title, string message)
    {
        if (alertPanel != null)
        {
            alertTitle.text = title;
            alertMessage.text = message;
            alertPanel.SetActive(true);
        }
    }
    
    public void CloseAlert()
    {
        if (alertPanel != null)
        {
            alertPanel.SetActive(false);
        }
    }
}