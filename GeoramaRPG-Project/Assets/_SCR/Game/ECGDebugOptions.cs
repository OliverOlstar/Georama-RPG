using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;

[Core.DebugOptionList]
public class ECGDebugOptions
{
	public static readonly DebugOption ForceDebugLoc = new DebugOption.String(DebugOption.Group.Misc, "Force Debug Loc", defaultSetting: DebugOption.DefaultSetting.Off);
}
