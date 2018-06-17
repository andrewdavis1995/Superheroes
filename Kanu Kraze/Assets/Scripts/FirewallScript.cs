using System.Collections;
using UnityEngine;

public class FirewallScript : MonoBehaviour
{
    public BoxCollider2D WallCollider;
    public bool Pressed;
    public GameObject Instruction;
    public Sprite[] TvImages;
    public SpriteRenderer Tv;
    public Animator TvAnimator;
    public RequiredIconFlickerScript FlickerIcon;

    // Use this for initialization
    void Start()
    {

    }

    public void Activate(AndrewScript andrew)
    {
        FlickerIcon.gameObject.SetActive(false);
        Pressed = true;
        Instruction.SetActive(false);
        StartCoroutine(Release(andrew));
    }

    IEnumerator Release(AndrewScript andrew)
    {
        andrew.Hack();
        Tv.sprite = TvImages[1];
        yield return new WaitForSeconds(2.5f);
        TvAnimator.enabled = true;
        yield return new WaitForSeconds(2);
        TvAnimator.enabled = false;
        Tv.sprite = TvImages[2];
        yield return new WaitForSeconds(1);
        WallCollider.isTrigger = true;
        andrew.DoneHacking();
    }
}
