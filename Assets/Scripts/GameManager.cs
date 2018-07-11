using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

public class GameManager : MonoBehaviour
{
    public const string key = "VcAXByUfxX6WtzIm85Pu7xLXFTW2CArM";
    public const string GoodsUrl = "http://www.freesell.cn/apis/machine/products";
    public const string PriceUrl = "https://lankam.shop/config/uploadConfig";
    public const string LoginUrl = "https://lankam.shop/admin/authentication";
    public const string OnlineUrl = "https://lankam.shop/admin/manager/new/getuserlist";
    public const string ShutdownUrl = "https://lankam.shop/admin/manager/shutdown/machine";
    public const string UserListUrl = "https://lankam.shop/weixinServer/getwechatuserlist";
    public const string SendMessageUrl = "https://lankam.shop/sendmessagetouser";


    public List<GameData> GameList;
    public List<GoodsItem> GoodsList = new List<GoodsItem>();
    public string m_token;


    public Dictionary<GoodsItem, Dictionary<GameData, PriceData>> ConfigDic = new Dictionary<GoodsItem, Dictionary<GameData, PriceData>>();


    public Button UpLoadButton;
    public Button InfomationButton;
    public Button OnlineButton;
    public Button UpLoadXMl;
    public Button UpLoadNewsXml;

    public Button GetUserListButton;
    public Button SendButton;


    public Dropdown m_Dropdown;

    public GameObject GoodsUIPrefab;
    public GameObject GameUIPrefab;
    public GameObject UserItemPrefab;
    public Transform UserContent;
    public Transform Content;
    public GameObject WinObject;

    public GameObject Panel1;
    public GameObject Panel2;


    void Start()
    {
        UpLoadButton.onClick.AddListener(UpLoadXML);
        InfomationButton.onClick.AddListener(GetInfomation);
        OnlineButton.onClick.AddListener(ClickOnlineButton);
        UpLoadXMl.onClick.AddListener(UpLoadQRXML);
        GetUserListButton.onClick.AddListener(ClickUserListButton);
        SendButton.onClick.AddListener(ClickSendButton);
        UpLoadNewsXml.onClick.AddListener(ClickUploadNewsXml);
        //GameList = new List<GameData>() { new GameData("Basketball", 2001), new GameData("Fruit", 2002), new GameData("Nail", 2003) };



        Screen.SetResolution(1024, 768, false);
        m_Dropdown.value = 0;
        meachineID = "0009000001";
        m_Dropdown.onValueChanged.AddListener(ChangeAccount);

        StartCoroutine(CheckMoivesResource());


        ReadGameXml(Application.streamingAssetsPath + "/2.txt", out GameList);
        ReadTxt(Application.streamingAssetsPath + "/1.txt", out ConfigDic);

        CreateUI();
    }

    private void ClickUploadNewsXml()
    {
        string result = UpLoadWeChatNewsXMLurl();
        Debug.Log(result);
        if (result.Contains("upload ok"))
        {
            StartCoroutine(ShowWin());
        }
    }

    private void ClickSendButton()
    {
        string data = "message_id=0&send_user_id=oWt5V06102xXUHAwTQBQUezKq-iU";
        Encoding encoding = Encoding.UTF8;
        string retString = HttpWebResponseUtility.CreateGetHttpResponse(SendMessageUrl, data, null, null, null, m_token);
        Debug.Log(retString);
    }

    private void ClickUserListButton()
    {
        //string data = "page=" + id;
        Encoding encoding = Encoding.UTF8;
        string retString = HttpWebResponseUtility.CreateGetHttpResponse(UserListUrl, null, null, null, null, m_token);
        Debug.Log(retString);
    }

    private void ChangeAccount(int arg0)
    {
        if (arg0 == 0)
        {
            meachineID = "0009000001";
        }
        else
        {
            meachineID = "0009000002";
        }
    }

    private void ClickOnlineButton()
    {
        Panel1.SetActive(true);
        Panel2.SetActive(false);
        StartCoroutine(GetOnlineInfomation());
    }

