Set obWCAD = CreateObject("WellCAD.Application")
obWCAD.ShowWindow()
Set obBHole = obWCAD.NewBorehole()
obBHole.Name = "MyDoc"
Set obLithoLog = obBHole.InsertNewLog(7)
obLithoLog.Name = "Litho"
