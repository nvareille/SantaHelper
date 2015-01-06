/*******************************************************************************

INTEL CORPORATION PROPRIETARY INFORMATION
This software is supplied under the terms of a license agreement or nondisclosure
agreement with Intel Corporation and may not be copied or disclosed except in
accordance with the terms of that agreement
Copyright(c) 2014 Intel Corporation. All Rights Reserved.

*******************************************************************************/
using UnityEngine;
using System.Collections;

public class SenseInput : MonoBehaviour {
	public delegate void OnDepthImageDelegate(PXCMImage data);
	public delegate void OnColorImageDelegate(PXCMImage data);
	public delegate void OnHandDataDelegate(PXCMHandData.IHand data);
	public delegate void OnFaceDataDelegate(PXCMFaceData.LandmarksData data);
    public delegate void OnCenterHandDelegate(PXCMHandData.IHand data);

	public  event OnDepthImageDelegate OnDepthImage;
	public  event OnColorImageDelegate OnColorImage;
	public  event OnHandDataDelegate   OnHandData;
	public  event OnFaceDataDelegate   OnFaceData;
    public  event OnCenterHandDelegate OnCenterData;
	
	private PXCMSenseManager sm=null;
	
	// Use this for initialization
	void Start () {
		/* Initialize a PXCMSenseManager instance */
		sm = PXCMSenseManager.CreateInstance();
		if (sm==null) return;
		
		/* Enable hand tracking and configure the hand module */
		pxcmStatus sts=sm.EnableHand();
		      		
		/* Enable face detection and configure the face module */
		sts=sm.EnableFace();
        /*PXCMFaceModule face = sm.QueryFace();
        if (face != null)
        {
            textWindow.WriteLn("Configure Face");
            PXCMFaceConfiguration face_cfg = face.CreateActiveConfiguration();
            face_cfg.landmarks.isEnabled=true; 
            face_cfg.ApplyChanges();
            face_cfg.Dispose();
        }*/

		/* Initialize the execution pipeline */ 
		sts=sm.Init(); 
		if (sts<pxcmStatus.PXCM_STATUS_NO_ERROR) {
			OnDisable();
			return;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (sm==null) return;
		
		/* Pause hand or face if they are not in action */
		sm.PauseFace(!CubeMagic.visualizeFace);
		sm.PauseHand(!CubeMagic.visualizeHand);
		
		/* Wait until any frame data is available */
		if (sm.AcquireFrame(false,0)<pxcmStatus.PXCM_STATUS_NO_ERROR) return;
		
		/* Retrieve hand tracking data if ready */
	    PXCMHandModule hand=sm.QueryHand();
		if (hand!=null) {
            PXCMHandData hand_data = hand.CreateOutput();
            hand_data.Update();

            PXCMHandData.IHand ihand;
            pxcmStatus sts=hand_data.QueryHandData(PXCMHandData.AccessOrderType.ACCESS_ORDER_BY_TIME,0,out ihand);
            if (sts >= pxcmStatus.PXCM_STATUS_NO_ERROR)
            {
                OnHandData(ihand);
                OnCenterData(ihand);
            }
            hand_data.Dispose();
		}
		
		/* Retrieve face data if ready */
		PXCMFaceModule face = sm.QueryFace();
        if (face != null)
        {
            PXCMFaceData face_data = face.CreateOutput();
            face_data.Update();

            PXCMFaceData.Face face0 = face_data.QueryFaceByIndex(0);
            if (face0 != null)
            {
                PXCMFaceData.LandmarksData data = face0.QueryLandmarks();
                if (data != null)
                    OnFaceData(data);
            }
            face_data.Dispose();
        }
		
		/* Retrieve the color and depth images if ready */
		PXCMCapture.Sample sample = sm.QueryFaceSample();
        if (sample == null) sample = sm.QueryHandSample();

        if (sample!=null  && sample.color != null)
            OnColorImage(sample.color);
        if (sample!=null && sample.depth != null)
			OnDepthImage(sample.depth);
		
		/* Now, process the next frame */
		sm.ReleaseFrame();
	}
	
	void OnDisable() {
        if (sm == null) return;
        sm.Dispose();
		sm=null;
	}
}
