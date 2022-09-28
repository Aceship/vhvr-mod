using System;
using UnityEngine;
using ValheimVRMod.Utilities;

namespace ValheimVRMod.Scripts
{
    public class SpectatorCamera : MonoBehaviour
    {
        GameObject followerCameraObject;
        Camera followCam;
        Camera specSkyboxCamera;
        Transform vrCameraTransform;

        private Vector3 velocity = Vector3.zero;
        private Vector3 offset = new Vector3(0, 2, -2);
        private static float maxRange = 4;
        private float initialSkyFOV;

        private bool postSpecRun;


        private enum cameraFollowType
        {
            SuperHot,
            VanityFollow,
            StabilizedFPV
        }



        void Start()
        {
            InitalizeCamera();
            postSpecRun = VHVRConfig.UseSpectatorCamera();

        }




        void Update()
        {
            Camera specSkyboxCam = CameraUtils.getCamera(CameraUtils.SPECTATOR_SKYBOX_CAMERA);

            followCam.enabled = VHVRConfig.UseSpectatorCamera();

            if (specSkyboxCam != null)
            {
                specSkyboxCam.enabled = followCam.enabled;
            }
            if (postSpecRun != VHVRConfig.UseSpectatorCamera())
            {
                Debug.Log("Spectator Camera set to: " + VHVRConfig.UseSpectatorCamera());
                resetCamera();
            }

            postSpecRun = VHVRConfig.UseSpectatorCamera();
        }

        void LateUpdate()
        {

            if (followCam.enabled && Player.m_localPlayer != null)
            {


                Transform HeadtargetTransform = Player.m_localPlayer.m_animator.GetBoneTransform(HumanBodyBones.Head);
                Transform localPlayerTransform = Player.m_localPlayer.GetTransform();


                switch (VHVRConfig.GetSpectatorCameraType())
                {

                    case ("ActionCamera"):
                        float actionCamFOV = VHVRConfig.GetactionCamFOV();
                        float actionCamPosDamp = VHVRConfig.GetactionPositionDampening();
                        float actionCamRotDamp = VHVRConfig.GetactionRotationDampening();
                        Vector3 actionoffset = VHVRConfig.GetactionPositionOffset();



                        ActionCamera(localPlayerTransform, actionCamFOV, actionCamPosDamp, actionCamRotDamp, actionoffset);
                        break;



                    case ("DynamicThirdPerson"):

                        bool dynamicCamStopNear = VHVRConfig.GetdynamicCamStopNear();
                        float dynamicCamFOV = VHVRConfig.GetdynamicCamFOV();
                        float dynamicCamPosDamp = VHVRConfig.GetdynamicCamPosDamp();
                        float dynamicCamRotDamp = VHVRConfig.GetdynamicCamRotDamp();
                        Vector3 dynamicCamoffset = VHVRConfig.GetdynamicCamoffset();

                        float dynamicBoatCamFOV = VHVRConfig.GetdynamicBoatCamFOV();
                        float dynamicBoatCamPosDamp = VHVRConfig.GetdynamicBoatCamPosDamp();
                        float dynamicBoatCamotDamp = VHVRConfig.GetdynamicBoatCamRotDamp();
                        Vector3 dynamicBoatCamoffset = VHVRConfig.GetdynamicBoatCamoffset();


                        if (Player.m_localPlayer.IsAttachedToShip())
                        {
                            // Vector3 playerVelocity = Player.m_localPlayer.GetVelocity();
                            ActionCamera(localPlayerTransform, dynamicBoatCamFOV, dynamicBoatCamPosDamp, dynamicBoatCamotDamp, dynamicBoatCamoffset);              //100f, 3f, -3f
                        }


                        else
                        {
                            // Character.GetCharactersInRange(localPlayerTransform, )
                            CameraCloseFollow(localPlayerTransform, dynamicCamStopNear, dynamicCamFOV, dynamicCamPosDamp, dynamicCamRotDamp, dynamicCamoffset);    //70f  1.5f, -2f
                        }
                        break;



                    case ("StabilizedFPV"):
                        if (vrCameraTransform == null)
                        {
                            vrCameraTransform = CameraUtils.getCamera(CameraUtils.VR_CAMERA).transform;
                        }

                        float fpvCamFOV = VHVRConfig.GetfpvCamFOV();
                        float fpvNearClipPlane = VHVRConfig.GetfpvCamNearClipPlane();
                        float fpvCamPosDamp = VHVRConfig.GetfpvCamPositionDampening();
                        float fpvCamRotDamp = VHVRConfig.GetfpvCamRotationDampening();
                        float fpvCamZOffset = VHVRConfig.GetfpvCamZPositionOffset();

                        CameraStabilizedFPV(vrCameraTransform, fpvCamFOV, fpvNearClipPlane, false, fpvCamZOffset, fpvCamPosDamp, fpvCamRotDamp);

                        break;


                }

            }

        }

