﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if MATHEMATICS
using Unity.Mathematics;
#endif

#if SPLINES
using UnityEngine.Splines;
#endif

namespace sc.modeling.splines.runtime
{
    public partial class SplineMesher
    {
        [Serializable]
        public class Cap
        {
            public Cap(Position position)
            {
                this.position = position;
            }
            
            public enum Position
            {
                Start,
                End
            }
            public readonly Position position;
            
            public GameObject prefab;
            [SerializeField] [ HideInInspector]
            private int previousPrefabID;

            public bool HasPrefabChanged()
            {
                if (prefab == null) return true;

                int hashCode = prefab.GetHashCode();
                if (hashCode != previousPrefabID)
                {
                    previousPrefabID = hashCode;
                    
                    return true;
                }
                
                return false;
            }
            
            [Tooltip("Positional offset, relative to the curve's tangent")]
            public Vector3 offset;
            [Tooltip("Shifts the object along the spline curve by this many units")]
            [Min(0f)]
            public float shift = 0f;
            
            [Tooltip("Align the object's forward direction to the tangent and roll of the spline")]
            public bool align = true;
            [Tooltip("Rotation in degrees, added to the object's rotation")]
            public Vector3 rotation;
            
            [Tooltip("Factor in the scale configured under the Deforming section, as well as scale data points created in the editor.")]
            public bool matchScale = true;
            public Vector3 scale = Vector3.one;
            
            //Save a reference to the instantiated objects, so they can be accessed again, deleted when necessary.
            //[HideInInspector]
            public GameObject[] instances = Array.Empty<GameObject>();

            public bool RequiresRespawn()
            {
                return HasPrefabChanged() || HasNoInstances() || HasMissingInstances();
            }
            
            public bool HasNoInstances()
            {
                return instances.Length == 0;
            }
            
            public bool HasMissingInstances()
            {
                for (int i = 0; i < instances.Length; i++)
                {
                    if (instances[i] == null) return true;
                }
                return false;
            }

            public void DestroyInstances()
            {
                //Destroy any existing instances
                for (int i = 0; i < instances.Length; i++)
                {
                    DestroyInstance(instances[i]);
                }
            }
            
            private static void DestroyInstance(Object obj)
            {
                #if UNITY_EDITOR
                if (Application.isPlaying && !UnityEditor.EditorApplication.isPaused)
                    Destroy(obj);
                else
                    DestroyImmediate(obj);
                #else
                Destroy(obj);
                #endif
            }
            
            public void Respawn(int splineCount, Transform parent)
            {
                DestroyInstances();
            
                //Nothing to spawn, clear out
                if (prefab == null)
                {
                    //Debug.Log("Cap has no prefab, clearing and bailing");
                    instances = Array.Empty<GameObject>();
                    return;
                }

                //One cap gets spawned per spline
                instances = new GameObject[splineCount];

                //Respawn the prefabs (once per spline)
                for (int i = 0; i < instances.Length; i++)
                {
                    GameObject instance = InstantiatePrefab(prefab);
                    instance.transform.SetParent(parent);
                
                    instances[i] = instance;
                }
            }
        
            private GameObject InstantiatePrefab(Object source)
            {
                bool isPrefab = false;
            
                #if UNITY_EDITOR
                if (UnityEditor.PrefabUtility.GetPrefabAssetType(source) == PrefabAssetType.Variant)
                {
                    //PrefabUtility.GetCorrespondingObjectFromSource still returns the base prefab. However, this does work.
                    isPrefab = source;
                }
                else
                {
                    Object original = UnityEditor.PrefabUtility.GetCorrespondingObjectFromOriginalSource(source);

                    isPrefab = original;

                    //This is necessary if the source if a prefab already instantiate in the scene
                    if (isPrefab) source = original;
                }
                #endif
                
                GameObject instance = null;
            
                #if UNITY_EDITOR
                if (isPrefab)
                {
                    instance = UnityEditor.PrefabUtility.InstantiatePrefab(source) as GameObject;
                }
                #endif

                //Non-prefabs and builds
                if (!isPrefab)
                {
                    instance = GameObject.Instantiate(source) as GameObject;
                }

                if (instance == null)
                {
                    Debug.LogError($"Failed to spawn instance. Source is prefab: {isPrefab}");
                }
                
                return instance;
            }
            
