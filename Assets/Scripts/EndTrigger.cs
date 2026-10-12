using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndTrigger : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private FollowLeader followLeader;
    [SerializeField] private Transform cameraFollowTarget;
    public AudioSource audioSourceRound;
    public AudioClip WinAudio;

    [Header("UI")]
    [SerializeField] private GameObject winUI;
    [SerializeField] private Image coverUI;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private TextMeshProUGUI tmpNameGame;
    [SerializeField] private TextMeshProUGUI tmpAuthorGame;
    [SerializeField] private OpenURLInBrowser downloadButtonUrlScript;

    [Header("Cover Loader")]
    public ImageDownloader coverLoader;

    private GameObject firstFinisher;

    private void Start()
    {
        if (virtualCamera == null) virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        if (followLeader == null) followLeader = FindObjectOfType<FollowLeader>();

        if (winUI == null)
        {
            winUI = FindInactiveByTag("WinCircle");
        }
        else
        {
            if (tmpNameGame == null) tmpNameGame = FindInactiveByTag("UINameGame").GetComponent<TextMeshProUGUI>();
            if (tmpAuthorGame == null) tmpAuthorGame = FindInactiveByTag("UIAuthorGame").GetComponent<TextMeshProUGUI>();
            if (coverLoader == null) coverLoader = FindInactiveByTag("Cover").GetComponent<ImageDownloader>();
            if (downloadButtonUrlScript == null) downloadButtonUrlScript = winUI.GetComponentInChildren<OpenURLInBrowser>();
        }
        if (winUI != null) winUI.SetActive(false);
    }

    private GameObject FindInactiveByTag(string tag)
    {
        var roots = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (var root in roots)
        {
            var allChildren = root.GetComponentsInChildren<Transform>(true);
            foreach (var child in allChildren)
                if (child.CompareTag(tag)) return child.gameObject;
        }
        return null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (firstFinisher != null) return;

        if (audioSourceRound != null)
            audioSourceRound.Stop();

        firstFinisher = other.gameObject;
        Debug.Log($"{firstFinisher.name} финишировал!");

        if (followLeader != null) followLeader.enabled = false;
        if (virtualCamera != null && cameraFollowTarget != null)
            virtualCamera.Follow = cameraFollowTarget;

        GameCircle gameCircle = other.GetComponent<GameCircle>();
        if (gameCircle != null)
        {
            if (downloadButtonUrlScript != null)
            {
                downloadButtonUrlScript.pathGame = gameCircle.Url;
                downloadButtonUrlScript.pathGameSubmit = "https://oknoweb.ru/submissions/" + gameCircle.Id;
            }
            if (tmpNameGame != null) tmpNameGame.text = gameCircle.Name;
            if (tmpAuthorGame != null) tmpAuthorGame.text = $"By:{gameCircle.Author}";

            if (coverLoader != null)
            {
                string coverUrl = $"https://oknoweb.ru/api/submissions/{gameCircle.Id}/cover_horizontal";
                StartCoroutine(coverLoader.DownloadImage(coverUrl));
            }
            
            
        }

        if (WinAudio != null)
        {
            AudioSource.PlayClipAtPoint(WinAudio, transform.position);
        }
        if (winUI != null) winUI.SetActive(true);
    }
}