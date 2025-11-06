using UnityEngine;

public class JumpscareTrigger : MonoBehaviour
{
    public Animator jumpscareAnimator;
    public float scareDuration = 180f;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jumpscareAnimator.gameObject.SetActive(true);
            jumpscareAnimator.Play("Jumpscare_Anim");
            jumpscareAnimator.GetComponent<AudioSource>().Play();

            StartCoroutine(DisableJumpscare());
        }
    }

    private System.Collections.IEnumerator DisableJumpscare()
    {
        yield return new WaitForSeconds(scareDuration);
        jumpscareAnimator.gameObject.SetActive(false);
    }
}