    #region Online
    private List<UsersItem> userList = new List<UsersItem>();
    private IEnumerator GetOnlineInfomation()
    {
        string JsonResult = OnlineInferface(1);
        yield return JsonResult;
        Debug.Log(JsonResult);
        UsersRoot userlist = JsonMapper.ToObject<UsersRoot>(JsonResult);

        for (int i = 1; i <= userlist.pages; i++)
        {
            JsonResult = OnlineInferface(i);
            yield return JsonResult;
            Debug.Log(JsonResult);
            userlist = JsonMapper.ToObject<UsersRoot>(JsonResult);
            if (userlist!=null)
            {
                foreach (UsersItem item in userlist.users)
                {
                    if (!userList.Contains(item))
                    {
                        userList.Add(item);
                    }
                }
            }
        } 
        //if (userlist != null)
        //{
        //    userList = userlist.users;
        //}
        CreateUserItemUI();
    }
    public string OnlineInferface(int id)
    {
        string data = "page=" + id;
        Encoding encoding = Encoding.UTF8;
        string retString = HttpWebResponseUtility.CreateGetHttpResponse(OnlineUrl, data, null, null, null, m_token);
        return retString;
    }
    void CreateUserItemUI()
    {
        if (UserContent.childCount > 0)
        {
            foreach (Transform item in UserContent)
            {
                Destroy(item.gameObject);
            }
        }
        if (userList != null)
        {
            foreach (UsersItem item in userList)
            {
                GameObject userItem = Instantiate(UserItemPrefab);
                UserInfomation gameinfo = userItem.GetComponent<UserInfomation>();
                string state = item.isOnline == 1 ? "在线" : "不在线";
                bool ison = item.isOnline == 1 ? true : false;
                gameinfo.SetData(item.username, state, ison, (gameID, canplay) =>
                 {
                     UsersItem NeedChange = null;
                     foreach (UsersItem data in userList)
                     {
                         if (data.username == gameID)
                         {
                             NeedChange = data;
                         }
                     }
                     if (NeedChange != null && !canplay)
                     {
                         NeedChange.isOnline = 0;
                         ShutdownMeachine(NeedChange.username);
                     }
                 });
                userItem.transform.SetParent(UserContent);
            }
        }
    }

    private string ShutdownMeachine(string machineName)
    {
        Encoding encoding = Encoding.UTF8;
        IDictionary<string, string> parameters = new Dictionary<string, string>();
        parameters.Add("machinename", machineName);
        string retString = HttpWebResponseUtility.CreatePostHttpResponse(ShutdownUrl, parameters, null, null, encoding, null, m_token);
        return retString;
    }
    #endregion




    private void GetInfomation()
    {
        Panel1.SetActive(false);
        Panel2.SetActive(true);
        StartCoroutine(CheckGoodsResource());
    }

    private void UpLoadQRXML()
    {
        string result = UpLoadQRXMLurl();
        Debug.Log(result);
        if (result.Contains("upload ok"))
        {
            StartCoroutine(ShowWin());
        }
    }

    private void UpLoadXML()
    {
        CreateTxt(Application.streamingAssetsPath + "/1.txt", ConfigDic);
        CreateXML(Application.streamingAssetsPath + "/discountsInfo.xml");
        string result = UpLoadXml();
        if (result.Contains("upload ok"))
        {
            StartCoroutine(ShowWin());
        }
    }
    private IEnumerator ShowWin()
    {
        WinObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        WinObject.SetActive(false);
    }

