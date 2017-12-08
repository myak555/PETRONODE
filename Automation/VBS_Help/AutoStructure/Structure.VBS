'Structure.vbs
'
'The script combines several WellCAD processes
'from the Image Module and demonstrates the automatic insertion
'of Polar&Rose and Cross Section boxes.
'
'Author: Timo Korth
'==============================================

'Definition of constants
const PATH = "C:\Automation\AutoStructure\"
const CONFIGFILE = "C:\Automation\AutoStructure\Structure.ini"

'Start WellCAD and retrieve the application object
Set obWCAD = CreateObject("WellCAD.Application")
obWCAD.ShowWindow()

'Open a WellCAD file and retrieve the document object
Set obBHDoc = obWCAD.OpenBorehole(PATH & "StructureRaw.wcl")

'Filter TT log, centralize it and convert to caliper using default fluid velocity
Set obLogTT = obBHDoc.FilterImageLog("TT", FALSE, CONFIGFILE)
Set obLogCent = obBHDoc.CentralizeImageData(obLogTT.Name)
obBHDoc.CalculateAcousticCaliper obLogCent.Name, FALSE, CONFIGFILE

'Get last log added to the document and rename
Set obLogAC = obBHDoc.Log(obBHDoc.NbOfLogs - 1)
obLogAC.Name = "Radius TT"

'Remove what is not needed
obBHDoc.RemoveLog(obLogCent.Name)
obBHDoc.RemoveLog(obLogTT.Name)

'Filter and normalize amplitude image
Set obLogNorm = obBHDoc.NormalizeImage("AMP Raw", FALSE, CONFIGFILE)
obLogNorm.Name = "AMP Static"

'Apply apparent to true correction to imported structure
Set obLogTrue = obBHDoc.ApplyStructureApparentToTrueCorrection("Struture Apparent", FALSE, CONFIGFILE)
obLogTrue.Name = "Structure True"

'Load template
obBHDoc.ApplyTemplate PATH & "Structure.wdt", FALSE, TRUE

'Display a cross section every 1 m
Set obCrossLog = obBHDoc.Log("Cross Section")
top = obLogAC.TopDepth
bot = obLogAC.BottomDepth
For depth = top To bot Step 1
	obCrossLog.InsertNewCrossBox top, depth
	top = depth
Next

'Display a rose diagram every 2 m
Set obPolarLog = obBHDoc.Log("Rose Diagram")
top = obLogAC.TopDepth
bot = obLogAC.BottomDepth
For depth = top To bot Step 2
	obPolarLog.InsertNewSchmitBox top, depth, ""
	top = depth
Next

'Display a dip diagram every 2 m
Set obDipLog = obBHDoc.Log("Dip Histogram")
top = obLogAC.TopDepth
bot = obLogAC.BottomDepth
For depth = top To bot Step 2
	obDipLog.InsertNewSchmitBox top, depth, ""
	top = depth
Next

