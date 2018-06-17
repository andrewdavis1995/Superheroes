using UnityEngine;
using UnityEngine.UI;

public class MatthewScript : MonoBehaviour
{
    public PlayerScript PlayerScript;
    private int _potionsCollected = 0;
    private float _regenerateCount = 0;
    public Text PotionText;

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
