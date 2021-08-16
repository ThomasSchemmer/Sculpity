//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Valve.VR
{
    using System;
    using UnityEngine;
    
    
    public partial class SteamVR_Actions
    {
        
        private static SteamVR_Action_Pose p_mixedreality_RightController;
        
        private static SteamVR_Action_Pose p_mixedreality_LeftController;
        
        private static SteamVR_Action_Boolean p_mixedreality_RightTrigger;
        
        private static SteamVR_Action_Vector2 p_mixedreality_LeftTouchpad;
        
        private static SteamVR_Action_Pose p_mixedreality_RightControllerTop;
        
        public static SteamVR_Action_Pose mixedreality_RightController
        {
            get
            {
                return SteamVR_Actions.p_mixedreality_RightController.GetCopy<SteamVR_Action_Pose>();
            }
        }
        
        public static SteamVR_Action_Pose mixedreality_LeftController
        {
            get
            {
                return SteamVR_Actions.p_mixedreality_LeftController.GetCopy<SteamVR_Action_Pose>();
            }
        }
        
        public static SteamVR_Action_Boolean mixedreality_RightTrigger
        {
            get
            {
                return SteamVR_Actions.p_mixedreality_RightTrigger.GetCopy<SteamVR_Action_Boolean>();
            }
        }
        
        public static SteamVR_Action_Vector2 mixedreality_LeftTouchpad
        {
            get
            {
                return SteamVR_Actions.p_mixedreality_LeftTouchpad.GetCopy<SteamVR_Action_Vector2>();
            }
        }
        
        public static SteamVR_Action_Pose mixedreality_RightControllerTop
        {
            get
            {
                return SteamVR_Actions.p_mixedreality_RightControllerTop.GetCopy<SteamVR_Action_Pose>();
            }
        }
        
        private static void InitializeActionArrays()
        {
            Valve.VR.SteamVR_Input.actions = new Valve.VR.SteamVR_Action[] {
                    SteamVR_Actions.mixedreality_RightController,
                    SteamVR_Actions.mixedreality_LeftController,
                    SteamVR_Actions.mixedreality_RightTrigger,
                    SteamVR_Actions.mixedreality_LeftTouchpad,
                    SteamVR_Actions.mixedreality_RightControllerTop};
            Valve.VR.SteamVR_Input.actionsIn = new Valve.VR.ISteamVR_Action_In[] {
                    SteamVR_Actions.mixedreality_RightController,
                    SteamVR_Actions.mixedreality_LeftController,
                    SteamVR_Actions.mixedreality_RightTrigger,
                    SteamVR_Actions.mixedreality_LeftTouchpad,
                    SteamVR_Actions.mixedreality_RightControllerTop};
            Valve.VR.SteamVR_Input.actionsOut = new Valve.VR.ISteamVR_Action_Out[0];
            Valve.VR.SteamVR_Input.actionsVibration = new Valve.VR.SteamVR_Action_Vibration[0];
            Valve.VR.SteamVR_Input.actionsPose = new Valve.VR.SteamVR_Action_Pose[] {
                    SteamVR_Actions.mixedreality_RightController,
                    SteamVR_Actions.mixedreality_LeftController,
                    SteamVR_Actions.mixedreality_RightControllerTop};
            Valve.VR.SteamVR_Input.actionsBoolean = new Valve.VR.SteamVR_Action_Boolean[] {
                    SteamVR_Actions.mixedreality_RightTrigger};
            Valve.VR.SteamVR_Input.actionsSingle = new Valve.VR.SteamVR_Action_Single[0];
            Valve.VR.SteamVR_Input.actionsVector2 = new Valve.VR.SteamVR_Action_Vector2[] {
                    SteamVR_Actions.mixedreality_LeftTouchpad};
            Valve.VR.SteamVR_Input.actionsVector3 = new Valve.VR.SteamVR_Action_Vector3[0];
            Valve.VR.SteamVR_Input.actionsSkeleton = new Valve.VR.SteamVR_Action_Skeleton[0];
            Valve.VR.SteamVR_Input.actionsNonPoseNonSkeletonIn = new Valve.VR.ISteamVR_Action_In[] {
                    SteamVR_Actions.mixedreality_RightTrigger,
                    SteamVR_Actions.mixedreality_LeftTouchpad};
        }
        
        private static void PreInitActions()
        {
            SteamVR_Actions.p_mixedreality_RightController = ((SteamVR_Action_Pose)(SteamVR_Action.Create<SteamVR_Action_Pose>("/actions/mixedreality/in/RightController")));
            SteamVR_Actions.p_mixedreality_LeftController = ((SteamVR_Action_Pose)(SteamVR_Action.Create<SteamVR_Action_Pose>("/actions/mixedreality/in/LeftController")));
            SteamVR_Actions.p_mixedreality_RightTrigger = ((SteamVR_Action_Boolean)(SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/mixedreality/in/RightTrigger")));
            SteamVR_Actions.p_mixedreality_LeftTouchpad = ((SteamVR_Action_Vector2)(SteamVR_Action.Create<SteamVR_Action_Vector2>("/actions/mixedreality/in/LeftTouchpad")));
            SteamVR_Actions.p_mixedreality_RightControllerTop = ((SteamVR_Action_Pose)(SteamVR_Action.Create<SteamVR_Action_Pose>("/actions/mixedreality/in/RightControllerTop")));
        }
    }
}
