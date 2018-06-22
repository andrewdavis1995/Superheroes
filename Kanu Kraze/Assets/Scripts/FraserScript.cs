using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{

    public class FraserScript : MonoBehaviour
    {
        public PlayerScript Player;
        private int _holdingLegos = 0;
        public bool Building = false;
        public GameObject Builder;
        private ConstructionScript _inBuildZone;
        public Text BricksText;
        public Image BricksImage;
        
        public void Die()
        {
            BricksText.gameObject.SetActive(false);
            BricksImage.gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.E) && !Building && Player.Active && _inBuildZone)
            {
                var success = _inBuildZone.BuildTriggered(_holdingLegos);
                if (success)
                {
                    _holdingLegos -= _inBuildZone.BlocksRequired;
                    BricksText.text = _holdingLegos.ToString();
                    SetBuildState(true);
                    _inBuildZone.Instruction.SetActive(false);
                    _inBuildZone = null;
                    transform.Translate(new Vector3(-1f, 0, 0));
                }
                else
                {
                    Debug.Log("Not enough bricks to build!");
                    Debug.Log((_inBuildZone.BlocksRequired - _holdingLegos) + " more needed.");
                }
            }
        }

        public void SetBuildState(bool state)
        {
            Builder.SetActive(state);
            foreach (var renderer in Player.Renderers)
            {
                renderer.enabled = !state;
            }
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.tag == "BuildTrigger" && Player.Active)
            {
                _inBuildZone = collision.GetComponentInChildren<ConstructionScript>();

                if (_holdingLegos >= _inBuildZone.BlocksRequired)
                {
                    _inBuildZone.Instruction.SetActive(true);
                }
                else
                {
                    _inBuildZone.NotEnough.SetActive(true);
                }
            }

            if (collision.transform.tag == "Lego")
            {
                _holdingLegos++;
                BricksText.text = _holdingLegos.ToString();
                Destroy(collision.gameObject);
            }
        }
        void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.transform.tag == "BuildTrigger" && Player.Active)
            {
                _inBuildZone = null;
                collision.GetComponentInChildren<ConstructionScript>().Instruction.SetActive(false);
                collision.GetComponentInChildren<ConstructionScript>().NotEnough.SetActive(false);
            }
        }
    }
}