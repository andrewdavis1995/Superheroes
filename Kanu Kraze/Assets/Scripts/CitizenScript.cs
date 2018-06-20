using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{

    public class CitizenScript : MonoBehaviour
    {
        public int Health = 100;
        public SpriteRenderer Renderer;
        public Sprite[] Sprites;
        public Transform[] Rubble;
        public ShipScript Ship;
        public string Name;
        public Sprite Reward;
        public bool Male;
        public string GiftName;
        public Image SummaryView;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public bool Hit(int hitPoints)
        {
            Health -= hitPoints;
            StartCoroutine(CreateRubble());
            if (Health < 0)
            {
                return true;
            }
            return false;
        }

        private IEnumerator CreateRubble()
        {
            yield return new WaitForSeconds(.3f);
            var iNumRubble = Random.Range(2, 9);
            for (var i = 0; i < iNumRubble; i++)
            {
                var iRubble = Random.Range(0, Rubble.Length);
                var obj = Instantiate(Rubble[iRubble], transform.position + new Vector3(Random.Range(-1f, 1f), 1f, -1f), Quaternion.identity, transform);
                obj.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(Random.Range(-80, 80), Random.Range(40, 100)));
            }

            if (Health <= 0)
            {
                StartCoroutine(SummonShip());
                Renderer.sprite = Sprites[0];
            }
            else if (Health < 20)
            {
                Renderer.sprite = Sprites[3];
            }
            else if (Health < 50)
            {
                Renderer.sprite = Sprites[2];
            }
            else if (Health < 80)
            {
                Renderer.sprite = Sprites[1];
            }
        }

        IEnumerator SummonShip()
        {
            var controller = GameObject.Find("UI Controller").GetComponent<UiScript>();
            controller.ShowCitizen(this);

            Ship.Citizen = gameObject;
            yield return 0;
        }

        public void ShipPickup()
        {
            SummaryView.color = new Color(1, 1, 1, 1);
            Ship.Activate(new Vector3(transform.position.x + 1.44f, transform.position.y + 6.917f, -20), true);
        }
    }
}