using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
#if UNITY_UWP
using Microsoft.AspNet.SignalR.Client;
#endif
public class SignalRManager : MonoBehaviour
{
    public Material _defaultSkybox;
#if UNITY_UWP
    private HubConnection _connection;
    private SynchronizationContext _unityThreadContext;
    // Use this for initialization
    void Start()
    {
        // take SynchronizationContext in Unity's main thread
        _unityThreadContext = SynchronizationContext.Current;
        InitializeConnection();
        RenderSettings.skybox = _defaultSkybox;
    }

    private void InitializeConnection()
    {
        // the URL which SignalR is implemented (ex: WebApp on Azure)
        _connection = new HubConnection("http://homepreview.azurewebsites.net/");
        var hubProxy = _connection.CreateHubProxy("imageUrlHub");
        hubProxy.On<string>(eventName: "create", onData: Create);
        hubProxy.On<string>(eventName: "changeSkyboxImage", onData: ChangeSkyboxImage);
        _connection.Start().ContinueWith(x =>
            {
                UnityEngine.Debug.Log(x.Exception?.Message ?? "Connected");
            }
        );
    }

    private void Create(string name)
    {
        // On Unity's mainthread
        _unityThreadContext.Post(d: _ =>
        {
            PrimitiveType type;
            if (!Enum.TryParse<PrimitiveType>(value: name, result: out type))
            {
                UnityEngine.Debug.LogWarning($"{name} is not defined");
                return;
            }
            var obj = GameObject.CreatePrimitive(type);
            obj.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 3;
            UnityEngine.Debug.Log($"{name} created at {obj.transform.position}");
        }, state: null);
    }

    private void ChangeSkyboxImage(string url)
    {
        Debug.Log(url);
        // On Unity's mainthread
        _unityThreadContext.Post(d: _ =>
        {
            //RenderSettings.skybox.mainTexture = (Texture)Resources.Load(path: "skysample");
            _defaultSkybox.mainTexture = (Texture)Resources.Load(path: "skysample");
            //RenderSettings.skybox = null;
        }, state: null);
    }
#endif
}
