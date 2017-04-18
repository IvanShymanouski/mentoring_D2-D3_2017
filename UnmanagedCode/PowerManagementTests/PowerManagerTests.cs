using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PowerManagement;

namespace PowerManagementTest
{
    [TestClass]
    public class PowerManagerTests
    {
        private static PowerManager _powerManager;

        [TestInitialize]
        public void Initialize()
        {
            _powerManager = new PowerManager();
        }

        [TestMethod]
        public void LastSleepTimeTest()
        {
            Console.WriteLine("Last sleep time: {0}", _powerManager.GetLastSleepTime());
        }

        [TestMethod]
        public void LastWakeTimeTest()
        {
            Console.WriteLine("Last wake time: {0}", _powerManager.GetLastWakeTime());
        }

        [TestMethod]
        public void SystemBatteryStateTest()
        {
            var state = _powerManager.GetSystemBatteryState();

            Console.WriteLine("Battery Present: {0}", state.BatteryPresent);
            Console.WriteLine("Remaining Capacity: {0}", state.RemainingCapacity);
        }

        [TestMethod]
        public void SystemPowerInformationTest()
        {
            var info = _powerManager.GetSystemPowerInformation();

            var time = new TimeSpan(0, 0, (int)info.TimeRemaining);

            Console.WriteLine($"Remaning time: {time.Days} day(s), {time.Minutes} minute(s), {time.Seconds} second(s)");
            //0 - active, 1 - passive, 2- invalid
            Console.WriteLine("Current system cooling mode: {0}", info.CoolingMode);
        }

        [TestMethod]
        public void SuspendSystemTest()
        {
            Assert.Fail("Comment fail assert to enter sleep mode");
           _powerManager.SuspendSystem();
        }

        [TestMethod]
        public void HibernationFileTest()
        {
            _powerManager.ManageHibernationFile(true);
            var dummyToBreakPoint = 0;
            _powerManager.ManageHibernationFile(false);
        }
    }
}
