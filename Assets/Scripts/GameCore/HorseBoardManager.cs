using DataModels;
using UnityEngine;
using UnityEngine.Events;

public class HorseBoardManager : MonoBehaviour
{
    [SerializeField]
    private Camera boardCamera;

    [SerializeField]
    private Collider boardCollider;
    [Header("Anchors")]
    [SerializeField]
    private Transform StartAnchorTransform;
    [SerializeField]
    private Transform EndAnchorTransform;

    private Vector3 deltaGrid;
    private Vector3 deltaX;
    private Vector3 deltaY;
    
    // Members
    public HorseBoard Board;

    public UnityAction<HorsePosition> OnBoardClickAtPosition;
    
    public GameObject StonePrefab;
    public GameObject StickPrefab;

    // States
    private bool ignoreInputChecking = false;

    private void Awake()
    {
        boardCamera ??= Camera.main;
        boardCamera ??= GetComponentInParent<Camera>();
        boardCollider ??= GetComponent<Collider>();
        
        Board = new HorseBoard();
    }

    void Start()
    {
        PreCalculating();
        OnBoardClickAtPosition += TestPutDownHorse;
    }


    private bool isStone;
    void TestPutDownHorse(HorsePosition position)
    {
        if (!position.IsInBoard()) return;
        var horseWorldPosition = StartAnchorTransform.position + deltaX * position.X + deltaY * position.Y;
        var quaternion = Quaternion.Euler(45, 45, 0);
        var prefab = isStone? StonePrefab : StickPrefab;
        GameObject horse = Instantiate(prefab, horseWorldPosition, quaternion);
        Board[position.X, position.Y] = isStone? HorseType.Stone : HorseType.Stick;
        Board.CheckStatus(position, gotty =>
        {
            if (gotty == GottyType.None) return;
            Debug.Log($"[{(isStone?"Stone":"Stick")}] Gotty: {gotty}");
        });
        isStone = !isStone;
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        
        Debug.DrawLine(StartAnchorTransform.position, StartAnchorTransform.position + deltaX, Color.red);
        Debug.DrawLine(StartAnchorTransform.position, StartAnchorTransform.position + deltaY, Color.green);
    }

    private void PreCalculating()
    {
        var cameraTransform = boardCamera.transform;
        var cameraUp = cameraTransform.up;
        var cameraRight = cameraTransform.right;
        
        deltaGrid = EndAnchorTransform.position - StartAnchorTransform.position;
        deltaGrid *= 0.2f;
        deltaX = Vector3.Dot(deltaGrid, cameraRight) * cameraRight;
        deltaY = Vector3.Dot(deltaGrid, cameraUp) * cameraUp;
        
    }


    private void CheckInput()
    {
        if (ignoreInputChecking) return;
        var pointerPosition = Vector3.zero;
        var hasPointer = false;
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            hasPointer = true;
            pointerPosition = Input.GetTouch(0).position;
        }
        else if (Input.GetMouseButtonDown(0))
        {
            hasPointer = true;
            pointerPosition = Input.mousePosition;
        }
        
        if (!hasPointer) return;
        
        var ray = boardCamera.ScreenPointToRay(pointerPosition);
        if (Physics.Raycast(ray, out var hit, 100))
        {
            if (hit.collider != boardCollider) return;
            PreCalculating();
            var hitPoint = hit.point;
            
            var relativePosition = hitPoint - StartAnchorTransform.position;
            var x = Vector3.Dot(relativePosition, deltaX) / deltaX.sqrMagnitude;
            var y = Vector3.Dot(relativePosition, deltaY) / deltaY.sqrMagnitude;
            var i = Mathf.RoundToInt(x);
            var j = Mathf.RoundToInt(y);
            if (i < 0 || i > 5 || j < 0 || j > 5) return;
            OnBoardClickAtPosition?.Invoke(new HorsePosition(i, j));
            // Debug.Log($"(x, y) = ({x}, {y})  i: {i}, j: {j}");
            // GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            // sphere.transform.localScale = Vector3.one * 0.2f;
            // sphere.transform.position = StartAnchorTransform.position + deltaX * i + deltaY * j;
        }
    }
}
