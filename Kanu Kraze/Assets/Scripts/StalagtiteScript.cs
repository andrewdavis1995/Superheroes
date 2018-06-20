using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{

    public class StalagtiteScript : MonoBehaviour
    {
        public Transform[] Rubble;
        private IEnumerator CreateRubble(bool player)
        {
            var colliders = GetComponents<Collider2D>();
            foreach (var col in colliders) { col.enabled = false; }
            var rend = GetComponent<SpriteRenderer>();
            if (rend) rend.enabled = false;

            yield return new WaitForSeconds(player ? .1f : 0);
            var iNumRubble = UnityEngine.Random.Range(5, 11);
            for (var i = 0; i < iNumRubble; i++)
            {
                var iRubble = UnityEngine.Random.Range(0, Rubble.Length);
                var obj = Instantiate(Rubble[iRubble], transform.position + new Vector3(UnityEngine.Random.Range(-1f, 1f), player ? 2f : 0, -1f), Quaternion.identity, transform);
                obj.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(UnityEngine.Random.Range(-80, 80), UnityEngine.Random.Range(40, 100)));
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag == "Platform" || collision.gameObject.tag == "Floor")
            {
                Crash(false);
            }
        }

        public void Crash(bool player)
        {
            StartCoroutine(CreateRubble(player));
            StartCoroutine(DestroyObject());
        }

        private IEnumerator DestroyObject()
        {
            yield return new WaitForSeconds(2);
            Destroy(gameObject);
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}