using System;
using System.Runtime.InteropServices;

namespace PowerManagementCOM
{
    [ComVisible(true)]
    [Guid("6307091E-206E-4335-B64B-D1267B45343C")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IPowerManagerCom
    {
        DateTime GetLastSleepTime();

        DateTime GetLastWakeTime();

        bool GetSystemBatteryStateCharging();

        int GetSystemCoolingMode();
    }
}