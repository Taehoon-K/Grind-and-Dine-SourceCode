using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BuildManager : MonoBehaviour
{
    [SerializeField]
    GameObject maincamera;
    Camera camera;

    private int index_;
    private int[] reqIndex;
    private ColliderInter colliderInter;
    private ColliderSauce colliderSauce;

    ThrowingMini throwmii;
    CameraRayMini cameraRayMini;

    private Vector3 pos;
    private RaycastHit hit;


    [SerializeField] private float distance;
    [SerializeField] private LayerMask layermask;
    [SerializeField] private LayerMask layermask2;
    [SerializeField] private LayerMask layermask3;


    void Start()
    {
        camera = maincamera.GetComponent<Camera>();
        throwmii = GetComponent<ThrowingMini>();
        cameraRayMini = GetComponent<CameraRayMini>();
    }
    private void Update()
    {
        Ray ray = camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));

        if (index_ < 18)
        {
            if (Physics.Raycast(ray, out hit, distance, layermask2)) //Æ¢±è±â
            {  
                if (colliderInter == null)
                {
                    colliderInter = hit.collider.gameObject.GetComponent<ColliderInter>();
                }
                else if (colliderInter.gameObject != hit.collider.gameObject)
                {
                    if (index_ >= 11)
                    {
                        colliderInter.previewItem[index_ - 11].SetActive(false);
                    }
                    colliderInter = hit.collider.gameObject.GetComponent<ColliderInter>();
                }
                reqIndex = colliderInter.requireId;
                bool found = false;
                for (int i = 0; i < reqIndex.Length; i++) //Æ¢±è±â°¡ Çã¿ëÇÏ´Â °ªÀÌ¶û µé°í ÀÖ´Â°Å¶û ºñ±³
                {
                    if (reqIndex[i] == index_)
                    {
                        found = true;
                        break;
                    }
                }

                if (throwmii.readyToThrow && found && colliderInter.canPlaced)//µé°íÀÖ´Â°Å¶û Çã¿ëÇÏ´Â°Å¶û °°À¸¸é
                {
                    colliderInter.previewItem[index_-11].SetActive(true);
                }

                if (cameraRayMini.interactionButton && found && throwmii.readyToThrow && colliderInter.canPlaced)
                {
                    cameraRayMini.interactionButton = false;
                    colliderInter.previewItem[index_ - 11].SetActive(false);
                    colliderInter.canPlaced = false;
                    throwmii.Ground();
                    cameraRayMini.DestroyPlate();
                    colliderInter.StartCo(index_-11); //ÀÌÁ¦ µÞÀÏÀº µý°÷ ³Ñ°Ü¼­ ÇÏ±â
                }
            }
            else
            {
                if (colliderInter != null)
                {
                    if (index_ >= 11)// && colliderInter.previewItem[index_ - 11].activeSelf)
                    colliderInter.previewItem[index_ - 11].SetActive(false);
                }
            }



            if (Physics.Raycast(ray, out hit, distance, layermask3))
            {  //¾ç³ä¿ë ±×¸©
                if (colliderSauce == null)
                {
                    colliderSauce = hit.collider.gameObject.GetComponent<ColliderSauce>();
                }
                else if (colliderSauce.gameObject != hit.collider.gameObject)
                {
                    colliderSauce = hit.collider.gameObject.GetComponent<ColliderSauce>();
                }
                reqIndex = colliderSauce.requireId;
                bool found = false;
                for (int i = 0; i < reqIndex.Length; i++) //¾ç³äÅë Çã¿ëÇÏ´Â °ªÀÌ¶û µé°í ÀÖ´Â°Å¶û ºñ±³
                {
                    if (reqIndex[i] == index_)
                    {
                        found = true;
                        break;
                    }
                }
                if (throwmii.readyToThrow && found && colliderSauce.canPlaced)//µé°íÀÖ´Â°Å¶û Çã¿ëÇÏ´Â°Å¶û °°À¸¸é
                {
                    colliderSauce.previewItem[SuaceTransform(index_)].SetActive(true);
                }
                else
                {
                    colliderSauce.previewItem[SuaceTransform(index_)].SetActive(false);
                }
                if (cameraRayMini.interactionButton && throwmii.readyToThrow && colliderSauce.canPlaced && found)
                {
                    cameraRayMini.interactionButton = false;
                    colliderSauce.canPlaced = false;
                    throwmii.Ground();
                    cameraRayMini.DestroyPlate();
                    colliderSauce.StartCo(SuaceTransform(index_)); //ÀÌÁ¦ µÞÀÏÀº µý°÷ ³Ñ°Ü¼­ ÇÏ±â
                }
            }
            else
            {
                if (colliderSauce != null)
                {
                    colliderSauce.previewItem[SuaceTransform(index_)].SetActive(false);
                }
            }
        }
    }

    private static int SuaceTransform(int n)
    {
        switch (n)
        {
            case 1:
                return 0;
            case 3:
                return 1;
            case 5:
                return 2;
            default:
                return 0;
        }
    }

    public void GetId(int index)
    {
        index_ = index;
    }

}
