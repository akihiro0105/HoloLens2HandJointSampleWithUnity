using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if WINDOWS_UWP
#if UNITY_2019
using UnityEngine.XR.WindowsMR;
#else
using UnityEngine.XR.WSA;
#endif
using System.Runtime.InteropServices;
using Windows.Perception.Spatial;
using Windows.Perception.People;
using Windows.UI.Input.Spatial;
#endif

public class HoloLens2HandJointManager : MonoBehaviour
{
#if WINDOWS_UWP
    private SpatialCoordinateSystem spatialCoordinateSystem;
    private HandJointKind[] handJoints;
    private JointPose[] jointPoses;

    private GameObject[] rightHandJoints;
    private GameObject[] leftHandJoints;
#endif

    // Start is called before the first frame update
    void Start()
    {
#if WINDOWS_UWP
        handJoints = new HandJointKind[]
        {
            HandJointKind.Palm,
            HandJointKind.Wrist,
            HandJointKind.ThumbMetacarpal,
            HandJointKind.ThumbProximal,
            HandJointKind.ThumbDistal,
            HandJointKind.ThumbTip,
            HandJointKind.IndexMetacarpal,
            HandJointKind.IndexProximal,
            HandJointKind.IndexIntermediate,
            HandJointKind.IndexDistal,
            HandJointKind.IndexTip,
            HandJointKind.MiddleMetacarpal,
            HandJointKind.MiddleProximal,
            HandJointKind.MiddleIntermediate,
            HandJointKind.MiddleDistal,
            HandJointKind.MiddleTip,
            HandJointKind.RingMetacarpal,
            HandJointKind.RingProximal,
            HandJointKind.RingIntermediate,
            HandJointKind.RingDistal,
            HandJointKind.RingTip,
            HandJointKind.LittleMetacarpal,
            HandJointKind.LittleProximal,
            HandJointKind.LittleIntermediate,
            HandJointKind.LittleDistal,
            HandJointKind.LittleTip
        };
        jointPoses = new JointPose[handJoints.Length];

        rightHandJoints = new GameObject[handJoints.Length];
        leftHandJoints = new GameObject[handJoints.Length];
        for (int i = 0; i < rightHandJoints.Length; i++)
        {
            rightHandJoints[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rightHandJoints[i].transform.localScale = Vector3.one * 0.01f;
            leftHandJoints[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leftHandJoints[i].transform.localScale = Vector3.one * 0.01f;
        }

        SpatialInteractionManager spatialInteraction = null;
#if UNITY_2019
        spatialCoordinateSystem= Marshal.GetObjectForIUnknown(WindowsMREnvironment.OriginSpatialCoordinateSystem) as SpatialCoordinateSystem;
#else
        spatialCoordinateSystem = Marshal.GetObjectForIUnknown(WorldManager.GetNativeISpatialCoordinateSystemPtr()) as SpatialCoordinateSystem;
#endif
        UnityEngine.WSA.Application.InvokeOnUIThread(() =>
        {
            spatialInteraction = SpatialInteractionManager.GetForCurrentView();
        }, true);
        spatialInteraction.SourceUpdated += SpatialInteraction_SourceUpdated;
#endif
    }

#if WINDOWS_UWP
    private void SpatialInteraction_SourceUpdated(SpatialInteractionManager sender, SpatialInteractionSourceEventArgs args)
    {
        var item = args.State;
        var handPose = item.TryGetHandPose();
        if (handPose != null && handPose.TryGetJoints(spatialCoordinateSystem, handJoints, jointPoses))
        {
            for (int i = 0; i < handJoints.Length; i++)
            {
                var joint = i;
                var pos = jointPoses[(int)handJoints[joint]].Position;
                var rot = jointPoses[(int)handJoints[joint]].Orientation;
                if (item.Source.Handedness == SpatialInteractionSourceHandedness.Right)
                {
                    UnityEngine.WSA.Application.InvokeOnAppThread(() =>
                    {
                        rightHandJoints[joint].transform.position = new Vector3(pos.X, pos.Y, -pos.Z);
                        rightHandJoints[joint].transform.rotation = new Quaternion(-rot.X, -rot.Y, rot.Z, rot.W);
                    }, false);
                }
                else if (item.Source.Handedness == SpatialInteractionSourceHandedness.Left)
                {
                    UnityEngine.WSA.Application.InvokeOnAppThread(() =>
                    {
                        leftHandJoints[joint].transform.position = new Vector3(pos.X, pos.Y, -pos.Z);
                        leftHandJoints[joint].transform.rotation = new Quaternion(-rot.X, -rot.Y, rot.Z, rot.W);
                    }, false);
                }
            }
        }
    }
#endif
}
