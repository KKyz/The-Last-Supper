using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SojaExiles

{

    public class Shoji_DoorSlide : MonoBehaviour
    {

        public Animator ShojiSlide;
        public bool open;
        public Transform Player;

        void Start()
        {
            open = false;
        }

        void OnMouseOver()
        {
            {
                if (Player)
                {
                    float dist = Vector3.Distance(Player.position, transform.position);
                    if (dist < 15)
                    {
                        if (open == false)
                        {
                            if (Input.GetMouseButtonDown(0))
                            {
                                StartCoroutine(opening());
                            }
                        }
                        else
                        {
                            if (open == true)
                            {
                                if (Input.GetMouseButtonDown(0))
                                {
                                    StartCoroutine(closing());
                                }
                            }

                        }

                    }
                }

            }

        }

        IEnumerator opening()
        {
            print("you are sliding the door open");
            ShojiSlide.Play("Shoji_Open");
            open = true;
            yield return new WaitForSeconds(.5f);
        }

        IEnumerator closing()
        {
            print("you are sliding the door closed");
            ShojiSlide.Play("Shoji_Close");
            open = false;
            yield return new WaitForSeconds(.5f);
        }


    }

}