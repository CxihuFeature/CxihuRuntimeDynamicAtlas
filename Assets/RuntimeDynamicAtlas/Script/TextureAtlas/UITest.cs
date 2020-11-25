using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RuntimeTextureAtlas;
using System.IO;
using System;
using UnityEngine.Networking;

public class UITest : MonoBehaviour
{
    Button button1;
    Button button2;
    Button button3Reset;
    Text textLog;
    Transform RawImagPanel;
    public RawImage[] rawImageArry;

    public UnityEngine.Object[] Resoucestextures;
    public string[] ResoucesfilesNameArry;
    public string[] filesArry;

    // Start is called before the first frame update
    void Start()
    {
        button1 = transform.Find("Button1").GetComponent<Button>();
        button1.onClick.AddListener(button1OnClick);
        button2 = transform.Find("Button2").GetComponent<Button>();
        button2.onClick.AddListener(button2OnClick);
        button3Reset = transform.Find("Button3").GetComponent<Button>();
        button3Reset.onClick.AddListener(button3ResetOnClick);
        textLog = transform.Find("TextLog").GetComponent<Text>();
        RawImagPanel = transform.Find("RawImagPanel");
        rawImageArry = RawImagPanel.GetComponentsInChildren<RawImage>();
        //Resources
        Resoucestextures = Resources.LoadAll("major", typeof(Texture));
        ResoucesfilesNameArry = new string[Resoucestextures.Length];
        for (int i = 0; i < Resoucestextures.Length; i++)
        {
            // filesNameArry[i] = "major/" + Path.GetFileNameWithoutExtension(filesArry[i]);
            ResoucesfilesNameArry[i] = "major/" + Resoucestextures[i].name;
        }

        //streamingAssetsPath
        filesArry = Directory.GetFiles(Application.streamingAssetsPath + "/shiwu", "*.png");

        StartSet();
        // StartCoroutine(StartSet2());
    }

    void StartSet()
    {
        for (int i = 0; i < rawImageArry.Length; i++)
        {
            //rawImageArry[i].texture = Resources.Load<Texture>(ResoucesfilesNameArry[i]);
            rawImageArry[i].texture = (Texture)Resoucestextures[i];
            rawImageArry[i].uvRect = new Rect(0, 0, 1, 1);
        }
    }
    IEnumerator StartSet2()
    {
        for (int i = 0; i < rawImageArry.Length; i++)
        {
            using (UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(filesArry[i]))
            {
                yield return unityWebRequest.SendWebRequest();
                if (!unityWebRequest.isNetworkError && !unityWebRequest.isHttpError)
                {
                    var texture = DownloadHandlerTexture.GetContent(unityWebRequest);
                    rawImageArry[i].texture = texture;
                }
            }
        }
    }

    DateTime startTime;
    DateTime endTime;
    void button1OnClick()
    {
        startTime = DateTime.Now;
        //RawImagesPackTexture(rawImageArry);
        LoadRawImagesResources(rawImageArry, ResoucesfilesNameArry);
        // StartCoroutine(LoadRawImagesFromFile(rawImageArry, filesArry));
        endTime = DateTime.Now;
        textLog.text = "DateTime:" + (endTime - startTime).Milliseconds + "ms";
    }

    void button2OnClick()
    {
        StartCoroutine(LoadRawImagesFromFile(rawImageArry, filesArry));
    }

    void button3ResetOnClick()
    {
        StartSet();
    }
    // Update is called once per frame
    void Update()
    {

    }


    public void RawImagesPackTexture(RawImage[] rawImagesArry, Texture[] textures = null)
    {
        for (int i = 0; i < rawImagesArry.Length; i++)
        {
            if (textures != null)
            {
                // 将贴图打包到图集，并应用到RawImage控件
                rawImagesArry[i].PackTexture(textures[i]);
            }
            else
            {
                rawImagesArry[i].PackTexture(rawImagesArry[i].texture);
            }
        }
    }
    public void LoadRawImageResources(RawImage rawImage, string file)
    {
        // 同步加载贴图
        var texture = Resources.Load<Texture>(file);
        // 将贴图打包到图集，并应用到RawImage控件
        rawImage.PackTexture(texture);
    }
    public void LoadRawImagesResources(RawImage[] rawImagesArry, string[] filesArry)
    {
        for (int i = 0; i < rawImagesArry.Length; i++)
        {
            // 同步加载贴图
            var texture = Resources.Load<Texture>(filesArry[i]);
            // 将贴图打包到图集，并应用到RawImage控件
            rawImagesArry[i].PackTexture(texture);
        }
    }


    public IEnumerator LoadRawImageFromFile(RawImage rawImage, string file)
    {
        // 异步加载贴图
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(file))
        {
            yield return uwr.SendWebRequest();

            if (!uwr.isNetworkError && !uwr.isHttpError)
            {
                // 贴图加载完毕
                var texture = DownloadHandlerTexture.GetContent(uwr);

                // 将贴图打包到图集，并应用到RawImage控件
                rawImage.PackTexture(texture,
                                  () => Destroy(texture)); // 打包成功后卸载原图
            }
        }
    }

    public IEnumerator LoadRawImagesFromFile(RawImage[] rawImagesArry, string[] filesArry)
    {
        for (int i = 0; i < rawImagesArry.Length; i++)
        {
            // Debug.Log("LoadRawImagesFromFile:"+rawImagesArry[i].name);
            // 异步加载贴图
            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(filesArry[i]))
            {
                yield return uwr.SendWebRequest();

                if (!uwr.isNetworkError && !uwr.isHttpError)
                {
                    // 贴图加载完毕
                    var texture = DownloadHandlerTexture.GetContent(uwr);

                    // 将贴图打包到图集，并应用到RawImage控件
                    rawImagesArry[i].PackTexture(texture,
                                      () => Destroy(texture)); // 打包成功后卸载原图
                }
            }
        }
    }
}
