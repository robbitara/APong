using UnityEngine;
using UnityEngine.UI;

public class BallPhysics : MonoBehaviour {

    public Vector2 ball_speed;
    public bool ctrl;

    Rigidbody2D rb2d;
    Vector2 CollisionPos;
    GameManager manager;
    float maxSpeed, incremento;

    void Awake() {
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    void Start() {
        maxSpeed = 5.5f;
        incremento = 1.01f;
        ResetBall();
    }

    private void FixedUpdate() {
        if (rb2d.velocity.magnitude > maxSpeed && !ctrl) {
            ctrl = true;
            rb2d.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    void OnCollisionEnter2D(Collision2D col) {

        if (col.gameObject.tag == "Player") {
            GameManager.points += 10;
            manager.Animate();
            GameManager.PlaySound(GetComponent<AudioSource>(), 0f);
        }

        if (ctrl) {
            rb2d.velocity = rb2d.velocity.normalized * maxSpeed;
        } else {

            ball_speed *= incremento;

            rb2d.velocity = rb2d.velocity.normalized * ball_speed.magnitude;
            print("Velocity: " + rb2d.velocity + "\nMagnitude: " + rb2d.velocity.magnitude);
        }

        // Controllo vicinanza collisioni
        float angle = GetAngle(rb2d.position, rb2d.gameObject.transform.parent.position);                           // calcolo l'angolo tra la posizione della palla e il centro dell'area di gioco
        if (Vector2.Distance(rb2d.position, CollisionPos) < 3f) {                                                   // se la distanza tra l'ultima collisione e la nuova collisione è minore di x
            rb2d.velocity = new Vector2(rb2d.velocity.x * Mathf.Cos(angle), rb2d.velocity.y * Mathf.Sin(angle));    // allora mando la palla verso il centro dell'area di gioco
        }

        CollisionPos = rb2d.position;

    }

    public void ResetBall() {
        rb2d.GetComponent<SpriteRenderer>().color = Color.white;

        ball_speed = new Vector2(0, 4f);
        ctrl = false;
        CollisionPos = rb2d.position;
        transform.localPosition = Vector2.zero;
        rb2d.velocity = Vector2.zero;
    }

    float GetAngle(Vector2 a, Vector2 b) {
        return ((Mathf.Atan2(b.y - a.y, b.x - a.x)));
    }
}