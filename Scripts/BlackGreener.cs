using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using VRageMath;
using VRage.Game;
using VRage.Collections;
using Sandbox.ModAPI.Ingame;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using Sandbox.Game.EntityComponents;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ObjectBuilders.Definitions;

namespace IngameScript.BlackGreener {
    public class Program : MyGridProgram {

//================ BlackGreener ================
Program() { }

void Main(string args) {
    var facesProviders = new List<IMyTextSurfaceProvider>();
    GridTerminalSystem.GetBlocksOfType(facesProviders);
    Echo($"FacesProviders: {facesProviders.Count}");
    facesProviders.ForEach(facesProvider => {
        if (facesProvider.SurfaceCount == 5) {
            facesProvider.GetSurface(2).Script = "TSS_Weather";
            facesProvider.GetSurface(4).ContentType = ContentType.SCRIPT;
            facesProvider.GetSurface(4).Script = "TSS_Gravity";
        }
        
        for (var i = 0; i < facesProvider.SurfaceCount; i++) {
            var surface = facesProvider.GetSurface(i);
            Echo($"  {surface.Name} {surface.DisplayName}");
            surface.ScriptForegroundColor = surface.FontColor = Color.Lime;
            surface.ScriptBackgroundColor = surface.BackgroundColor = new Color(0x101010);
        }
    });
}

    }
}