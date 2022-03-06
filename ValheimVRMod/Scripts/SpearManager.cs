﻿using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using ValheimVRMod.VRCore;
using UnityEngine.Rendering;
using ValheimVRMod.Scripts.Block;
using ValheimVRMod.Utilities;

namespace ValheimVRMod.Scripts {
    public class SpearManager : MonoBehaviour {
        private const int MAX_SNAPSHOTS = 7;
        private const int MIN_SNAPSHOTSCHECK = 3;
        private int tickCounter;
        private List<Vector3> snapshots = new List<Vector3>();
        private GameObject fixedSpear;

        private const float minDist = 0.16f;
        private const float slowThrowModifier = 1.5f;
        private const float mediumThrowModifier = 2f;
        private const float fastThrowModifier = 2.5f;
        private const float mediumThrowMinDist = 0.65f;
        private const float fastThrowMinDist = 0.9f;

        public static Vector3 spawnPoint;
        public static Vector3 aimDir;
        public static Vector3 startAim; 
        public static bool isThrowing;
        private GameObject rotSave;
        private static bool isThrowingStance;
        private LineRenderer directionLine;
        private float directionCooldown;
        private float totalCooldown = 2;
        private readonly Vector3 handAimOffset = new Vector3(0, -0.45f, -0.55f);

        private Transform mainHandTransform;
        private Transform offHandTransform;
        private SteamVR_Input_Sources mainHandInputSource;
        private SteamVR_Action_Boolean useAction;

        private void Awake() {
            
            if (VHVRConfig.LeftHanded()) {
                mainHandInputSource = SteamVR_Input_Sources.LeftHand;
                mainHandTransform = VRPlayer.leftHand.transform;
                offHandTransform = VRPlayer.rightHand.transform;
                useAction = SteamVR_Actions.valheim_UseLeft;
            }
            else {
                mainHandInputSource = SteamVR_Input_Sources.RightHand;
                mainHandTransform = VRPlayer.rightHand.transform;
                offHandTransform = VRPlayer.leftHand.transform;
                useAction = SteamVR_Actions.valheim_Use;
            }
            
            fixedSpear = new GameObject();
            rotSave = new GameObject();
            rotSave.transform.SetParent(transform.parent);
            rotSave.transform.position = transform.position;
            rotSave.transform.localRotation = transform.localRotation;

            directionLine = new GameObject().AddComponent<LineRenderer>();
            directionLine.widthMultiplier = 0.03f;
            directionLine.positionCount = 2;
            directionLine.material.color = Color.white;
            directionLine.enabled = false;
            directionLine.receiveShadows = false;
            directionLine.shadowCastingMode = ShadowCastingMode.Off;
            directionLine.lightProbeUsage = LightProbeUsage.Off;
            directionLine.reflectionProbeUsage = ReflectionProbeUsage.Off;
        }

        private void OnDestroy() {
            Destroy(fixedSpear);
            Destroy(rotSave);
            Destroy(directionLine,directionCooldown);
        }

        private void OnRenderObject() {
            fixedSpear.transform.position = transform.position;
            switch (VHVRConfig.SpearThrowType()) {
                case "DartType":
                    UpdateDartSpearThrowCalculation();
                    return;
                case "TwoStagedThrowing":
                    UpdateTwoStagedThrowCalculation();
                    return;
                case "SecondHandAiming":
                    UpdateSecondHandAimCalculation();
                    return;
                default:
                    UpdateDartSpearThrowCalculation();
                    return;
            }
        }

