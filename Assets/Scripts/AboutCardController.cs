using UnityEngine;

public class AboutCardController : MonoBehaviour
{
    [SerializeField] private GameObject aboutPanel;
    [SerializeField] private string websiteUrl = "";

    // Hook to the "Close" button inside the card
    public void Hide()
    {
        aboutPanel.SetActive(false);
    }

    // Hook to an info button
    public void Show()
    {
        aboutPanel.SetActive(true);
    }

    // Hook to the "Website" button inside the card
    public void OpenWebsite()
    {
        Application.OpenURL(websiteUrl);
    }
}