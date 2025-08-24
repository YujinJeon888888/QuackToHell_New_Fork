using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public interface ICardShopView
{
    event Action<int, ulong, int> OnClickBuy;
    event Action OnClickLock;
    event Action OnClickReRoll;

    void ShowLoading(bool on);
    void ShowResult(bool success, string msg);

    void SetLockedVisual(bool locked);
    void SetRefreshInteractable(bool interactable);
}

public sealed class CardShopView : MonoBehaviour, ICardShopView
{
    [Header("UI")]
    //[SerializeField] private Button buyButton;
    [SerializeField] private TMP_InputField cardIdInput;
    [SerializeField] private TMP_InputField priceInput;
    [SerializeField] private TMP_Text statusText;

    [SerializeField] private Button lockButton;
    [SerializeField] private Button rerollButton;
    [SerializeField] private Image lockIcon;
    [SerializeField] private Sprite spriteLocked;
    [SerializeField] private Sprite spriteUnlocked;

    public event Action<int, ulong, int> OnClickBuy;
    public event Action OnClickLock;
    public event Action OnClickReRoll;

    private void Awake()
    {
        /*buyButton.onClick.AddListener(() =>
        {
            int.TryParse(cardIdInput.text, out var cardId);
            int.TryParse(priceInput.text, out var price);
            OnClickBuy?.Invoke(cardId, 0UL, price);
        });

        if (lockButton) lockButton.onClick.AddListener(() => OnClickLock?.Invoke());
        if (rerollButton) rerollButton.onClick.AddListener(() => OnClickReRoll?.Invoke());
    }

    public void ShowLoading(bool on)
    {
        if (statusText) statusText.text = on ? "Processing..." : "";
    }

    public void ShowResult(bool success, string msg)
    {
        if (statusText) statusText.text = success ? $"✅ {msg}" : $"❌ {msg}";
    }

    public void SetLockedVisual(bool locked)
    {
        if (lockIcon)
            lockIcon.sprite = locked ? spriteLocked : spriteUnlocked;
    }

    public void SetRefreshInteractable(bool interactable)
    {
        if (rerollButton) rerollButton.interactable = interactable;
    }
}
