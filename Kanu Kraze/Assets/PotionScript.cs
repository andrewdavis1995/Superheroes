using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{

    public class PotionScript : MonoBehaviour
    {
        public PlayerScript Player { get; internal set; }
        public int HealthBenefits;

        // Update is called once per frame
        void Update()
        {
            if (!Player) return;

            StartCoroutine(Transition());
        }

        private IEnumerator Transition()
        {
            yield return 0;

            float transitionDuration = .75f;

            var t = 0.0f;
            var startingPos = transform.position;
            while (t < 1f)
            {
                t += Time.deltaTime * (Time.timeScale / transitionDuration);

                var destination = new Vector3(Player.transform.position.x, Player.transform.position.y);

                transform.position = Vector3.Lerp(startingPos, destination, t);
                yield return 0;
            }
            Player.HealthGained(HealthBenefits);
            Destroy(gameObject);
        }
    }
}