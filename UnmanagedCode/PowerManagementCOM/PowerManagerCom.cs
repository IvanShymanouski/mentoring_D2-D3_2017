using System;
using System.Runtime.InteropServices;
using PowerManagement;

namespace PowerManagementCOM
{
    [ComVisible(true)]
    [Guid("84521DAE-1B25-41F1-9548-3B72EDE2223E")]
    [ClassInterface(ClassInterfaceType.None)]
    public sealed class PowerManagerCom : IPowerManagerCom
    {
        private readonly PowerManager _powerManager;

        public PowerManagerCom()
        {
            _powerManager = new PowerManager();
        }

        public DateTime GetLastSleepTime()
        {
            return _powerManager.GetLastSleepTime();
        }

        public DateTime GetLastWakeTime()
        {
            return _powerManager.GetLastWakeTime();
        }

        public bool GetSystemBatteryStateCharging()
        {
            return _powerManager.GetSystemBatteryState().Charging;
        }

        public int GetSystemCoolingMode()
        {
            return _powerManager.GetSystemPowerInformation().CoolingMode;
        }
    }
}
