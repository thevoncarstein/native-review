using UnityEngine;
using UnityEngine.UI;

public class NativeReviewExample : MonoBehaviour
{
    [SerializeField] private Button _loadButton;
    [SerializeField] private Button _showButton;
    [SerializeField] private Text _reviewText;

    private void Start()
    {
        _loadButton.onClick.AddListener(() => NativeReview.Instance.LoadReview());
        _showButton.onClick.AddListener(() => NativeReview.Instance.ShowReview());

        NativeReview.Instance.OnReviewLoaded += SetText;
        NativeReview.Instance.OnReviewDisplayed += SetText;
    }

    private void SetText(bool status, string message)
    {
        _reviewText.text += $"Status = {status}, {message}\n";
    }
}