using UnityEngine;

public class PatrolScript : MonoBehaviour
{

    public EnemyScript Enemy;

    public int MinX = 0;
    public int MaxX = 0;

    private bool _movingLeft = false;
    public SpriteRenderer _renderer;

    public float Speed = 1;

    // Use this for initialization
    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Enemy.Punching) return;

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
}
