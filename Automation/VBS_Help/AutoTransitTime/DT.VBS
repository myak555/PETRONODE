'DT.vbs
'
'This script demonstrates a quick look first arrival pick
'from Rx1, Rx2 and Rx3 and calculates the transit time
'
'Author: Timo Korth (ALT)
'==============================================

'Definition of constants
Const PATH = "C:\Automation\AutoTransitTime\"
Const CONFIGFILE = "C:\Automation\AutoTransitTime\DT.ini"

'Start WellCAD
Set obWCAD = CreateObject("WellCAD.Application")
obWCAD.ShowWindow()

'Create new borehole document from existing WellCAD file
Set obBHDoc = obWCAD.OpenBorehole(PATH & "DT.WCL")

'Run the first arrival picking process and filter the result
Set obDTRx1 = obBHDoc.PickFirstArrival("RX1", FALSE, CONFIGFILE)
Set obDTRx1Flt = obBHDoc.FilterLog(obDTRx1.Name, FALSE, CONFIGFILE)
obDTRx1Flt.Name = "DT1"

Set obDTRx2 = obBHDoc.PickFirstArrival("RX2", FALSE, CONFIGFILE)
Set obDTRx2Flt = obBHDoc.FilterLog(obDTRx2.Name, FALSE, CONFIGFILE)
obDTRx2Flt.Name = "DT2"

Set obDTRx3 = obBHDoc.PickFirstArrival("RX3", FALSE, CONFIGFILE)
Set obDTRx3Flt = obBHDoc.FilterLog(obDTRx3.Name, FALSE, CONFIGFILE)
obDTRx3Flt.Name = "DT3"

'Insert formula logs for the transit time
Set obDT1 = obBHDoc.InsertNewLog(2) 
obDT1.Name = "TT Rx1 Rx2"
obDT1.Formula = "{DT2}-{DT1}"

Set obDT2 = obBHDoc.InsertNewLog(2) 
obDT2.Name = "TT Rx2 Rx3"
obDT2.Formula = "{DT3}-{DT2}"

'Apply final template
obBHDoc.ApplyTemplate PATH & "DT.WDT"