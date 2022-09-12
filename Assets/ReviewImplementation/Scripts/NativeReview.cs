#region Namespaces

#if UNITY_ANDROID

using Google.Play.Review;

#elif UNITY_IOS

using UnityEngine.iOS;

#endif

using System.Collections;
using UnityEngine;
using UnityEngine.Events;

#endregion

public class NativeReview : MonoBehaviour
{
    #region Fields, Properties & Initializing

    #region Singleton

    private static NativeReview _instance;

    public static NativeReview Instance =>
        _instance ??= FindObjectOfType<NativeReview>() ??
                    new GameObject(nameof(NativeReview))
                    .AddComponent<NativeReview>();

    #endregion

    public UnityAction<bool, string> OnReviewLoaded { get; set; }
    public UnityAction<bool, string> OnReviewDisplayed { get; set; }
    public bool IsReviewReady { get; set; }

#if UNITY_ANDROID

    private ReviewManager _reviewManager = new ReviewManager();
    private PlayReviewInfo _playReviewInfo;

#endif

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

    #endregion

    public void LoadReview()
    {
        var message = string.Empty;

        if (Application.platform == RuntimePlatform.Android) LoadNativeAndroidReview();
        else LoadGeneralReview();

        #region Local functions

        void LoadNativeAndroidReview()
        {
#if UNITY_ANDROID

            if (_playReviewInfo == null)
                StartCoroutine(RequestReviewInfo());

            IEnumerator RequestReviewInfo()
            {
                var requestFlowOperation = _reviewManager.RequestReviewFlow();
                yield return requestFlowOperation;
                if (requestFlowOperation.Error != ReviewErrorCode.NoError)
                {
                    IsReviewReady = false;
                    message = requestFlowOperation.Error.ToString();

                    yield break;
                }
                _playReviewInfo = requestFlowOperation.GetResult();

                IsReviewReady = true;
                message = "Review loaded successfully.";

                OnReviewLoaded?.Invoke(IsReviewReady, message);
            }

#endif
        }

        void LoadGeneralReview()
        {
#if !UNITY_ANDROID || UNITY_EDITOR

            IsReviewReady = true;
            message = "Review loaded successfully.";
            OnReviewLoaded?.Invoke(IsReviewReady, message);

#endif
        }

        #endregion
    }

    public void ShowReview()
    {
        if (Application.platform == RuntimePlatform.Android) ShowNativeAndroidReview();
        else if (Application.platform == RuntimePlatform.IPhonePlayer) ShowNativeIOSReview();
        else ShowGeneralReview();

        #region Local functions

        void ShowNativeAndroidReview()
        {
#if UNITY_ANDROID

            if (_playReviewInfo == null)
            {
                OnReviewDisplayed?.Invoke(false, "Review failed to load.");
                return;
            }

            StartCoroutine(LaunchReviewFlow());

            IEnumerator LaunchReviewFlow()
            {
                var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
                yield return launchFlowOperation;
                ResetLoadStatus();
                if (launchFlowOperation.Error != ReviewErrorCode.NoError)
                {
                    OnReviewDisplayed?.Invoke(false, launchFlowOperation.Error.ToString());
                    yield break;
                }
                OnReviewDisplayed?.Invoke(true, "Review displayed successfully.");

                void ResetLoadStatus() => _playReviewInfo = null;
            }

#endif
        }

        void ShowNativeIOSReview()
        {
#if UNITY_IOS

            bool displayResult = false;

                if (IsReviewReady)
                    displayResult = Device.RequestStoreReview();

            var displayMessage = displayResult ?
                "Review displayed successfully." :
                "Review failed to load.";

            OnReviewDisplayed?.Invoke(displayResult, displayMessage);

            ResetLoadStatus();

            void ResetLoadStatus() => IsReviewReady = false;

#endif
        }

        void ShowGeneralReview()
        {
#if UNITY_EDITOR

            OnReviewDisplayed?.Invoke(true, "Review displayed.");

#endif
        }

        #endregion
    }
}