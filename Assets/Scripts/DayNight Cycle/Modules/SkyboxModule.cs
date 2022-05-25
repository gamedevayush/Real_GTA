using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRolijk.DayNight.Module
{
    public class SkyboxModule : DayNightModuleBase
    {
        [SerializeField] Gradient skyColor, groundColor;

        public override void UpdateModule(float intensity)
        {
            RenderSettings.skybox.SetColor("_SkyTint", skyColor.Evaluate(intensity));
            RenderSettings.skybox.SetColor("_GroundColor", groundColor.Evaluate(intensity));
        }
    }
}
