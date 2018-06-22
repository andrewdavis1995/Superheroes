using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{

    public class ConstructionScript : MonoBehaviour
    {
        public GameObject[] Platforms;
        public int BlocksRequired = 9;
        private FraserScript _fraserScript;
        public Vector3 EndPosition;
        public Vector3 EndSize;
        public Transform BaseElement;
        public GameObject Instruction;
        public GameObject NotEnough;
        public GameObject[] JumpStoppers;
        public RequiredIconFlickerScript FlickerIcon;

        // Use this for initialization
        void Start()
        {
            var fraser = GameObject.Find("Fraser");
            _fraserScript = fraser.GetComponent<FraserScript>();
        }

        public bool BuildTriggered(int availableBlocks)
        {
            if (availableBlocks >= BlocksRequired)
            {
                StartCoroutine(ShowPlatforms());
                _fraserScript.Building = true;
                FlickerIcon.gameObject.SetActive(false);
                return true;
            }
            return false;
        }

        IEnumerator ShowPlatforms()
        {
            for (var i = 0; i < Platforms.Length; i++)
            {
                Platforms[i].SetActive(true);
                yield return new WaitForSeconds(.8f);
            }
            _fraserScript.Building = false;

            _fraserScript.SetBuildState(false);

            foreach (var a in _fraserScript.Player.animators)
            {
                a.ResetTrigger("Stop");
            }

            StartCoroutine(MoveIntoPosition());
        }

        private IEnumerator MoveIntoPosition()
        {
            var t = 0.0f;
            var startingPos = BaseElement.position;
            var startingSize = BaseElement.localScale;
            while (t < 1.2f)
            {
                t += Time.deltaTime * (Time.timeScale / 1.2f);

                BaseElement.position = Vector3.Lerp(startingPos, EndPosition, t);
                BaseElement.localScale = Vector3.Lerp(startingSize, EndSize, t);
                yield return 0;
            }
            foreach (var stop in JumpStoppers)
            {
                Destroy(stop);
            }
            Destroy(gameObject);
        }
    }
}