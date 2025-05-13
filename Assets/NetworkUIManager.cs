using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkUIManager : MonoBehaviour
{

    public Button hostButton;
    public Button joinButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hostButton.onClick.AddListener(Host);
        joinButton.onClick.AddListener(Join);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Host()
    {
        NetworkManager.Singleton.StartHost();
        Camera.main.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void Join()
    {
        NetworkManager.Singleton.StartClient();
        Camera.main.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
