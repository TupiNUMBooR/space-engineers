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

namespace IngameScript.Sling {
    public class Program : MyGridProgram {


//================ Sling ================
//1.03 km = 143 deg
float secondsBeforeRelease = 15;
float targetVelocity = 999; //-30:30 RPM
float releaseAngleError = 1;

List<IMyMotorStator> spinRotors = new List<IMyMotorStator>();
List<IMyPistonBase> spinPistons = new List<IMyPistonBase>();
List<IMyShipMergeBlock> merges = new List<IMyShipMergeBlock>();
List<IMyWarhead> warheads = new List<IMyWarhead>();
IMyTextSurface face;
float releaseAngle;
string log;
Stage stage;
DateTime time;

public Program() {
    UpdateBlocks();
    face = Me.GetSurface(0);
    face.ContentType = ContentType.TEXT_AND_IMAGE;
    face.FontSize = 2;
    face.Alignment = TextAlignment.CENTER;
    face.FontColor = Color.Lime;
    face.BackgroundColor = new Color(0x101010);
    face.WriteText($"Rotors: {spinRotors.Count}\nMerge blocks: {merges.Count}\nWarheads: {warheads.Count}\nArgument: [0-360] deg");

    Detonation = false;
    Velocity = 0;
    Merge = true;
    Updating = false;
    Retracting = false;
}

void Save() { }

void Main() {
    face.WriteText("ololo");
}

void Main(string argument) {
    var angle = Angle;
    log = $"{stage}\n" +
          $"A:{releaseAngle:N2}; a:{angle:N2}\n" +
          $"t:{DateTime.Now - time}\n";

    if (argument != "") {
        var valid = float.TryParse(argument, out releaseAngle) && releaseAngle >= 0 && releaseAngle <= 360;
        time = DateTime.Now.AddSeconds(secondsBeforeRelease);
        stage = valid ? Stage.SpeedingUp : Stage.Off;
        Updating = valid;
        Velocity = valid ? targetVelocity : 0;
        Retracting = valid;
    }

    switch (stage) {
        case Stage.Off:
            break;

        case Stage.SpeedingUp:
            if (DateTime.Now > time) stage = Stage.WaitAngleToRelease;
            break;

        case Stage.WaitAngleToRelease:
            if (Math.Abs(angle - releaseAngle) < releaseAngleError) {
                Detonation = true;
                Merge = false;
                Velocity = 0;
                Retracting = false;
                time = DateTime.Now.AddSeconds(1);
                stage = Stage.WaitToTurnOnMerge;
            }

            break;

        case Stage.WaitToTurnOnMerge:
            if (DateTime.Now > time) {
                Merge = true;
                Updating = false;
                log += "Finished";
                stage = Stage.Off;
            }

            break;
    }

    face.WriteText(log);
}

float Angle => spinRotors.Sum(r => r.Angle) / (float) Math.PI * 180 % 360;

bool Merge {
    set {
        merges.ForEach(m => m.GetActionWithName($"OnOff_{(value ? "On" : "Off")}").Apply(m));
        UpdateBlocks();
    }
}

float Velocity {
    set => spinRotors.ForEach(r => r.TargetVelocityRPM = value);
}

bool Retracting {
    set => spinPistons.ForEach(p => p.Velocity = value ? 0.5f : -5f);
}

bool Updating {
    set {
        if (value)
            Runtime.UpdateFrequency |= UpdateFrequency.Update1;
        else
            Runtime.UpdateFrequency &= ~UpdateFrequency.Update1;
    }
}

bool Detonation {
    set => warheads.ForEach(w => {
            w.IsArmed = value;
            w.DetonationTime = value ? 15 : 0;
            if (value) w.StartCountdown();
            else w.StopCountdown();
        });
}

void UpdateBlocks() {
    GridTerminalSystem.GetBlockGroupWithName("Spinner Rotors").GetBlocksOfType(spinRotors);
    GridTerminalSystem.GetBlockGroupWithName("Spinner Pistons").GetBlocksOfType(spinPistons);
    GridTerminalSystem.GetBlocksOfType(merges);
    GridTerminalSystem.GetBlocksOfType(warheads);
}

enum Stage {
    Off,
    SpeedingUp,
    WaitAngleToRelease,
    WaitToTurnOnMerge
}


    }
}