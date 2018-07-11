using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PanelEditor : MonoBehaviour
{

    public GameObject GamePrefab;
    public Text GoodName;
    //public RawImage rawImage;
    private Transform gameParent;

    public void SetData(GoodsItem item, Dictionary<GameData, PriceData> gamelist)
    {
        GoodName.text = item.productName + "\n￥：" + item.price;

        #region MyRegion
        //string url = item.image;
        //string GoodsName = Path.GetFileName(url);
        //string localurl = Application.streamingAssetsPath + "/GoodsData/" + GoodsName;
        //if (File.Exists(localurl))
        //{
        //    Texture2D t = LoadByIO(localurl);
        //    rawImage.texture = t;
        //}
        //else
        //{
        //    StartCoroutine(LoadPicture(url));
        //}
        #endregion


        foreach (GameData temp in gamelist.Keys)
        {
            GameObject gameUI = Instantiate(GamePrefab);
            gameUI.transform.SetParent(gameParent);
            GameInfomation gameinfo = gameUI.GetComponent<GameInfomation>();
            gameinfo.SetData(temp.name, gamelist[temp], (gameID, changevalue, canplay) =>
            {
                GameData NeedChange = null;
                foreach (GameData data in gamelist.Keys)
                {
                    if (data.name.ToString() == gameID)
                    {
                        NeedChange = data;
                    }
                }
                gamelist[NeedChange] = new PriceData(canplay, float.Parse(changevalue));
            });
        }
    }

    //IEnumerator LoadPicture(string filePath)
    //{
    //    using (WWW www = new WWW(filePath))
    //    {
    //        yield return www;
    //        if (string.IsNullOrEmpty(www.error))
    //        {
    //            Texture2D t = www.texture;
    //            rawImage.texture = t;
    //            LoadConfig.GetInstance().m_downLoader.StartDownload(filePath);
    //        }
    //        else
    //        {
    //            Debug.Log(www.error);
    //        }
    //    }
    //}

    void Awake()
    {
        gameParent = transform.Find("GameList");
    }
    public static Texture2D LoadByIO(string filepath)
    {
        //double startTime = (double)Time.time;
        //创建文件读取流
        FileStream fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read);
        fileStream.Seek(0, SeekOrigin.Begin);
        //创建文件长度缓冲区
        byte[] bytes = new byte[fileStream.Length];
        //读取文件
        fileStream.Read(bytes, 0, (int)fileStream.Length);
        //释放文件读取流
        fileStream.Close();
        fileStream.Dispose();
        fileStream = null;

        //创建Texture
        int width = 300;
        int height = 372;
        Texture2D texture = new Texture2D(width, height);
        texture.LoadImage(bytes);
        return texture;
    }

}
