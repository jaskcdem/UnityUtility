using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class SerializePlayerBar
{
    /* BarMain : sudden Change Bar when got damage, the base requird
     * BarSub : slowly Change Bar when got damage
     */
    public Image HealthbarMain;
    public Image HealthbarSub = null;
    public Image ManabarMain;
    public Image ManabarSub = null;
}

[Serializable]
public class SerializePlayerFloatingBar
{
    //floating bar setting
    public bool showFloating = false;
    public BaseFloatingBar hpBar, mpBar;
    public int defHpBarIndex = 0, defMpBarIndex = 0;
    public int defTextIndex = 0;
    public float floatingStartY = 1.9f;
}

[Serializable]
public class SerializeSingleBar
{
    /* BarMain : sudden Change Bar when got damage, the base requird
     * BarSub : slowly Change Bar when got damage
     */
    public Image BarMain;
    public Image BarSub = null;
}

[Serializable]
public class SerializeSingleFloatingBar
{
    //floating bar setting
    public bool showFloating = false;
    public BaseFloatingBar Bar;
    public int defBarIndex = 0;
    public int defTextIndex = 0;
    public float floatingStartY = 0.9f;
    //child name : auto-binding to "SerializeSingleBar" request
    public string mainChildName = "bar-perent";
    public string subChildName = "bar-perent/bar";
}
