using UnityEngine;
using UnityEngine.EventSystems;
using Edit;

public class DragAndDrop : MonoBehaviour
{
    private bool isDragging = true;

    private Material mat;
    private Color originalColor;

    private Rigidbody2D rigidbody2D;
    private BoxCollider2D machineCollider;

    [SerializeField] private EditManager editManager;
    

    private bool spawn = true;
    private void OnMouseDown()
    {
        if (spawn && !CanPlaceMachine()) { 
            spawn = false;
            Destroy(this.gameObject);
        }
        else if (spawn) {
            spawn = false;
        }
        isDragging = true;
        editManager.ResetActiveMachineType();
    }

    private void OnMouseUp()
    {
        if (!spawn && !CanPlaceMachine())
        {
            Destroy(this.gameObject);
        }
        isDragging = false;
    }

    private void OnMouseOver(){
        if (Input.GetMouseButtonDown(1))
        {
            Destroy(this.gameObject);
        }
    }

    public bool CanPlaceMachine()
    {
        Vector2 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        this.gameObject.SetActive(false);
        bool ableToSpawnMachine = Physics2D.OverlapBox(position + machineCollider.offset, machineCollider.size, 0) == null;
        this.gameObject.SetActive(true);

        return ableToSpawnMachine;
    }

    private void Start()
    {
        mat = this.gameObject.GetComponent<SpriteRenderer>().material;
        originalColor = mat.color;

        rigidbody2D = this.gameObject.GetComponent<Rigidbody2D>();
        machineCollider = GetComponent<BoxCollider2D>();
        //rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY
                        | RigidbodyConstraints2D.FreezeRotation;
    }

    private void Update()
    {
        if (isDragging) {
            this.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 4;
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            transform.Translate(mousePosition);
            if (machineCollider.IsTouchingLayers(-1)) {
                mat.color = Color.red;
            }
            else {
                mat.color = originalColor;
            }
        } 
        else {
            mat.color = originalColor;
            this.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 3;
        }
    }
}
