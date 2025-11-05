using UnityEngine;

public class JumpscareTrigger : MonoBehaviour
{
    public Animator jumpscareAnimator; 
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jumpscareAnimator.gameObject.SetActive(true);
            jumpscareAnimator.Play("Jumpscare_Anim");
            jumpscareAnimator.GetComponent<AudioSource>().Play();
        }
    }
}
