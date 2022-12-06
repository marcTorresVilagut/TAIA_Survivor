using UnityEngine;

public class CannonBall : MonoBehaviour {
    public GameObject explosion;
    public CannonController m_CannonController;

    private void Start() {
        m_CannonController = GetComponentInParent<CannonController>();
    }
    private void OnCollisionEnter(Collision collision) {
        if(collision.transform.CompareTag("agent") ||
           collision.transform.CompareTag("platform") ||
           collision.transform.CompareTag("collision_area")) {
        
            Explode();
        }
        
    }

    public void Explode() {

        m_CannonController.CannonBallHit();
        // Destroy this object
        Destroy(gameObject);
    }
}