        private void CalculateAndSetFOV(float fieldOfView)
        {
            float oldfov = followCam.fieldOfView;
            followCam.fieldOfView = fieldOfView;
            specSkyboxCamera.fieldOfView += (fieldOfView - oldfov);
        }


        // The camera used in Super Hot's Spectator Camera. I reduced the FOV slightly, since it was absurdly high for Valheim. 
        // This one doesn't look good in bases, but it's great for sailing and action. 

        private void ActionCamera(Transform targetTransform, float fieldOfView, float positionSmoothing, float roationSmoothing, Vector3 offset)
        {
            Vector3 targetPosition;
            CalculateAndSetFOV(fieldOfView);
            targetPosition = targetTransform.position + (targetTransform.right * offset.x) + (targetTransform.up * offset.y) + (targetTransform.forward * offset.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, positionSmoothing * Time.unscaledDeltaTime);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetTransform.rotation, (roationSmoothing * 100) * Time.unscaledDeltaTime);

        }




        // This camera is a standard third person camera that follows the player. It'll stop it's position follow when you're within a certain range of the camera.
        // It seems to have issues with jittering in some situations with transform.lookat, such as a slow moving boat that's heaving a lot. 

        private void CameraCloseFollow(Transform targetTransform, bool closeFollowStopWhenClose, float fieldOfView, float positionSmoothing, float roationSmoothing, Vector3 offset)
        {

            CalculateAndSetFOV(fieldOfView);
            followCam.nearClipPlane = 0.01f;
            float distanceFromPlayer;
            Vector3 targetPosition;
            Transform headTransform = Player.m_localPlayer.m_animator.GetBoneTransform(HumanBodyBones.Head);

            distanceFromPlayer = (transform.position - targetTransform.position).sqrMagnitude;
            if (closeFollowStopWhenClose == false || distanceFromPlayer > maxRange * maxRange)
            {
                targetPosition = targetTransform.position + (targetTransform.forward * offset.z) + (targetTransform.up * offset.y);
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, positionSmoothing);
            }

            //transform.LookAt(headTransform);
            Vector3 directionHeadVector = headTransform.position - transform.position;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(directionHeadVector), (roationSmoothing * 100) * Time.deltaTime);
        }


        // A smoother FPV camera. VR footage is pretty terrible to look at unless it's stabilized. 
        // Taken from <https://github.com/Wyattari/VRSmoothCamUnity/blob/main/VRSmoothCamUnityProject/Assets/VRSmoothCam/Scripts/SmoothCamMethods.cs>. 
        // Dampening from 0 to 0.1f

        private void CameraStabilizedFPV(Transform targetTransform, float fieldOfView, float nearClipPlane, bool lockRotation, float zOffset, float positionDampening, float rotationDampening)
        {
            followCam.nearClipPlane = nearClipPlane;

            CalculateAndSetFOV(fieldOfView);

            var velocity = Vector3.zero;
            transform.position = Vector3.SmoothDamp(transform.position, targetTransform.position + targetTransform.forward * zOffset, ref velocity, positionDampening);

            float angularVelocity = 0f;
            float delta = Quaternion.Angle(transform.rotation, targetTransform.rotation);
            if (delta > 0f)
            {
                float t = Mathf.SmoothDampAngle(delta, 0.0f, ref angularVelocity, rotationDampening);
                t = 1.0f - (t / delta);



                if (lockRotation) // Broken, still working on it.
                {
                    /*
                    float rollCorrectionSpeed = 1f;
                   
                    float roll = Vector3.Dot(transform.right, Vector3.up);  
                    transform.Rotate(0, 0, -roll * rollCorrectionSpeed);
                    Vector3 directionVector = targetTransform.position - transform.position;
                    Quaternion lookRotation = Quaternion.LookRotation(directionVector);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetTransform.rotation, (rotationDampening * 1000) * Time.deltaTime);
                    */

                    transform.rotation = Quaternion.LookRotation(targetTransform.forward);
                    // broken when looking up and down
                    //float lockZ = 0f;
                    // transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, lockZ);


                }

                else
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetTransform.rotation, t);
                }
            }



        }

        private void InitalizeCamera()
        {
            followerCameraObject = this.gameObject;
            followCam = GetComponent<Camera>();
            specSkyboxCamera = CameraUtils.getCamera(CameraUtils.SPECTATOR_SKYBOX_CAMERA);
        }

        public void resetCamera()
        {
            Debug.Log("Camera " + VHVRConfig.GetSpectatorCameraType() + " is Enabled.");
            Debug.Log(followerCameraObject);

            if (Player.m_localPlayer != null)
            {
                Vector3 playerPosition = Player.m_localPlayer.transform.position;
                transform.position = new Vector3(playerPosition.x, playerPosition.y + offset.y, playerPosition.z + offset.z);

            }
        }
    }
}