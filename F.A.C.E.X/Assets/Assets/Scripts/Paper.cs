using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paper : MonoBehaviour
{
    public float radius = 5.0F;
    public float power = 40.0F;
    public AudioSource sound;
    public GameObject coffee;

    private int count;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Policeman")
        {
            if (count == 2)
            {
                Vector3 explosionPos = transform.position;
                Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);

                foreach (Collider hit in colliders)
                {
                    Rigidbody rb = hit.GetComponent<Rigidbody>();

                    if (rb != null)
                        rb.AddExplosionForce(power, explosionPos, radius, 3.0F);
                }

                sound.Play();
                coffee.GetComponent<Animation>().Play();

                count = 1;
            }
            else
            {
                count++;
            }
        }
    }
}
