'CasedHole.vbs
'
'The following scripts demonstrates a simplified way to process
'traveltime and traveltime thickness to determine internal and external
'casing pipe radius.
'
'Author: Timo Korth
'==============================================

'Definition of constants
Const PATH = "C:\Automation\AutoCasedHole\"
Const CONFIGFILE = "C:\Automation\AutoCasedHole\CasedHole.ini"

'Start WellCAD and open document to process
Set obWCAD = CreateObject("WellCAD.Application")
Set obBHDoc = obWCAD.OpenBorehole(PATH & "CasedHole.wcl")
obWCAD.ShowWindow()

' Apply filter to TT log, centralize it and convert to caliper using default fluid velocity
Set obLogTT = obBHDoc.FilterImageLog("Travel Time 1", FALSE, CONFIGFILE)
Set obLogCent = obBHDoc.CentralizeImageData(obLogTT.Name)
obBHDoc.CalculateAcousticCaliper obLogCent.Name, FALSE, CONFIGFILE

'Get last log added to the document
Set obLogAC = obBHDoc.Log(obBHDoc.NbOfLogs - 1)

'Set its name
obLogAC.Name = "Radius TT"

'Remove what is not needed
obBHDoc.RemoveLog(obLogCent.Name)
obBHDoc.RemoveLog(obLogTT.Name)

'Extract a radius curve
obBHDoc.ExtractImageLogStatistics obLogAC.Name, FALSE, CONFIGFILE
Set obLogInner = obBHDoc.Log(obBHDoc.NbOfLogs - 1)
obLogInner.Name = "Inner Radius"

'Rename
Set obLogAmp = obBHDoc.Log("Amplitude 1")
obLogAmp.Name = "Inner Amplitude"

'Filter TTThickness log and compute thickness
Set obLogCondT = obBHDoc.ApplyConditionalTesting("Amplitude Thickness", "Travel Time Thickness", FALSE, CONFIGFILE)
Set obLogTTT = obBHDoc.FilterImageLog(obLogCondT.Name, FALSE, CONFIGFILE)
obBHDoc.CalculateCasingThickness obLogTTT.Name, FALSE, CONFIGFILE

'Get the last log added to the document
Set obLogTh = obBHDoc.Log(obBHDoc.NbOfLogs - 1)

'Remove what is not needed
obBHDoc.RemoveLog(obLogTTT.Name)
obBHDoc.RemoveLog(obLogCondT.Name)

'Get a thickness curve
obBHDoc.ExtractImageLogStatistics obLogTh.Name, FALSE, CONFIGFILE
Set obLogOuter = obBHDoc.Log(obBHDoc.NbOfLogs - 1)
obLogOuter.Name = "Thickness"

'load template which will add thickness to the inner radius to get the outer radius
obBHDoc.ApplyTemplate PATH & "CasedHole.wdt", FALSE, TRUE