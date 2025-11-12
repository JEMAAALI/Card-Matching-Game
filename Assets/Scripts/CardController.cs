using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CardController : MonoBehaviour
{
    public int cardValue;
    public bool isFlipped = false;
    public bool matchCheck = true;
    private Button button;
    private Image image;

    private Sprite cardFace;
    private Sprite cardBack;

    private Animation animation;


    void Awake()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();
        animation = GetComponent<Animation>();
        button.onClick.AddListener(OnCardClicked);
    }
     
    public void OnCardClicked()
    {
        button.onClick.RemoveListener(OnCardClicked);
        if (!isFlipped)
        {
            StartCoroutine(FlipCard());
            if (matchCheck == true) { 
            StartCoroutine(MatchEffects(this));
            }
        }
    }

    IEnumerator MatchEffects(CardController card)
    {
        WaitForSeconds _w1 = new WaitForSeconds(1f);
        WaitForSeconds _w2 = new WaitForSeconds(2f);
        yield return _w1;
        card.transform.GetChild(0).gameObject.SetActive(true);
        yield return _w2;
        card.transform.GetChild(0).gameObject.SetActive(false);
    }

    IEnumerator FlipCard()
    {
        WaitForSeconds _w = new WaitForSeconds(0.5f);
        animation.Play();  
        yield return _w;
        isFlipped = true;
        image.sprite = cardFace;
    }

     

}