            //Transforms the prefab to the desired spline position and rotation
            public void ApplyTransform(SplineMesher splineMesher)
            {
                #if MATHEMATICS
                for (int splineIndex = 0; splineIndex < instances.Length; splineIndex++)
                {
                    Transform target = this.instances[splineIndex].transform;
                    
                    //Coincidentally the two enum values correspond to 0 and 1
                    float t = (int)position;

                    float splineLength = splineMesher.splineContainer.Splines[splineIndex].CalculateLength(splineMesher.splineContainer.transform.localToWorldMatrix);

                    float shiftLength = shift;

                    if (position == Position.Start) shiftLength += splineMesher.settings.distribution.trimStart;
                    else if (position == Position.End) shiftLength += splineMesher.settings.distribution.trimEnd;
                    
                    //Shift along spline by X-units
                    float tOffset = (shiftLength) / splineLength;
                    if (position == Cap.Position.End) tOffset = -tOffset;

                    t += tOffset;
            
                    //Ensure a tangent can always be derived at very the start and end
                    t = Mathf.Clamp(t, 0.0001f, 0.9999f);
            
                    splineMesher.splineContainer.Splines[splineIndex].Evaluate(t, out float3 splinePoint, out float3 tangent, out float3 up);
                    
                    float3 forward = math.normalize(tangent);
                    float3 right = math.cross(forward, up);
            
                    //Rotation
                    Quaternion m_rotation = Quaternion.Euler(rotation);
                    if (align)
                    {
                        m_rotation = Quaternion.LookRotation(forward, up) * Quaternion.Euler(rotation);
                        
                        m_rotation = splineMesher.SampleRollRotation(splineMesher.splineContainer.Splines[splineIndex], forward, t * splineLength, splineIndex) * m_rotation;
                        right = m_rotation * math.right();
                        up = m_rotation * math.up();

                        //Flipped?
                        if(position == Position.End || (position == Position.Start && scale.z < 0)) right = -right;
                    }

                    //Offset
                    splinePoint += right * (offset.x - splineMesher.settings.deforming.curveOffset.x);
                    splinePoint += up * (offset.y - splineMesher.settings.deforming.curveOffset.y);
                    splinePoint += forward * offset.z;

                    splinePoint.x += splineMesher.settings.deforming.pivotOffset.x;
                    splinePoint.y += splineMesher.settings.deforming.pivotOffset.y;
                    
                    //Position of point on spline in world-space
                    splinePoint = splineMesher.splineContainer.transform.TransformPoint(splinePoint);
                    
                    if (splineMesher.settings.conforming.enable)
                    {
                        if (SplineMeshGenerator.PerformConforming(splinePoint, splineMesher.settings.conforming, 1f, out float3 hitPosition, out float3 hitNormal))
                        {
                            splinePoint.y = hitPosition.y + offset.y;

                            quaternion hitRotation = quaternion.LookRotationSafe(tangent, hitNormal);
                            
                            if (splineMesher.settings.conforming.align)
                            {
                                //m_rotation = hitRotation;
                            }
                        }
                    }
                    
                    Vector3 m_scale = scale;
                    if (matchScale)
                    {
                        m_scale.x *= splineMesher.settings.deforming.scale.x;
                        m_scale.y *= splineMesher.settings.deforming.scale.y;
                        m_scale.z *= splineMesher.settings.deforming.scale.z;

                        float3 splineScale = splineMesher.SampleScale(t * splineLength, splineIndex);
                        m_scale.x *= splineScale.x;
                        m_scale.y *= splineScale.y;
                    }
                    target.localScale = m_scale;
                    
                    target.SetPositionAndRotation(splinePoint, m_rotation);

                    //Gray out fields as any chances would be overwritten anyway
                    target.hideFlags = HideFlags.NotEditable;
                    //this.instances[splineIndex].hideFlags = hideInstances ? HideFlags.HideInHierarchy : HideFlags.None;
                }
                #endif
            }
        }

        public Cap startCap = new Cap(Cap.Position.Start);
        public Cap endCap = new Cap(Cap.Position.End);
        
        #if SPLINES
        /// <summary>
        /// Updates the Transform of the start/end caps. Respawns them if necessary (ie. prefab changed)
        /// </summary>
        public partial void UpdateCaps()
        {
            if (!splineContainer) return;

            var splineCountChanged = splineContainer.Splines.Count != splineCount;
            //if(splineCountChanged) Debug.Log($"Spline count changed from {splineCount} to {splineContainer.Splines.Count}");

            if (splineCountChanged || startCap.RequiresRespawn())
            {
                startCap.Respawn(splineCount, this.transform);
            }
            
            if (splineCountChanged || endCap.RequiresRespawn())
            {
                endCap.Respawn(splineCount, this.transform);
            }
            
            startCap.ApplyTransform(this);
            endCap.ApplyTransform(this);
        }
        #endif
    }
}