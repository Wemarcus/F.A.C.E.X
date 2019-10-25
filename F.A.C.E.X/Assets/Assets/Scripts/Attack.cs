using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public Dialogues dialogues;
    public GameObject first_sword;
    public GameObject second_sword;
    public GameObject old_camera;
    public GameObject new_camera;

    private Animator anim;
    private bool check;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (dialogues.dialog_history[9] == 2 && !check)
        {
            Debug.Log("Sono entrato qui..");
            check = true;

            new_camera.SetActive(true);
            old_camera.SetActive(false);

            StartCoroutine(Sword());
        }
    }

    IEnumerator Sword()
    {
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(0.5f);
        first_sword.SetActive(false);
        second_sword.SetActive(true);
    }
}
