using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        RenderSettings.skybox.mainTexture = (Texture)Resources.Load(path: "teidetour23_under_over_stereo_pair_4000");
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown("space"))
        {
            RenderSettings.skybox.mainTexture = (Texture)Resources.Load(path: "Dirrogate_Airport_Stereoscopic_360_VR");
        }

    }
}
