using TMPro;
using UnityEngine;

public class StructureShooterUI : MonoBehaviour
{
    [SerializeField] private StructureShooter shooter;
    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private TMP_Text blocksText;
    [SerializeField] private GameObject gameOverPanel; // holds messageText + the "Play Again" button
    [SerializeField] private TMP_Text messageText;

    private void OnEnable()
    {
        shooter.OnAmmoChanged += HandleAmmoChanged;
        shooter.OnBlocksChanged += HandleBlocksChanged;
        shooter.OnGameOver += HandleGameOver;
        shooter.OnGameReset += HandleGameReset;
    }

    private void OnDisable()
    {
        shooter.OnAmmoChanged -= HandleAmmoChanged;
        shooter.OnBlocksChanged -= HandleBlocksChanged;
        shooter.OnGameOver -= HandleGameOver;
        shooter.OnGameReset -= HandleGameReset;
    }

    private void HandleAmmoChanged(int ammo)
    {
        ammoText.text = $"Ammo: {ammo}";
    }

    private void HandleBlocksChanged(int knocked, int total)
    {
        blocksText.text = $"Blocks: {knocked}/{total}";
    }

    private void HandleGameOver(bool won)
    {
        messageText.text = won ? "Structure destroyed!" : "Out of ammo!";
        gameOverPanel.SetActive(true);
    }

    private void HandleGameReset()
    {
        gameOverPanel.SetActive(false);
        ammoText.text = "Ammo: -";
        blocksText.text = "Blocks: -";
    }
}