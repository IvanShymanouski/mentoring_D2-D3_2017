Dim powerManager
Set powerManager = CreateObject("PowerManagementCOM.PowerManagerCom")
Dim result 
result = ""
result = result & "Last sleep Time: " & powerManager.GetLastSleepTime() & vbCrLf
result = result & "Last wake Time: " & powerManager.GetLastWakeTime() & vbCrLf
result = result & "Is charging: " & powerManager.GetSystemBatteryStateCharging() & vbCrLf
result = result & "Current system cooling mode: " & powerManager.GetSystemCoolingMode()
MsgBox result