using System.Collections;
using UnityEngine;

public class PatrolScript : MonoBehaviour
{

    public int MinX = 0;
    public int MaxX = 0;

    private bool _movingLeft = false;
    private bool _punching = false;
    public SpriteRenderer _renderer;
    public Animator _animator;

    public Sprite[] PunchImages;

    public float Speed = 1;

    // Use this for initialization
    void Start()
    {
        _animator = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();
    }

    IEnumerator DoPunch()
    {
        _animator.enabled = false;
        
        // loop through images
        for (int i = 0; i < PunchImages.Length; i++)
        {
            _renderer.sprite = PunchImages[i];
            yield return new WaitForSeconds(.025f);
        }

        _animator.enabled = true;
        _punching = false;
        yield return 0;
    }

    public void Punch()
    {
        _punching = true;
        StartCoroutine(DoPunch());
    }

    // Update is called once per frame
    void Update()
    {
        if (_punching) return;

        if (_movingLeft)
        {
            transform.Translate(-Speed * new Vector3(2, 0, 0) * Time.deltaTime);
            if (transform.position.x <= MinX)
            {
                _movingLeft = false;
                _renderer.flipX = false;
            }
        }
        else
        {
            transform.Translate(Speed * new Vector3(2, 0, 0) * Time.deltaTime);
            if (transform.position.x >= MaxX)
            {
                _movingLeft = true;
                _renderer.flipX = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Death")
        {
            Destroy(gameObject);
        }
    }
}
