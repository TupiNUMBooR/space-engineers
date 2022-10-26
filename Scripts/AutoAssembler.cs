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
using VRage;

namespace IngameScript.AutoAssembler {
    public class Program : MyGridProgram {

//================ AutoAssembler ================
Program() { }

void Main(string args) {
    var assemblers = new List<IMyAssembler>();
    var containers = new List<IMyCargoContainer>();
    var dict = new Dictionary<string, MyFixedPoint>();

    GridTerminalSystem.GetBlocksOfType(containers);
    GridTerminalSystem.GetBlocksOfType(assemblers);
    
    foreach (var cont in containers) {
        for (int i = 0; i < cont.InventoryCount; i++) {
            var items = new List<MyInventoryItem>();
            cont.GetInventory(i).GetItems(items);
            foreach (var item in items) {
                // var id = $"{item.Type.TypeId}_{item.Type.SubtypeId}";
                var id = $"{item.Type}";
                if (dict.ContainsKey(id)) dict[id] += item.Amount;
                else dict[id] = item.Amount;
            }
        }
    }

    var face = Me.GetSurface(0);
    face.WriteText(dict.Aggregate("abc", (a,b) => $"{a}\n{b.Key}: {b.Value}"));

    var s = "MyObjectBuilder_Component/SteelPlate";
    var n = dict.ContainsKey(s) ? dict[s] : 0;
    MyDefinitionId d;
    MyDefinitionId.TryParse("MyObjectBuilder_BlueprintDefinition", "SteelPlate", out d);
    if (n < 1000) assemblers[0].AddQueueItem(d, 1000-n);
}

    }
}