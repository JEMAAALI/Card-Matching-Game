using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button), typeof(AudioSource))]
public class CardsController : MonoBehaviour
{
    // Public fields (allow BoardManager to set them)
    [HideInInspector] public int cardValue;
    [HideInInspector] public bool isFlipped = false;

    // Inspector-assigned references
    [SerializeField] private Image image;
    [SerializeField] private Animation flipAnimation;   // optional: assign your Animation component here
    [SerializeField] private AudioClip flipAudioClip;

    private Sprite cardFace;
    private Sprite cardBack;
    private Button button;
    [SerializeField] private GameObject sparkleEffect; // assign in inspector
    [SerializeField] private GameObject winEffect; // assign in inspector
    private AudioSource audioSource;


    private void Awake()
    {
        button = GetComponent<Button>();
        audioSource = GetComponent<AudioSource>();

        if (image == null)
            image = GetComponent<Image>();

        button.onClick.AddListener(OnCardClicked);
    }

    /// <summary>
    /// Initialize card visuals and value.
    /// </summary>
    public void SetCard(int value, Sprite backSprite, Sprite faceSprite)
    {
        cardValue = value;
        cardBack = backSprite;
        cardFace = faceSprite;
        HideCardImmediate();
        EnableCard();
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnCardClicked);
    }

    /// <summary>
    /// Called by the Button click.
    /// </summary>
    public void OnCardClicked()
    {
        // Prevent double-click while flipping
        button.interactable = false;

        // play audio
        if (flipAudioClip != null)
            audioSource.PlayOneShot(flipAudioClip);
        else
            audioSource.Play();

        if (!isFlipped)
        {
            StartCoroutine(FlipCardCoroutine());
            
        }
        else
        {
            // re-enable if card was already flipped but for some reason button was disabled
            button.interactable = true;
        }

     }

 

    private IEnumerator FlipCardCoroutine()
    {
        if (flipAnimation != null)
            flipAnimation.Play();

        // animation length isn't guaranteed; keep a small wait (adjust to your animation)
        yield return new WaitForSeconds(0.5f);

        isFlipped = true;
        image.sprite = cardFace;
        // Note: do not automatically disable the button here; GameManager/BoardManager or Save logic decides.
        
        // notify game logic (decoupled via EventBus)
        EventBus.RaiseCardFlipped(this);
    }

    /// <summary>
    /// Hide visuals immediately (no animation).
    /// </summary>
    public void HideCardImmediate()
    {
        isFlipped = false;
        if (cardBack != null)
            image.sprite = cardBack;
    }

    public void HideCard()
    {
        // Called to hide after mismatch: play animation then swap
        StartCoroutine(HideCardCoroutine());
    }

    private IEnumerator HideCardCoroutine()
    {
        // optional: play flip animation backwards (if you have)
        if (flipAnimation != null)
            flipAnimation.Play();

        yield return new WaitForSeconds(0.45f);

        isFlipped = false;
        if (cardBack != null)
            image.sprite = cardBack;

        // re-enable click
        EnableCard();
    }

    public void DisableCard()
    {
        // logic your old script used: disable listeners and show overlay child (child[1])
        button.onClick.RemoveListener(OnCardClicked);
        button.interactable = false;

        if (transform.childCount > 1)
            transform.GetChild(1).gameObject.SetActive(true);
    }

    public void EnableCard()
    {
        // ensure listener is present
        button.onClick.RemoveListener(OnCardClicked);
        button.onClick.AddListener(OnCardClicked);
        button.interactable = true;

        if (transform.childCount > 1)
            transform.GetChild(1).gameObject.SetActive(false);
    }

    /// <summary>
    /// Called to show card briefly (your HiddenButtonListener behavior).
    /// </summary>
    public void ShowPeek()
    {
        StartCoroutine(FlipPeekCoroutine());
    }

    private IEnumerator FlipPeekCoroutine()
    {
        if (flipAnimation != null)
            flipAnimation.Play();

        yield return new WaitForSeconds(0.45f);
        image.sprite = cardFace;

        yield return new WaitForSeconds(0.45f);
        image.sprite = cardBack;
    }

    // For compatibility with old code that used LoadFlipped()
    public void LoadFlipped()
    {
        // Flip visually but keep button disabled (used by LoadGame to show saved flipped cards)
        StartCoroutine(LoadFlippedCoroutine());
    }

    private IEnumerator LoadFlippedCoroutine()
    {
        if (flipAnimation != null)
            flipAnimation.Play();

        yield return new WaitForSeconds(0.45f);
        isFlipped = true;
        image.sprite = cardFace;

        // After loading, you may want to keep it flipped and disabled (original behavior)
        button.interactable = false;
        button.onClick.RemoveListener(OnCardClicked);

        
    }

    public void ShowSparkle()
    {
        if (sparkleEffect != null)
            sparkleEffect.SetActive(true);
    }

    public void HideSparkle()
    {
        if (sparkleEffect != null)
            sparkleEffect.SetActive(false);
    }

   
}
