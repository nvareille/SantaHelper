/*******************************************************************************

INTEL CORPORATION PROPRIETARY INFORMATION
This software is supplied under the terms of a license agreement or nondisclosure
agreement with Intel Corporation and may not be copied or disclosed except in
accordance with the terms of that agreement
Copyright(c) 2014 Intel Corporation. All Rights Reserved.

*******************************************************************************/
using UnityEngine;
using System.Collections;

public class CubeMagic : MonoBehaviour {
	public SenseInput input;
	public float hmin, hmax;
	public float vmin, vmax;

    public int dir;
	
    public GameObject[] obj;

	private Texture2D depthImage=null;
	private static bool visualizeImage=false;
	public  static bool visualizeHand=true;
	public  static bool visualizeFace=false;
	private PXCMSizeI32 depthSize;
	private PXCMSizeI32 colorSize;
	private bool rotate=false;
	
	// Use this for initialization
	void Start () {
		/* Set event handlers */
		input.OnHandData+=OnHandData;
		input.OnDepthImage+=OnDepthImage;
		input.OnFaceData+=OnFaceData;
		input.OnColorImage+=OnColorImage;
        input.OnCenterData += OnCenter;
		
		depthSize.width=depthSize.height=0;
		colorSize.width=colorSize.height=0;
		rotate=false;
	}
	
	// Update is called once per frame
	void Update () {
		if (!rotate) return;
		transform.Rotate(new Vector3(0,-3,0));
	}

	void SetRotateMyCube(string sender, PXCMPointF32 xy) {
		/* If this is my cube */
		rotate=(xy.x>=hmin && xy.x<=hmax && xy.y>=vmin && xy.y<=vmax);
		if (!rotate) return;
    }
	
    // xy: image center
	void OnHandData(PXCMHandData.IHand ihand)
    {  
		if (!visualizeHand) return;
		if (depthSize.width==0 || depthSize.height==0) return;

        PXCMPointF32 xy = ihand.QueryMassCenterImage();
		xy.x=1-xy.x/depthSize.width;
		xy.y=xy.y/depthSize.height;
		SetRotateMyCube ("Hand", xy);
	}
	
	void OnFaceData(PXCMFaceData.LandmarksData data) {
		if (!visualizeFace) return;
        if (colorSize.width == 0 || colorSize.height == 0) return;

        PXCMFaceData.LandmarkPoint[] points;
        if (!data.QueryPoints(out points)) return;

        /* Use nose tip as the control point */
        PXCMPointF32 xy=points[data.QueryPointIndex(PXCMFaceData.LandmarkType.LANDMARK_NOSE_TIP)].image;
		/* Mirror, normalize */
		xy.x=(1-(xy.x/colorSize.width));
		xy.y=(xy.y/colorSize.height);
		
		/* scale it so we don't have to make huge moves */
		xy.x=(xy.x-0.5f)*2+0.5f;
		xy.y=(xy.y-0.5f)*2+0.5f;
		SetRotateMyCube("Face", xy);
	}
	
	void OnDepthImage(PXCMImage image) {
		/* Save depth size for later use */
		depthSize.width=image.info.width;
		depthSize.height=image.info.height;
		if (!visualizeImage) return;
		if (!rotate) return;

		if (depthImage==null) {
			/* If not allocated, allocate Texture2D */
			depthSize.width=image.info.width;
			depthSize.height=image.info.height;
			depthImage=new Texture2D((int)depthSize.width, (int)depthSize.height, TextureFormat.ARGB32, false);
			
			/* Associate the Texture2D with the cube */
			renderer.material.mainTexture=depthImage;
			renderer.material.mainTextureScale=new Vector2(-1,-1);
		}
		
		/* Retrieve the image data in Texture2D */
		PXCMImage.ImageData data;
		image.AcquireAccess(PXCMImage.Access.ACCESS_READ,PXCMImage.PixelFormat.PIXEL_FORMAT_RGB32,out data);
		data.ToTexture2D(0, depthImage);
		image.ReleaseAccess(data);
		
		/* Display on the Cube */
		depthImage.Apply();
	}

    PXCMHandData.JointData[] getJoints(PXCMHandData.IHand hand)
    {
        int count = 0;
        PXCMHandData.JointData[] j = new PXCMHandData.JointData[PXCMHandData.NUMBER_OF_JOINTS];
        //PXCMDataSmoothing.Smoother3D[] i = new PXCMDataSmoothing.Smoother3D[PXCMHandData.NUMBER_OF_JOINTS];

        while (count < PXCMHandData.NUMBER_OF_JOINTS)
        {
            hand.QueryTrackedJoint((PXCMHandData.JointType)count, out j[count]);
            ++count;
        }
        return (j);
    }

    void OnCenter(PXCMHandData.IHand ihand)
    {
        PXCMHandData.JointData[] joints = getJoints(ihand);
        Vector2[] points = new Vector2[3];
        float or;

        points[0] = new Vector2(joints[0].positionWorld.x, joints[0].positionWorld.y);
        points[1] = new Vector2(joints[10].positionWorld.x, joints[10].positionWorld.y);
        points[2] = points[0] - points[1];

        or = -Vector2.Angle(Vector2.up, points[2]);

        Debug.Log(points[2].x + " " + points[2].y);

        obj[0].transform.position = new Vector3(-points[0].x, points[0].y) * 20;
        obj[1].transform.position = new Vector3(-points[1].x, points[1].y) * 20;
        obj[2].transform.position = (obj[0].transform.position + obj[1].transform.position) / 2;
        obj[2].transform.eulerAngles = new Vector3(0, 0, points[0].x < points[1].x ? or : -or);

        if (points[0].x < points[1].x)
            dir = 1;
        else
            dir = -1;
        if (points[0].y > points[1].y)
            dir = -dir;
    }

	void OnColorImage(PXCMImage image) {
		colorSize.width=image.info.width;
		colorSize.height=image.info.height;
	}

    void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.name == "Candy-Cane" || c.gameObject.name == "Present" || c.gameObject.name == "Star" || c.gameObject.name == "Teddybear")
        {
            GameObject g = c.gameObject;

            g.rigidbody.AddForce(new Vector3(300f * dir, 0f, 0f));
            
        }
    }
	
	void OnGUI() {
		/* Only the first cube needs the show menu. It's shared among all cubes */
	}
}
