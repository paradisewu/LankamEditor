using UnityEngine;
using System.Collections;

public class LoadConfig : MonoBehaviour
{
    private static LoadConfig _instance;
    public static LoadConfig GetInstance()
    {
        if (_instance == null)
        {
            _instance = Camera.main.gameObject.AddComponent<LoadConfig>();
        }
        return _instance;
    }

    public LoadALLData m_downLoader;

    public void Awake()
    {
        _instance = this;
        m_downLoader = new LoadALLData();
        m_downLoader.Init();
    }
    public void Update()
    {
        if (m_downLoader == null)
        {
            return;
        }
        m_downLoader.UpdateDownload();
    }
}
