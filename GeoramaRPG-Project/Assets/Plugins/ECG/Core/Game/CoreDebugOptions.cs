using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Core.DebugOptionList]
public class CoreDebugOptions
{
	public static readonly DebugOption ForceDebugLoc = new DebugOption.String(DebugOption.Group.Misc, "Force Debug Loc", defaultSetting: DebugOption.DefaultSetting.Off);
}
