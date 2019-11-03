﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class CapturePhoto : MonoBehaviour
{
    //Accessing camera utility
    private bool camExists;
    private WebCamTexture rearView;
    private Texture defaultBackground;
    public RawImage background;
    public AspectRatioFitter ratioFitter;

    //screenshot utilities
    Texture2D screenshot;
    bool captured = false;
    public int FileCounter = 0;

    string fileName;
    public bool onLoaded = false;
    Texture2D loadedImageTexture;
    public Image img;
    
     private void Start()
    {
        defaultBackground = background.texture;
        WebCamDevice[] cameras = WebCamTexture.devices;

        if (cameras.Length == 0)
        {
            camExists = false;
            return;
        }

        for (int i = 0; i < cameras.Length; i++)
        {
            if (!cameras[i].isFrontFacing)
            {
                rearView = new WebCamTexture(cameras[i].name, Screen.width, Screen.height);
            }
        }
        //debugging only--
        //rearView = new WebCamTexture(cameras[0].name, Screen.width, Screen.height);
     
        if (rearView == null)
        {
            return;
        }
        rearView.Play();
        background.texture = rearView;

        camExists = true;
        screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);   //Screenshot whole screen
    }
    private void Update()
    {
        if (!camExists)
            return;
        Debug.Log("before coroutine");
        //Reference: https://www.youtube.com/watch?v=c6NXkZWXHnc
        float ratio = (float)rearView.width / (float)rearView.height;
        ratioFitter.aspectRatio = ratio;

        float scaleY = rearView.videoVerticallyMirrored ? -1f : 1f;
        background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

        int orient = -rearView.videoRotationAngle;
        background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);

        //Screenshot Code
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            StartCoroutine("Capture");
        }

       // if (Input.GetKeyDown(KeyCode.Mouse1))
         // {
           //    StartCoroutine("LoadCapture");
          //}
       //debugging only--
       // StartCoroutine("Capture");
    }
    //screenshot
    //reference for ONGUI and Capture :https://www.youtube.com/watch?v=bQayHTts7HI
    void OnGUI()
    {

        if (captured)
        {
            GUI.DrawTexture(new Rect(40, 40, 80, 80), screenshot, ScaleMode.StretchToFill);
        }

       // if (onLoaded)
         // {
           //    GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), loadedImageTexture, ScaleMode.ScaleToFit);
          //}

        
    }

     IEnumerator Capture()
     {
          Debug.Log("inside coroutine");
          yield return new WaitForEndOfFrame();
          screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
          screenshot.Apply();
          captured = true;

          //var byteArray = screenshot.EncodeToPNG();
          byte[] byteArray = screenshot.EncodeToPNG();  //mine
          fileName = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
          File.WriteAllBytes(Application.persistentDataPath + "/" + fileName + ".png", byteArray);

          //Load Image Code  
          //background.gameObject.SetActive(false); //mine
          byte[] readImage = null;
          readImage = File.ReadAllBytes(Application.persistentDataPath + "/" + fileName + ".png");

          loadedImageTexture = new Texture2D(300, 200, TextureFormat.RGB24, false);
          loadedImageTexture.LoadImage(readImage);
          GameObject newImage = GameObject.Find("Background");     //newImage is the backGround from webcam now
          newImage.GetComponent<RawImage>().texture = loadedImageTexture;    //Sets newImage as loadedImage

}
     /*IEnumerator LoadCapture()
     {
          yield return new WaitForEndOfFrame();
          
     
          //img.sprite = s;
          //loadedImageTexture.Apply();
          //onLoaded = true;
     }*/

}

