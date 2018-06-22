using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{

    public class ComicScript : MonoBehaviour
    {
        public Vector3[] Positions;
        public float[] Zooms;
        private int _index = 0;
        public Camera TheCamera;

        private readonly float _transitionDuration = 1.2f;

        // Use this for initialization
        void Start()
        {
            StartCoroutine(Transition());
        }
        
        IEnumerator Transition()
        {
            var t = 0.0f;
            var startingPos = transform.position;
            var startingZoom = new Vector2(TheCamera.orthographicSize, 0);
            while (t < 1f)
            {
                t += Time.deltaTime * (Time.timeScale / _transitionDuration);

                var destination = new Vector3(Positions[_index].x, Positions[_index].y, -5);
                var destinationZoom = new Vector2(Zooms[_index], 0);

                transform.position = Vector3.Lerp(startingPos, destination, t);
                TheCamera.orthographicSize = Vector2.Lerp(startingZoom, destinationZoom, t).x;
                yield return 0;
            }
            yield return new WaitForSeconds(_index == 0 ? 1 : 3);
            _index++;
            if (_index < Positions.Length) { StartCoroutine(Transition()); }
            else { SceneManager.LoadScene(1); }
        }

    }
}