        private void FixedUpdate() {
            tickCounter++;
            if (tickCounter < 5) {
                return;
            }

            snapshots.Add(Player.m_localPlayer.transform.InverseTransformPoint(mainHandTransform.position));

            if (snapshots.Count > MAX_SNAPSHOTS) {
                snapshots.RemoveAt(0);
            }
            
            tickCounter = 0;
            if (!VHVRConfig.UseSpearDirectionGraphic()) {
                return;
            }

            if (directionCooldown <= 0) {
                directionCooldown = 0;
                directionLine.enabled = false;
            }
            else if (!isThrowingStance) {
                directionCooldown -= Time.unscaledDeltaTime*5;
            }
        }
        private void UpdateSecondHandAimCalculation()
        {
            Transform cameraHead = CameraUtils.getCamera(CameraUtils.VR_CAMERA).transform;
            var direction = offHandTransform.position - cameraHead.transform.position;
            if (!SteamVR_Actions.valheim_Grab.GetState(mainHandInputSource)) {
                startAim = Vector3.zero;
                ResetSpearOffset();
                return;
            }
            if (!isThrowingStance && !isThrowing) {
                UpdateDirectionLine(
                    mainHandTransform.position,
                    mainHandTransform.position + (direction).normalized * 50);
            }
            if (useAction.GetStateDown(mainHandInputSource)) {
                if (startAim == Vector3.zero) {
                    startAim = direction.normalized;
                }
                isThrowingStance = true;
            }
            ShieldBlock.instance?.ScaleShieldSize(0.4f);
            if (isThrowingStance) {
                UpdateSpearThrowModel(direction.normalized);
                UpdateDirectionLine(
                    mainHandTransform.position - direction.normalized,
                    mainHandTransform.position + direction.normalized * 50);
            }

            if (!useAction.GetStateUp(mainHandInputSource)) {
                return;
            }

            if (snapshots.Count < MIN_SNAPSHOTSCHECK) {
                return;
            }

            if (isThrowing) {
                ResetSpearOffset();
                startAim = Vector3.zero;
                return;
            }

            spawnPoint = mainHandTransform.position;
            var throwing = CalculateThrowAndDistance();
            aimDir = direction.normalized * throwing.ThrowSpeed;

            if (throwing.Distance > minDist) {
                isThrowing = true;
            }

            if (useAction.GetStateUp(mainHandInputSource) && throwing.Distance <= minDist) {
                startAim = Vector3.zero;
                ResetSpearOffset();
            }
        }
        private void UpdateTwoStagedThrowCalculation()
        {
            if (!SteamVR_Actions.valheim_Grab.GetState(mainHandInputSource)) {
                startAim = Vector3.zero;
                ResetSpearOffset();
                return;
            }
            if (!isThrowingStance&&!isThrowing) {
                UpdateDirectionLine(
                    mainHandTransform.position,
                    mainHandTransform.position + (mainHandTransform.TransformDirection(handAimOffset).normalized * 50));
            }
            if (useAction.GetStateDown(mainHandInputSource)) {
                if (startAim == Vector3.zero) {
                    startAim = mainHandTransform.TransformDirection(handAimOffset).normalized;
                }
                isThrowingStance = true;
            }

            if (isThrowingStance) {
                UpdateSpearThrowModel(startAim.normalized);
                UpdateDirectionLine(
                    mainHandTransform.position - startAim,
                    mainHandTransform.position + startAim * 50);
            }

            if (!useAction.GetStateUp(mainHandInputSource)) {
                return;
            }

            if (snapshots.Count < MIN_SNAPSHOTSCHECK) {
                return;
            }

            if (isThrowing) {
                ResetSpearOffset();
                startAim = Vector3.zero;
                return;
            }

            spawnPoint = mainHandTransform.position;
            var throwing = CalculateThrowAndDistance();
            aimDir = startAim.normalized * throwing.ThrowSpeed;

            if (throwing.Distance > minDist) {
                isThrowing = true;
            }

            if (useAction.GetStateUp(mainHandInputSource) && throwing.Distance <= minDist) {
                startAim = Vector3.zero;
                ResetSpearOffset(); 
            }
        }
        private void UpdateDartSpearThrowCalculation()
        {
            if (!SteamVR_Actions.valheim_Grab.GetState(mainHandInputSource)) {
                startAim = Vector3.zero;
                ResetSpearOffset();
                return;
            }
            if (useAction.GetStateDown(mainHandInputSource)) {
                if (startAim == Vector3.zero) {
                    startAim = Player.m_localPlayer.transform.InverseTransformPoint(mainHandTransform.position);
                }
                isThrowingStance = true;
            }

            if (isThrowingStance) {
                var inversePosition = Player.m_localPlayer.transform.TransformDirection(Player.m_localPlayer.transform.InverseTransformPoint(mainHandTransform.position) - startAim).normalized;
                UpdateSpearThrowModel(inversePosition);
                UpdateDirectionLine(
                    mainHandTransform.position - inversePosition, 
                    mainHandTransform.position + inversePosition.normalized * 50);
            }

            if (!useAction.GetStateUp(mainHandInputSource)) {
                return;
            }

            if (snapshots.Count < MIN_SNAPSHOTSCHECK) {
                return;
            }

            if (isThrowing) {
                ResetSpearOffset();
                startAim = Vector3.zero;
                return;
            }

            spawnPoint = mainHandTransform.position;
            var throwing = CalculateThrowAndDistance();
            aimDir = Player.m_localPlayer.transform.TransformDirection(throwing.PosEnd - startAim).normalized * throwing.ThrowSpeed;

            if (throwing.Distance > minDist) {
                isThrowing = true;
            }

            if (useAction.GetStateUp(mainHandInputSource) && throwing.Distance <= minDist) {
                startAim = Vector3.zero;
                ResetSpearOffset();
            }
        }

