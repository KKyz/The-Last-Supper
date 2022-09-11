using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SojaExiles

{

    public class Window_Slide : MonoBehaviour
    {

        public Animator WindowSlide;
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
            print("you are sliding the window open");
            WindowSlide.Play("Window Slide Open");
            open = true;
            yield return new WaitForSeconds(.5f);
        }

        IEnumerator closing()
        {
            print("you are sliding the window closed");
            WindowSlide.Play("Window Slide Close");
            open = false;
            yield return new WaitForSeconds(.5f);
        }


    }

}