    void CreateGameXMl<T>(string filePath, T t)
    {

        using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            XmlSerializer xml = new XmlSerializer(typeof(T));
            using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
            {
                xml.Serialize(sw, t);
            }
        }
    }
    void ReadGameXml<T>(string filePath, out T t)
    {
        if (!File.Exists(filePath))
        {
            t = default(T);
            return;
        }
        //XML反序列化
        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            XmlSerializer xml = new XmlSerializer(typeof(T));
            T lsPs = (T)xml.Deserialize(fs);
            t = lsPs;
        }
    }
    void ReadTxt(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return;
        }
        BinaryFormatter bf = new BinaryFormatter();
        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            Dictionary<GoodsItem, Dictionary<GameData, PriceData>> lsPs = (Dictionary<GoodsItem, Dictionary<GameData, PriceData>>)bf.Deserialize(fs);

            ConfigDic = lsPs;
        }
    }
    void ReadTxt<T>(string filePath, out T t)
    {
        if (!File.Exists(filePath))
        {
            t = default(T);
            return;
        }
        BinaryFormatter bf = new BinaryFormatter();
        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            T lsPs = (T)bf.Deserialize(fs);

            t = lsPs;
        }

    }
    void CreateTxt(string filePath)
    {
        BinaryFormatter bf = new BinaryFormatter();

        //创建文件流
        using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            bf.Serialize(fs, ConfigDic);
        }
    }
    void CreateTxt<T>(string filePath, T t)
    {
        BinaryFormatter bf = new BinaryFormatter();

        //创建文件流
        using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            bf.Serialize(fs, t);
        }
    }


    void CreateDictory()
    {
        if (ConfigDic != null)
        {
            ConfigDic.Clear();
        }
        else
        {
            ConfigDic = new Dictionary<GoodsItem, Dictionary<GameData, PriceData>>();
        }

        for (int i = 0; i < GoodsList.Count; i++)
        {
            Dictionary<GameData, PriceData> temp = new Dictionary<GameData, PriceData>();
            for (int j = 0; j < GameList.Count; j++)
            {
                temp.Add(GameList[j], new PriceData(true, 0.4f));
            }
            ConfigDic.Add(GoodsList[i], temp);
        }
        CreateUI();
    }

    void CreateUI()
    {
        if (Content.childCount > 0)
        {
            foreach (Transform item in Content)
            {
                Destroy(item.gameObject);
            }
        }
        if (ConfigDic != null)
        {
            foreach (GoodsItem item in ConfigDic.Keys)
            {
                GameObject goodUI = Instantiate(GoodsUIPrefab);
                PanelEditor panelData = goodUI.GetComponent<PanelEditor>();
                panelData.SetData(item, ConfigDic[item]);
                goodUI.transform.SetParent(Content);
            }
        }
    }



    void CreateXML(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        XmlDocument xmlDoc = new XmlDocument();
        XmlElement root = xmlDoc.CreateElement("root");
        if (ConfigDic != null)
        {
            int index = 1;
            foreach (GoodsItem item in ConfigDic.Keys)
            {
                XmlElement elmNew = xmlDoc.CreateElement("discountsInfo");
                elmNew.SetAttribute("goodsid", item.productId.ToString());
                int id = 1;
                int count = 0;
                foreach (GameData temp in ConfigDic[item].Keys)
                {
                    if (ConfigDic[item][temp].canplay)
                    {
                        string gameIndex = "gameid" + id;
                        string gamePriceIndex = "gameprice" + id;
                        elmNew.SetAttribute(gameIndex, temp.index.ToString());
                        elmNew.SetAttribute(gamePriceIndex, ConfigDic[item][temp].price.ToString());
                        count++;
                        id++;
                    }
                }
                elmNew.SetAttribute("gamenum", count.ToString());
                index++;
                root.AppendChild(elmNew);
            }
        }
        xmlDoc.AppendChild(root);
        xmlDoc.Save(filePath);
    }
    void ReadXml(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return;
        }
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(filePath);
        XmlNodeList nodeList = xmlDoc.SelectSingleNode("root").ChildNodes;
        //遍历每一个节点，拿节点的属性以及节点的内容
        foreach (XmlElement xe in nodeList)
        {
            Dictionary<string, float> temp = new Dictionary<string, float>();
            int gameCount = (xe.Attributes.Count - 2) / 2;
            string goodsid = xe.GetAttribute("goodsid");

            for (int i = 1; i <= gameCount; i++)
            {
                string gameid = "gameid" + i;
                string priceid = "gameprice" + i;
                string gameIdText = xe.GetAttribute(gameid);
                string priceIdText = xe.GetAttribute(priceid);
                float price = float.Parse(priceIdText);
                temp.Add(gameIdText, price);
            }
            //ConfigDic.Add(goodsid, temp);
        }
    }

    public string[] meachineIDArray;
    public string meachineID = "";
    private IEnumerator CheckGoodsResource()
    {
        GoodsList.Clear();
        foreach (string item in meachineIDArray)
        {
            if (!String.IsNullOrEmpty(item))
            {
                string appId = "00090";
                string meachineId = item;
                if (meachineId == "")
                {
                    yield break;
                }
                string sign = GetSign(new List<string> { appId, meachineId, key });
                string josn = ClientGoods(appId, meachineId, sign);
                yield return josn;
                GoodsJson goodsJson = JsonMapper.ToObject<GoodsJson>(josn);  //解析Json文件
                if (goodsJson != null)
                {
                    foreach (GoodsItem gooditem in goodsJson.data)
                    {
                        if (!GoodsList.Contains(gooditem))
                        {
                            GoodsList.Add(gooditem);
                        }
                    }
                }
            }
        }
        CreateDictory();
    }
    private IEnumerator CheckMoivesResource()
    {
        string JsonToken = LoginAccount("admin", "123456");
        yield return JsonToken;
        Debug.Log(JsonToken);
        Token token = JsonMapper.ToObject<Token>(JsonToken);
        if (token != null)
        {
            m_token = token.token;
        }
    }


    public string ClientGoods(string appId, string MachineId, string sign)
    {
        Encoding encoding = Encoding.UTF8;
        IDictionary<string, string> parameters = new Dictionary<string, string>();
        parameters.Add("appId", appId);
        parameters.Add("MachineId", MachineId);
        parameters.Add("Sign", sign);
        string retString = HttpWebResponseUtility.CreatePostHttpResponse(GoodsUrl, parameters, null, null, encoding, null);
        return retString;
    }
    public string UpLoadXml()
    {
        string token = m_token;
        if (token == null)
        {
            return "";
        }
        Encoding encoding = Encoding.UTF8;
        IDictionary<string, string> parameters = new Dictionary<string, string>();
        parameters.Add("name", "discountsInfo.xml");
        string retString = HttpWebResponseUtility.Upload_Request(PriceUrl, parameters, encoding, Application.streamingAssetsPath + "/discountsInfo.xml", "discountsInfo.xml", token);
        return retString;
    }

    public string UpLoadQRXMLurl()
    {
        string token = m_token;
        if (token == null)
        {
            return "";
        }
        if (!File.Exists(Application.streamingAssetsPath + "/qrcodeInfo.xml"))
        {
            return "";
        }
        //string xml = File.ReadAllText(Application.streamingAssetsPath + "/qrcodeInfo.xml");
        //string encodedXml = xml.Replace("&", "&amp;");/*.Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");*/
        //File.Delete(Application.streamingAssetsPath + "/qrcodeInfo.xml");
        //File.WriteAllText(Application.streamingAssetsPath + "/qrcodeInfo.xml", encodedXml);

        Encoding encoding = Encoding.UTF8;
        IDictionary<string, string> parameters = new Dictionary<string, string>();
        parameters.Add("name", "qrcodeInfo.xml");
        string retString = HttpWebResponseUtility.Upload_Request(PriceUrl, parameters, encoding, Application.streamingAssetsPath + "/qrcodeInfo.xml", "qrcodeInfo.xml", token);
        return retString;
    }

    public string UpLoadWeChatNewsXMLurl()
    {
        string token = m_token;
        if (token == null)
        {
            return "";
        }
        if (!File.Exists(Application.streamingAssetsPath + "/WeChatNews.xml"))
        {
            return "";
        }
        string xml = File.ReadAllText(Application.streamingAssetsPath + "/WeChatNews.xml");
        string encodedXml = xml.Replace("&", "&amp;");/*.Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");*/
        File.Delete(Application.streamingAssetsPath + "/WeChatNews.xml");
        File.WriteAllText(Application.streamingAssetsPath + "/WeChatNews.xml", encodedXml);

        Encoding encoding = Encoding.UTF8;
        IDictionary<string, string> parameters = new Dictionary<string, string>();
        parameters.Add("name", "WeChatNews.xml");
        string retString = HttpWebResponseUtility.Upload_Request(PriceUrl, parameters, encoding, Application.streamingAssetsPath + "/WeChatNews.xml", "WeChatNews.xml", token);
        return retString;
    }


    public string LoginAccount(string userName, string password)
    {
        Encoding encoding = Encoding.UTF8;
        IDictionary<string, string> parameters = new Dictionary<string, string>();
        parameters.Add("username", userName);
        parameters.Add("password", password);
        string retString = HttpWebResponseUtility.CreatePostHttpResponse(LoginUrl, parameters, null, null, encoding, null);
        return retString;
    }


    public static string GetSign(List<string> list)
    {
        list.Sort(string.CompareOrdinal);

        string value = string.Empty;
        foreach (string item in list)
        {
            value += item;
        }
        return md5(value).ToUpper();
    }
    public static string md5(string source)
    {
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        byte[] data = System.Text.Encoding.UTF8.GetBytes(source);
        byte[] md5Data = md5.ComputeHash(data, 0, data.Length);
        md5.Clear();

        string destString = "";
        for (int i = 0; i < md5Data.Length; i++)
        {
            destString += System.Convert.ToString(md5Data[i], 16).PadLeft(2, '0');
        }
        destString = destString.PadLeft(32, '0');
        return destString;
    }
}
public class Token
{
    public string token { get; set; }
}
public class GoodsJson
{
    public int errCode { get; set; }

    public string msg { get; set; }

    public List<GoodsItem> data { get; set; }
}
[Serializable]
public class GoodsItem
{
    /// <summary>
    /// 产品ID
    /// </summary>
    public double productId { get; set; }
    /// <summary>
    /// 康师傅经典奶茶
    /// </summary>
    public string productName { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string image { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public double stock { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public double price { get; set; }
}

[Serializable]
public class GameData
{
    public string name;
    public int index;

    public GameData() { }
    public GameData(string name, int index)
    {
        this.name = name;
        this.index = index;
    }
}

[Serializable]
public class PriceData
{
    public bool canplay;
    public float price;

    public PriceData() { }
    public PriceData(bool canplay, float price)
    {
        this.canplay = canplay;
        this.price = price;
    }
}

public class UsersItem
{
    /// <summary>
    /// 
    /// </summary>
    public string username { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int isOnline { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string manager { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string version { get; set; }
    /// <summary>
    /// 上海花园路
    /// </summary>
    public string location { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string useTime { get; set; }
}

public class UsersRoot
{
    /// <summary>
    /// 
    /// </summary>
    public int count { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int pages { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public List<UsersItem> users { get; set; }
}