        private void UpdateSpearThrowModel(Vector3 inversePosition)
        {
            var offsetPos = Vector3.Distance(mainHandTransform.position, rotSave.transform.position);
            transform.position = mainHandTransform.position - Vector3.ClampMagnitude(inversePosition, offsetPos);
            transform.LookAt(mainHandTransform.position + inversePosition);
            transform.localRotation = transform.localRotation * (rotSave.transform.localRotation) * Quaternion.AngleAxis(180, Vector3.right);
        }
        private void ResetSpearOffset()
        {
            transform.position = rotSave.transform.position;
            transform.localRotation = rotSave.transform.localRotation;
            isThrowingStance = false;
            ShieldBlock.instance?.ScaleShieldSize(1f);
        }
        private void UpdateDirectionLine(Vector3 pos1 ,Vector3 pos2)
        {
            if (!VHVRConfig.UseSpearDirectionGraphic()) {
                return;
            }
            List<Vector3> pointList = new List<Vector3>();
            pointList.Add(pos1);
            pointList.Add(pos2);
            directionLine.SetPositions(pointList.ToArray());
            directionLine.enabled = true;
            directionCooldown = totalCooldown;
        }

        class ThrowCalculate
        {
            public Vector3 PosStart { get; set; }
            public Vector3 PosEnd { get; set; }
            public float ThrowSpeed { get; set; }
            public float Distance { get; set; }
            public ThrowCalculate(Vector3 posStart,Vector3 posEnd,float throwSpeed)
            {
                PosStart = posStart;
                PosEnd = posEnd;
                ThrowSpeed = throwSpeed;
                Distance = Vector3.Distance(posEnd, posStart);
            }
        }
        private ThrowCalculate CalculateThrowAndDistance()
        {
            var dist = 0.0f;
            Vector3 posEnd = Player.m_localPlayer.transform.InverseTransformPoint(mainHandTransform.position);
            Vector3 posStart = Player.m_localPlayer.transform.InverseTransformPoint(mainHandTransform.position);

            foreach (Vector3 snapshot in snapshots) {
                var curDist = Vector3.Distance(snapshot, posEnd);
                if (curDist > dist) {
                    dist = curDist;
                    posStart = snapshot;
                }
            }

            var throwSpeed = 1f;
            if (VHVRConfig.SpearThrowSpeedDynamic()) {
                var speedModifier = slowThrowModifier;
                if (dist > fastThrowMinDist) {
                    speedModifier = fastThrowModifier;
                }
                else if (dist > mediumThrowMinDist) {
                    speedModifier = mediumThrowModifier;
                }
                throwSpeed = Vector3.Distance(posEnd, posStart) * speedModifier;
            }
            return new ThrowCalculate(posStart, posEnd, throwSpeed);
        }
    }
}