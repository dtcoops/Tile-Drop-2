using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [Header("Targets")]
    [SerializeField] private Transform playerA;
    [SerializeField] private Transform playerB;
    [SerializeField] private Transform levelExit;

    [Header("Pan Settings")]
    [SerializeField] private float lerpSpeed = 4f;
    [SerializeField] private Vector3 panAxis = Vector3.right;

    [Tooltip("World units from exit to stop panning (so the exit tile stays in view). 0 = auto from orthographic half-width.")]
    [SerializeField] private float stopMargin = 0f;

    private Camera cam;
    private Vector3 panDir;
    private float startAxisPos;
    private float maxAxisPos = float.MaxValue;

    void Awake()
    {
        cam = GetComponent<Camera>();
        AutoFindPlayers();
    }

    void Start()
    {
        panDir = panAxis.normalized;
        startAxisPos = Vector3.Dot(transform.position, panDir);

        if (stopMargin <= 0f && cam != null && cam.orthographic)
            stopMargin = cam.orthographicSize * cam.aspect;

        RecalculateMax();
    }

    void LateUpdate()
    {
        if (playerA == null && playerB == null) return;

        Transform a = playerA != null ? playerA : playerB;
        Transform b = playerB != null ? playerB : playerA;

        Vector3 midpoint = (a.position + b.position) * 0.5f;
        float midAxisPos = Vector3.Dot(midpoint, panDir);
        float camAxisPos = Vector3.Dot(transform.position, panDir);

        // Only pan when the midpoint leads the camera center
        if (midAxisPos <= camAxisPos) return;

        float targetAxisPos = Mathf.Clamp(midAxisPos, startAxisPos, maxAxisPos);
        float delta = targetAxisPos - camAxisPos;
        transform.position = Vector3.Lerp(transform.position, transform.position + panDir * delta, lerpSpeed * Time.deltaTime);
    }

    private void RecalculateMax()
    {
        if (levelExit == null)
        {
            maxAxisPos = float.MaxValue;
            return;
        }

        float exitAxisPos = Vector3.Dot(levelExit.position, panDir);
        maxAxisPos = Mathf.Max(startAxisPos, exitAxisPos - stopMargin);
    }

    private void AutoFindPlayers()
    {
        if (playerA != null && playerB != null) return;

        var players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length >= 2)
        {
            playerA = players[0].transform;
            playerB = players[1].transform;
        }
        else if (players.Length == 1)
        {
            playerA = players[0].transform;
        }
    }
}
