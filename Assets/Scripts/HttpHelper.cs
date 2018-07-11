using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using UnityEngine;

class HttpHelper
{
    public HttpHelper()
    {
        cookie = new CookieContainer();
    }
    CookieContainer cookie;

    private string HttpPost(string Url, string postDataStr)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
        request.Method = "POST";
        request.ContentType = "application/x-www-form-urlencoded";
        request.ContentLength = Encoding.UTF8.GetByteCount(postDataStr);
        request.CookieContainer = cookie;
        Stream myRequestStream = request.GetRequestStream();
        StreamWriter myStreamWriter = new StreamWriter(myRequestStream, Encoding.GetEncoding("gb2312"));
        myStreamWriter.Write(postDataStr);
        myStreamWriter.Close();

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();

        response.Cookies = cookie.GetCookies(response.ResponseUri);
        Stream myResponseStream = response.GetResponseStream();
        StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
        string retString = myStreamReader.ReadToEnd();
        myStreamReader.Close();
        myResponseStream.Close();

        return retString;
    }

    public string HttpGet(string Url, string postDataStr)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
        request.Method = "GET";
        request.ContentType = "text/html;charset=UTF-8";

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        Stream myResponseStream = response.GetResponseStream();
        StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
        string retString = myStreamReader.ReadToEnd();
        myStreamReader.Close();
        myResponseStream.Close();

        return retString;
    }

    public static string HttpPostData(string url, string param)
    {
        var result = string.Empty;
        //注意提交的编码 这边是需要改变的 这边默认的是Default：系统当前编码
        byte[] postData = Encoding.UTF8.GetBytes(param);

        // 设置提交的相关参数 
        HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
        Encoding myEncoding = Encoding.UTF8;
        request.Method = "POST";
        request.KeepAlive = false;
        request.AllowAutoRedirect = true;
        request.ContentType = "application/x-www-form-urlencoded";
        request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 2.0.50727; .NET CLR  3.0.04506.648; .NET CLR 3.5.21022; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729)";
        request.ContentLength = postData.Length;

        // 提交请求数据 
        System.IO.Stream outputStream = request.GetRequestStream();
        outputStream.Write(postData, 0, postData.Length);
        outputStream.Close();

        HttpWebResponse response;
        Stream responseStream;
        StreamReader reader;
        string srcString;
        response = request.GetResponse() as HttpWebResponse;
        responseStream = response.GetResponseStream();
        reader = new System.IO.StreamReader(responseStream, Encoding.GetEncoding("UTF-8"));
        srcString = reader.ReadToEnd();
        result = srcString;   //返回值赋值
        reader.Close();

        return result;
    }

    //private static string HttpPostData(string url, int timeOut, Dictionary<string, string> imgDic, NameValueCollection stringDict)
    //{
    //    LogEntry entry = new LogEntry("发送图片开始-HttpPostData -- " + stringDict["open_ids"], 1);
    //    LogStub.Log(entry);

    //    var firstImg = imgDic.FirstOrDefault();
    //    string fileKeyName = firstImg.Key;
    //    string filePath = firstImg.Value;

    //    string responseContent;
    //    var memStream = new MemoryStream();
    //    var webRequest = (HttpWebRequest)WebRequest.Create(url);
    //    // 边界符
    //    var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
    //    // 边界符
    //    var beginBoundary = Encoding.ASCII.GetBytes("--" + boundary + "\r\n");
    //    //var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

    //    // 最后的结束符
    //    var endBoundary = Encoding.ASCII.GetBytes("--" + boundary + "--\r\n");

    //    // 设置属性
    //    webRequest.Method = "POST";
    //    webRequest.Timeout = timeOut;
    //    webRequest.ContentType = "multipart/form-data; boundary=" + boundary;

    //    // 写入文件
    //    const string filePartHeader =
    //        "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" +
    //         "Content-Type: application/octet-stream\r\n\r\n";
    //    var header = string.Format(filePartHeader, fileKeyName, filePath);
    //    var headerbytes = Encoding.UTF8.GetBytes(header);

    //    memStream.Write(beginBoundary, 0, beginBoundary.Length);
    //    memStream.Write(headerbytes, 0, headerbytes.Length);

    //    //var buffer = new byte[1024];
    //    //int bytesRead; // =0

    //    //while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
    //    //{
    //    //    memStream.Write(buffer, 0, bytesRead);
    //    //}

    //    WebClient wc = new WebClient();
    //    byte[] buffer = wc.DownloadData(filePath);
    //    memStream.Write(buffer, 0, buffer.Length);

    //    //第二章图片
    //    //memStream.Write(beginBoundary, 0, beginBoundary.Length);


    //    //var aaa = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
    //    //memStream.Write(aaa, 0, aaa.Length);

    //    string imgName = string.Empty;
    //    string imgPath = string.Empty;
    //    foreach (var img in imgDic.Where(p => p.Key != fileKeyName))
    //    {
    //        imgName = img.Key;
    //        imgPath = img.Value;

    //        string nxetFileFormat = "\r\n--" + boundary + "\r\n" + filePartHeader;

    //        header = string.Format(nxetFileFormat, imgName, imgPath);
    //        headerbytes = Encoding.UTF8.GetBytes(header);

    //        memStream.Write(headerbytes, 0, headerbytes.Length);
    //        //fileStream = new FileStream(imgPath, FileMode.Open, FileAccess.Read);
    //        //while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
    //        //{
    //        //    memStream.Write(buffer, 0, bytesRead);
    //        //}

    //        buffer = wc.DownloadData(imgPath);
    //        memStream.Write(buffer, 0, buffer.Length);
    //    }

    //    // 写入字符串的Key
    //    var stringKeyHeader = "\r\n--" + boundary +
    //                           "\r\nContent-Disposition: form-data; name=\"{0}\"" +
    //                           "\r\n\r\n{1}\r\n";

    //    foreach (byte[] formitembytes in from string key in stringDict.Keys
    //                                     select string.Format(stringKeyHeader, key, stringDict[key])
    //                                         into formitem
    //                                         select Encoding.UTF8.GetBytes(formitem))
    //    {
    //        memStream.Write(formitembytes, 0, formitembytes.Length);
    //    }

    //    // 写入最后的结束边界符
    //    memStream.Write(endBoundary, 0, endBoundary.Length);

    //    webRequest.ContentLength = memStream.Length;

    //    var requestStream = webRequest.GetRequestStream();

    //    memStream.Position = 0;
    //    var tempBuffer = new byte[memStream.Length];
    //    memStream.Read(tempBuffer, 0, tempBuffer.Length);
    //    memStream.Close();

    //    requestStream.Write(tempBuffer, 0, tempBuffer.Length);
    //    requestStream.Close();

    //    var httpWebResponse = (HttpWebResponse)webRequest.GetResponse();

    //    using (var httpStreamReader = new StreamReader(httpWebResponse.GetResponseStream(),
    //                                                    Encoding.GetEncoding("utf-8")))
    //    {
    //        responseContent = httpStreamReader.ReadToEnd();
    //    }

    //    fileStream.Close();
    //    httpWebResponse.Close();
    //    webRequest.Abort();

    //    entry = new LogEntry("发送图片结束-HttpPostData -- " + responseContent, 1);
    //    LogStub.Log(entry);

    //    return responseContent;
    //}

    public static bool HttpDownload(string url, string path)
    {
        string tempPath = System.IO.Path.GetDirectoryName(path) + @"\temp";
        System.IO.Directory.CreateDirectory(tempPath);  //创建临时文件目录
        string tempFile = tempPath + @"\" + System.IO.Path.GetFileName(path) + ".temp"; //临时文件
        if (System.IO.File.Exists(tempFile))
        {
            System.IO.File.Delete(tempFile);    //存在则删除
        }
        try
        {
            FileStream fs = new FileStream(tempFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            // 设置参数
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            //发送请求并获取相应回应数据
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            //直到request.GetResponse()程序才开始向目标网页发送Post请求
            Stream responseStream = response.GetResponseStream();
            //创建本地文件写入流
            //Stream stream = new FileStream(tempFile, FileMode.Create);
            byte[] bArr = new byte[1024];
            int size = responseStream.Read(bArr, 0, (int)bArr.Length);
            while (size > 0)
            {
                //stream.Write(bArr, 0, size);
                fs.Write(bArr, 0, size);
                size = responseStream.Read(bArr, 0, (int)bArr.Length);
            }
            //stream.Close();
            fs.Close();
            responseStream.Close();
            System.IO.File.Move(tempFile, path);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}

/// <summary>  
/// 有关HTTP请求的辅助类  
/// </summary>  
public class HttpWebResponseUtility
{
    private static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
    /// <summary>  
    /// 创建GET方式的HTTP请求  
    /// </summary>  
    /// <param name="url">请求的URL</param>  
    /// <param name="timeout">请求的超时时间</param>  
    /// <param name="userAgent">请求的客户端浏览器信息，可以为空</param>  
    /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>  
    /// <returns></returns>  
    public static string CreateGetHttpResponse(string url, string postDataStr, int? timeout, string userAgent, CookieCollection cookies, string header = null)
    {
        if (string.IsNullOrEmpty(url))
        {
            throw new ArgumentNullException("url");
        }
        HttpWebRequest request = WebRequest.Create(url + (postDataStr == "" ? "" : "?") + postDataStr) as HttpWebRequest;
        request.Method = "GET";
        request.UserAgent = DefaultUserAgent;
        if (!string.IsNullOrEmpty(userAgent))
        {
            request.UserAgent = userAgent;
        }
        if (timeout.HasValue)
        {
            request.Timeout = timeout.Value;
        }
        if (header != null)
        {
            request.Headers.Add("Authentication", header);
        }
        if (cookies != null)
        {
            request.CookieContainer = new CookieContainer();
            request.CookieContainer.Add(cookies);
        }
        HttpWebResponse response = request.GetResponse() as HttpWebResponse;
        string retString = string.Empty;
        using (Stream myResponseStream = response.GetResponseStream())
        {
            using (StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8))
            {
                retString = myStreamReader.ReadToEnd();
            }
        }
        return retString;
    }
    /// <summary>  
    /// 创建POST方式的HTTP请求  
    /// </summary>  
    /// <param name="url">请求的URL</param>  
    /// <param name="parameters">随同请求POST的参数名称及参数值字典</param>  
    /// <param name="timeout">请求的超时时间</param>  
    /// <param name="userAgent">请求的客户端浏览器信息，可以为空</param>  
    /// <param name="requestEncoding">发送HTTP请求时所用的编码</param>  
    /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>  
    /// <returns></returns>  
    public static string CreatePostHttpResponse(string url, IDictionary<string, string> parameters, int? timeout, string userAgent, Encoding requestEncoding, CookieCollection cookies, string header = null)
    {
        if (string.IsNullOrEmpty(url))
        {
            throw new ArgumentNullException("url");
        }
        if (requestEncoding == null)
        {
            throw new ArgumentNullException("requestEncoding");
        }
        HttpWebRequest request = null;
        //如果是发送HTTPS请求  
        if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            request = WebRequest.Create(url) as HttpWebRequest;
            request.ProtocolVersion = HttpVersion.Version10;
        }
        else
        {
            request = WebRequest.Create(url) as HttpWebRequest;
        }
        request.Method = "POST";
        request.ContentType = "application/x-www-form-urlencoded";
        if (!string.IsNullOrEmpty(userAgent))
        {
            request.UserAgent = userAgent;
        }
        else
        {
            request.UserAgent = DefaultUserAgent;
        }
        if (header != null)
        {
            request.Headers.Add("Authentication", header);
        }
        if (timeout.HasValue)
        {
            request.Timeout = timeout.Value;
        }
        if (cookies != null)
        {
            request.CookieContainer = new CookieContainer();
            request.CookieContainer.Add(cookies);
        }
        //如果需要POST数据  
        if (!(parameters == null || parameters.Count == 0))
        {
            StringBuilder buffer = new StringBuilder();
            int i = 0;
            foreach (string key in parameters.Keys)
            {
                if (i > 0)
                {
                    buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                }
                else
                {
                    buffer.AppendFormat("{0}={1}", key, parameters[key]);
                }
                i++;
            }
            byte[] data = requestEncoding.GetBytes(buffer.ToString());
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
        }

        HttpWebResponse response = request.GetResponse() as HttpWebResponse;
        string retString = string.Empty;
        using (Stream myResponseStream = response.GetResponseStream())
        {
            using (StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8")))
            {
                retString = myStreamReader.ReadToEnd();
            }
        }
        return retString;
    }

    public static string HttpPostJson(string url, IDictionary<string, string> parameters, Encoding requestEncoding, string fileNamePath, string saveName, string header)
    {
        if (string.IsNullOrEmpty(url))
        {
            throw new ArgumentNullException("url");
        }
        if (requestEncoding == null)
        {
            throw new ArgumentNullException("requestEncoding");
        }
        HttpWebRequest request = null;
        if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            request = WebRequest.Create(url) as HttpWebRequest;
            request.ProtocolVersion = HttpVersion.Version10;
        }
        else
        {
            request = WebRequest.Create(url) as HttpWebRequest;
        }
        request.Method = "POST";
        request.ContentType = "application/json"; // or whatever - application/json, etc, etc

        if (header != null)
        {
            request.Headers.Add("Authentication", header);
        }

        using (StreamWriter requestWriter = new StreamWriter(request.GetRequestStream()))
        {
            string valueString = File.ReadAllText(fileNamePath);
            requestWriter.Write(valueString);
        }
      


        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        using (StreamReader sr = new StreamReader(response.GetResponseStream()))
        {
            return sr.ReadToEnd();
        }
    }

    private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
    {
        return true; //总是接受  
    }

    //public static string UploadFile(string strRequestUri, IDictionary<string, string> parameters, Encoding requestEncoding, string m_fileNamePath, string header)
    //{
    //    #region MyRegion


    //    DateTime start = DateTime.Now;

    //    // 要上传的文件
    //    FileStream oFileStream = new FileStream(m_fileNamePath.ToString(), FileMode.Open, FileAccess.Read);
    //    BinaryReader oBinaryReader = new BinaryReader(oFileStream);

    //    // 根据uri创建HttpWebRequest对象
    //    HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(new Uri(strRequestUri));
    //    httpReq.Method = "POST";
    //    if (header != null)
    //    {
    //        httpReq.Headers.Add("Authentication", header);
    //    }
    //    if (!(parameters == null || parameters.Count == 0))
    //    {
    //        StringBuilder buffers = new StringBuilder();
    //        int i = 0;
    //        foreach (string key in parameters.Keys)
    //        {
    //            if (i > 0)
    //            {
    //                buffers.AppendFormat("&{0}={1}", key, parameters[key]);
    //            }
    //            else
    //            {
    //                buffers.AppendFormat("{0}={1}", key, parameters[key]);
    //            }
    //            i++;
    //        }
    //        byte[] data = requestEncoding.GetBytes(buffers.ToString());
    //        using (Stream stream = httpReq.GetRequestStream())
    //        {
    //            stream.Write(data, 0, data.Length);
    //        }
    //    }
    //    //对发送的数据不使用缓存
    //    httpReq.AllowWriteStreamBuffering = false;

    //    //设置获得响应的超时时间（半小时）
    //    httpReq.Timeout = 300000;
    //    //httpReq.Timeout = 5000;
    //    // httpReq.ReadWriteTimeout = 150000;
    //    httpReq.KeepAlive = true;
    //    httpReq.ProtocolVersion = HttpVersion.Version11;

    //    httpReq.ContentType = "application/pdf";
    //    long filelength = oFileStream.Length;
    //    httpReq.SendChunked = true;
    //    //在将 AllowWriteStreamBuffering 设置为 false 的情况下执行写操作时，必须将 ContentLength 设置为非负数，或者将 SendChunked 设置为 true。

    //    //每次上传4k
    //    int bufferLength = 4 * 1024;
    //    byte[] buffer = new byte[bufferLength];

    //    //已上传的字节数
    //    long offset = 0;

    //    //开始上传时间
    //    DateTime startTime = DateTime.Now;
    //    int size = oBinaryReader.Read(buffer, 0, bufferLength);
    //    Stream postStream = httpReq.GetRequestStream();

    //    while (size > 0)
    //    {
    //        postStream.Write(buffer, 0, size);
    //        offset += size;
    //        size = oBinaryReader.Read(buffer, 0, bufferLength);
    //        //Console.Write(".");
    //    }
    //    //Console.WriteLine(".");
    //    postStream.Flush();
    //    postStream.Close();

    //    //获取服务器端的响应
    //    WebResponse webRespon = httpReq.GetResponse();
    //    Stream s = webRespon.GetResponseStream();
    //    StreamReader sr = new StreamReader(s);
    //    DateTime end = DateTime.Now;
    //    TimeSpan ts = end - start;
    //    //读取服务器端返回的消息
    //    String sReturnString = sr.ReadLine();
    //    Console.WriteLine("retcode=" + sReturnString + " 花费时间=" + ts.TotalSeconds.ToString());
    //    s.Close();
    //    sr.Close();
    //    return sReturnString;
    //    #endregion
    //}


    //public static string Upload_Request(string address, IDictionary<string, string> parameters, Encoding requestEncoding, string fileNamePath, string saveName, string header)
    //{
    //    int returnValue = 0;

    //    // 要上传的文件 
    //    FileStream fs = new FileStream(fileNamePath, FileMode.Open, FileAccess.Read);
    //    BinaryReader r = new BinaryReader(fs);

    //    //时间戳 
    //    string strBoundary = "----------" + DateTime.Now.Ticks.ToString("x");
    //    byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + strBoundary + "\r\n");

    //    //请求头部信息 
    //    StringBuilder sb = new StringBuilder();
    //    sb.Append("--");
    //    sb.Append(boundaryBytes);
    //    sb.Append("\r\n");
    //    sb.Append("Content-Disposition: form-data; name=\"");
    //    sb.Append("file");
    //    sb.Append("\"; filename=\"");
    //    sb.Append(saveName);
    //    sb.Append("\"");
    //    sb.Append("\r\n");
    //    sb.Append("Content-Type: ");
    //    sb.Append("application/octet-stream");
    //    sb.Append("\r\n");
    //    sb.Append("\r\n");
    //    string strPostHeader = sb.ToString();
    //    byte[] postHeaderBytes = Encoding.UTF8.GetBytes(strPostHeader);

    //    // 根据uri创建HttpWebRequest对象 
    //    HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(new Uri(address));
    //    httpReq.Method = "POST";
    //    if (!(parameters == null || parameters.Count == 0))
    //    {
    //        StringBuilder buffers = new StringBuilder();
    //        int i = 0;
    //        foreach (string key in parameters.Keys)
    //        {
    //            if (i > 0)
    //            {
    //                buffers.AppendFormat("&{0}={1}", key, parameters[key]);
    //            }
    //            else
    //            {
    //                buffers.AppendFormat("{0}={1}", key, parameters[key]);
    //            }
    //            i++;
    //        }
    //    }
    //    if (header != null)
    //    {
    //        httpReq.Headers.Add("Authentication", header);
    //    }
    //    //对发送的数据不使用缓存 
    //    httpReq.AllowWriteStreamBuffering = false;

    //    //设置获得响应的超时时间（300秒） 
    //    httpReq.Timeout = 300000;
    //    httpReq.ContentType = "multipart/form-data; boundary=" + strBoundary;
    //    long length = fs.Length + postHeaderBytes.Length + boundaryBytes.Length;
    //    long fileLength = fs.Length;
    //    httpReq.ContentLength = length;
    //    try
    //    {
    //        //每次上传4k 
    //        int bufferLength = 4096;
    //        byte[] buffer = new byte[bufferLength];

    //        //已上传的字节数 
    //        long offset = 0;

    //        //开始上传时间 
    //        DateTime startTime = DateTime.Now;
    //        int size = r.Read(buffer, 0, bufferLength);
    //        Stream postStream = httpReq.GetRequestStream();

    //        //发送请求头部消息 
    //        postStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
    //        while (size > 0)
    //        {
    //            postStream.Write(buffer, 0, size);
    //            offset += size;
    //            TimeSpan span = DateTime.Now - startTime;
    //            double second = span.TotalSeconds;
    //            size = r.Read(buffer, 0, bufferLength);
    //        }

    //        //添加尾部的时间戳 
    //        postStream.Write(boundaryBytes, 0, boundaryBytes.Length);
    //        postStream.Close();
    //        Debug.Log(httpReq.Headers);
    //        //获取服务器端的响应 
    //        WebResponse webRespon = httpReq.GetResponse();
    //        Stream s = webRespon.GetResponseStream();
    //        StreamReader sr = new StreamReader(s);

    //        //读取服务器端返回的消息 
    //        String sReturnString = sr.ReadLine();
    //        s.Close();
    //        sr.Close();
    //        return sReturnString;
    //    }
    //    catch
    //    {
    //        returnValue = 0;
    //    }
    //    finally
    //    {
    //        fs.Close();
    //        r.Close();
    //    }
    //    return "";
    //}

    //public static string UpNew(string url, IDictionary<string, string> parameters, Encoding requestEncoding, string fileNamePath, string saveName, string header)
    //{
    //    FileStream fs = new FileStream(fileNamePath, FileMode.Open, FileAccess.Read);
    //    StreamReader sr = new StreamReader(fs);
    //    sr.BaseStream.Seek(0, SeekOrigin.Begin);
    //    string xmlContent = "";
    //    string str = sr.ReadLine();
    //    while (str != null)
    //    {
    //        //ssConsole.WriteLine(str);
    //        xmlContent = xmlContent += str;
    //        xmlContent += "\r\n ";
    //        str = sr.ReadLine();
    //    }

    //    sr.Close();
    //    fs.Close();

    //    string boundary = header; //Could be any string  
    //    string Enter = "\r\n";

    //    //part 1  
    //    string part1 = "--" + boundary + Enter
    //            + "Content-Type: application/octet-stream" + Enter
    //            + "Content-Disposition: form-data; filename=\"" + fileNamePath + "\"; name=\"file\"" + Enter + Enter;
    //    //part 2  
    //    string part2 = Enter
    //            + "--" + boundary + Enter
    //            + "Content-Type: text/plain" + Enter
    //            + "Content-Disposition: form-data; name=\"dataFormat\"" + Enter + Enter
    //            + "hk" + Enter
    //            + "--" + boundary + "--";

    //    string postDataStr = part1 + xmlContent + part2;
    //    //textBox1.Text = postDataStr;

    //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
    //    request.Method = "POST";
    //    request.ContentType = "multipart/form-data;boundary=" + boundary;
    //    if (header != null)
    //    {
    //        request.Headers.Add("Authentication", header);
    //    }
    //    //if (!(parameters == null || parameters.Count == 0))
    //    //{
    //    //    StringBuilder buffer = new StringBuilder();
    //    //    int i = 0;
    //    //    foreach (string key in parameters.Keys)
    //    //    {
    //    //        if (i > 0)
    //    //        {
    //    //            buffer.AppendFormat("&{0}={1}", key, parameters[key]);
    //    //        }
    //    //        else
    //    //        {
    //    //            buffer.AppendFormat("{0}={1}", key, parameters[key]);
    //    //        }
    //    //        i++;
    //    //    }
    //    //    byte[] data = requestEncoding.GetBytes(buffer.ToString());
    //    //    using (Stream stream = request.GetRequestStream())
    //    //    {
    //    //        stream.Write(data, 0, data.Length);
    //    //    }
    //    //}


    //    request.ContentLength = Encoding.UTF8.GetByteCount(postDataStr);

    //    Stream myRequestStream = request.GetRequestStream();
    //    StreamWriter myStreamWriter = new StreamWriter(myRequestStream, Encoding.GetEncoding("UTF-8"));

    //    myStreamWriter.Write(postDataStr);
    //    myStreamWriter.Close();


    //    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
    //    Stream myResponseStream = response.GetResponseStream();
    //    StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
    //    string retString = myStreamReader.ReadToEnd();
    //    //textBox2.Text = retString;

    //    myStreamReader.Close();
    //    myResponseStream.Close();
    //    return retString;
    //}

    public static string Upload_Request(string url, IDictionary<string, string> parameters, Encoding requestEncoding, string fileNamePath, string saveName, string header)
    {
        string filePath = fileNamePath;
        string fileName = saveName;
        string postURL = url;

        // 边界符  
        var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
        var beginBoundary = Encoding.ASCII.GetBytes("--" + boundary + "\r\n");
        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

        // 最后的结束符  
        var endBoundary = Encoding.ASCII.GetBytes("--" + boundary + "--\r\n");

        // 文件参数头  
        const string filePartHeader =
            /*"Authentication={0}\r\n" + */"Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" +
             "Content-Type: application/octet-stream\r\n\r\n";
        var fileHeader = string.Format(filePartHeader, "file", fileName);
        var fileHeaderBytes = Encoding.UTF8.GetBytes(fileHeader);

        // 开始拼数据  
        var memStream = new MemoryStream();
        memStream.Write(beginBoundary, 0, beginBoundary.Length);

        // 文件数据  
        memStream.Write(fileHeaderBytes, 0, fileHeaderBytes.Length);
        var buffer = new byte[1024];
        int bytesRead; // =0  
        while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
        {
            memStream.Write(buffer, 0, bytesRead);
        }
        fileStream.Close();

        // Key-Value数据  
        var stringKeyHeader = "\r\n--" + boundary +
                               "\r\nContent-Disposition: form-data; name=\"{0}\"" +
                               "\r\n\r\n{1}\r\n";

        Dictionary<string, string> stringDict = (Dictionary<string, string>)parameters;
        foreach (byte[] formitembytes in from string key in stringDict.Keys
                                         select string.Format(stringKeyHeader, key, stringDict[key])
                                             into formitem
                                         select Encoding.UTF8.GetBytes(formitem))
        {
            memStream.Write(formitembytes, 0, formitembytes.Length);
        }

        // 写入最后的结束边界符  
        memStream.Write(endBoundary, 0, endBoundary.Length);

        //倒腾到tempBuffer?  
        memStream.Position = 0;
        var tempBuffer = new byte[memStream.Length];
        memStream.Read(tempBuffer, 0, tempBuffer.Length);
        memStream.Close();

        // 创建webRequest并设置属性  
        var webRequest = (HttpWebRequest)WebRequest.Create(postURL);
        webRequest.Method = "POST";
        webRequest.Timeout = 100000;
        webRequest.ContentType = "multipart/form-data; boundary=" + boundary;
        webRequest.ContentLength = tempBuffer.Length;
        if (header != null)
        {
            webRequest.Headers.Add("Authentication", header);
        }

        var requestStream = webRequest.GetRequestStream();
        requestStream.Write(tempBuffer, 0, tempBuffer.Length);
        requestStream.Close();

        var httpWebResponse = (HttpWebResponse)webRequest.GetResponse();
        string responseContent;
        using (var httpStreamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.GetEncoding("utf-8")))
        {
            responseContent = httpStreamReader.ReadToEnd();
        }

        httpWebResponse.Close();
        webRequest.Abort();
        return responseContent;
        //context.Response.ContentType = "text/plain";
        //context.Response.Write(responseContent);
    }
}
