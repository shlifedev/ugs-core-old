﻿using System;
using System.IO;
using System.Net;
using System.Text;
using Hamster.ZG;
using Hamster.ZG.IO.FileReader;
using Hamster.ZG.IO.FileWriter;
using Newtonsoft.Json;

namespace UGS_Console
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            UnityGoogleSheet.Initalize("https://script.google.com/macros/s/AKfycbyOBVdYiUz6W1WJCHhV5SS4r0Bq3NIyCKW8ugVunsBD-4Bbn30U/exec", "dpqlcb123");
            UnityGoogleSheet.Generate("1usG2ox2jkDi3OtbCMNTIVCKsFTyZr5M0dKxW9tO2tWo", true, true);

            UnityGoogleSheet.Load<Example1.Localization.Item.Name>();
            foreach(var value in Example1.Localization.Item.Name.NameList)
            {
                Console.WriteLine(value.KR);
            }
        }
    }
     
   
}

public class GoogleDriveWebRequester : IZGRequester
{
    public static GoogleDriveWebRequester Instance
    {
        get
        {
            if (instance == null)
                instance = new GoogleDriveWebRequester();

            return instance;
        }
    }
    static GoogleDriveWebRequester instance;
    public string baseURL = "";
    public string password = "";
    public void SearchGoogleDriveDirectory(string folderID, Action<GetFolderInfo> callback)
    {

    }

    public void ReadGoogleSpreadSheet(string sheetID, Action<GetTableResult, string> callback)
    {
        Instance.Get($"{baseURL}?password={password}&instruction=getTable&sheetID={sheetID}", (x) =>
        {
            if (x == null)
            {
                Console.WriteLine($"Cannot Read Google Sheet!");
            }
            else
            {
                try
                {
                    var value = JsonConvert.DeserializeObject<GetTableResult>(x);
                    callback?.Invoke(value, x);
                }
                catch
                {
                    callback?.Invoke(null, x);
                }
            }
        });
    }

    public void WriteObject(string spreadSheetID, string sheetID, string key, string[] value, Action onWrited = null)
    {

    }

    public void CreateDefaultTable(string folderID, string fileName, Action<string> callback)
    {

    }

    public void CopyExamples(string folderID, Action<string> callback)
    {

    }


    private void Get(string url, Action<string> callback)
    {
        try
        {
            WebRequest request = WebRequest.Create(url);
            request.Timeout = 7500;
            request.Credentials = CredentialCache.DefaultCredentials;
            WebResponse response = request.GetResponse();
            var statusCode = ((HttpWebResponse) response).StatusCode;
            string responseFromServer = "";

            if (statusCode == HttpStatusCode.RequestTimeout)
            {
                callback?.Invoke(null);
            }

            if (statusCode == HttpStatusCode.OK)
            {
                using (Stream dataStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(dataStream);
                    responseFromServer = reader.ReadToEnd();
                    callback?.Invoke(responseFromServer);
                }
            }
            else
            {
                callback?.Invoke(null);
            }

            response.Close();
        }
        catch (System.Exception e)
        {
            Console.WriteLine(e.Message + "\n" + e.StackTrace);
        }
    }

    private void Post(string json, Action<string> callback)
    {
        try
        {
            WebRequest request = WebRequest.Create(baseURL);
            request.Method = "POST";
            request.Timeout = 7500;
            byte[] data = Encoding.UTF8.GetBytes(json);
            request.ContentType = "application/json";
            request.ContentLength = data.Length;

            Stream ds = request.GetRequestStream();
            ds.Write(data, 0, data.Length);
            ds.Close();


            request.Credentials = CredentialCache.DefaultCredentials;
            WebResponse response = request.GetResponse();
            var statusCode = ((HttpWebResponse) response).StatusCode;
            string responseFromServer = "";

            if (statusCode == HttpStatusCode.RequestTimeout)
            {
                Console.WriteLine("Timeout - ZegoGoogleSheet Initialize Failed! Try Check Setting Window.");
                callback?.Invoke(null);
            }

            if (statusCode == HttpStatusCode.OK)
            {
                using (Stream dataStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(dataStream);
                    responseFromServer = reader.ReadToEnd();
                    callback?.Invoke(responseFromServer);
                }
            }
            else
            {
                Console.WriteLine(statusCode);
                callback?.Invoke(null);
            }

            response.Close();
        }
        catch (System.Exception e)
        {
            Console.WriteLine(e.Message + "\n" + e.StackTrace);
        }
    }
}