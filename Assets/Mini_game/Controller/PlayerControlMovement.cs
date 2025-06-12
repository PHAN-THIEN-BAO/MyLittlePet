using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerControlMovement : MonoBehaviour
{
    [SerializeField] float speed = 5f;

    public InputAction LeftAction;
    public InputAction RightAction;
    public InputAction TopAction;
    public InputAction BottomAction;

    private Rigidbody2D rigidbody2D;
    private Vector2 move;

    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();

        LeftAction.Enable();
        RightAction.Enable();
        TopAction.Enable();
        BottomAction.Enable();
    }

    void Update()
    {
        move = Vector2.zero;

        if (LeftAction.IsPressed())
        {
            move.x = -1;
        }
        else if (RightAction.IsPressed())
        {
            move.x = 1;
        }

        if (BottomAction.IsPressed())
        {
            move.y = -1;
        }
        else if (TopAction.IsPressed())
        {
            move.y = 1;
        }
    }

    void FixedUpdate()
    {
        Vector2 newPos = rigidbody2D.position + move.normalized * (speed * Time.fixedDeltaTime);
        rigidbody2D.MovePosition(newPos);
    }
}
