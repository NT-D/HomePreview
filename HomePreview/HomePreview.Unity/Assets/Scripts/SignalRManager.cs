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
#if UNITY_UWP
    private HubConnection _connection;
    private SynchronizationContext _unityThreadContext;
    // Use this for initialization
    void Start()
    {
        // take SynchronizationContext in Unity's main thread
        _unityThreadContext = SynchronizationContext.Current;
        InitializeConnection();
    }

    private void InitializeConnection()
    {
        // the URL which SignalR is implemented (ex: WebApp on Azure)
        _connection = new HubConnection("http://homepreview.azurewebsites.net/");
        var createCubeProxy = _connection.CreateHubProxy("imageUrlHub");
        createCubeProxy.On<string>(eventName: "create", onData: Create);
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
#endif
}
