using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MatthewScript : MonoBehaviour
{
    public PlayerScript PlayerScript;
    private int _potionsCollected = 0;
    private float _regenerateCount = 0;
    public Text PotionText;

    public Transform Potion;
    public Sprite[] PotionImages;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!PlayerScript.Alive) return;

        _regenerateCount += Time.deltaTime;
        if (_regenerateCount >= 5)
        {
            _regenerateCount = 0;
            PlayerScript.HealthGained(3);
        }

        if (!PlayerScript.Active) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartCoroutine(SendPotions(1));
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StartCoroutine(SendPotions(2));
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            StartCoroutine(SendPotions(3));
        }
    }

    public IEnumerator SendPotions(int quantity)
    {
        if (quantity <= _potionsCollected)
        {
            var players = GameObject.Find("GameController").GetComponent<GameControllerScript>().Players.Where(p => p.Alive && !p.Complete && p.name != "Matthew");
            foreach (var item in players)
            {
                var obj = Instantiate(Potion, transform.position + new Vector3(0, 2, -1), Quaternion.identity, null);
                obj.GetComponent<PotionScript>().Player = item;
                _potionsCollected -= quantity;
                PotionText.text = _potionsCollected.ToString();
                obj.GetComponent<PotionScript>().HealthBenefits = 12 * quantity;
                var random = Random.Range(0, PotionImages.Length);
                obj.GetComponent<SpriteRenderer>().sprite = PotionImages[random];
            }
        }
        yield return 0;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Potion")
        {
            _potionsCollected++;
            PotionText.text = _potionsCollected.ToString();
            Destroy(collision.gameObject);
        }
    }
}
