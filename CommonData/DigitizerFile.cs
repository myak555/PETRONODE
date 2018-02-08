using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;

namespace Petronode.CommonData
{
    public class DigitizerFile
    {
        public DigitizerCalibration Calibration = new DigitizerCalibration();
        public FitterSolution Solution = new FitterSolution();
        public List<DigitizerPoint> Points = new List<DigitizerPoint>();
        public List<string> PreviousComments = new List<string>();

        /// <summary>
        /// Creates an empty file
        /// </summary>
        public DigitizerFile()
        {
        }

        /// <summary>
        /// Saves file to disk, overwrites
        /// </summary>
        public void Save(string filename, bool PreserveOriginalPoints)
        {
            FileStream fs = null;
            StreamWriter sw = null;
            try
            {
                fs = File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.Read);
                sw = new StreamWriter(fs);
                string previousComment = "";
                foreach (string s in PreviousComments)
                {
                    if (s == previousComment) continue; // remove duplicates
                    sw.WriteLine(s);
                    previousComment = s;
                }
                if (PreviousComments.Count <= 0) sw.WriteLine("#");
                Calibration.SaveCalibration(sw);
                Solution.Save(sw);
                Calibration.SaveHeader(sw, PreserveOriginalPoints);
                if( PreserveOriginalPoints)
                    foreach (DigitizerPoint p in Points) sw.WriteLine(p.OriginalString);
                else
                    foreach (DigitizerPoint p in Points) sw.WriteLine(p.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (sw != null) sw.Close();
                if (fs != null) fs.Close();
            }            
        }

        /// <summary>
        /// Loads file from disk, file must exist
        /// </summary>
        public void Load(string filename)
        {
            PreviousComments.Clear();
            Solution.Functions.Clear();
            Points.Clear();
            FileStream fs = null;
            StreamReader sr = null;
            try
            {
                fs = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                sr = new StreamReader(fs);
                while (!sr.EndOfStream)
                {
                    string s = sr.ReadLine();
                    if (s.StartsWith("#"))
                    {
                        if (Calibration.LoadCalibrationLine(s)) continue;
                        if (Solution.LoadSolutionLine(s)) continue;
                        PreviousComments.Add(s);
                        continue;
                    }
                    Calibration.LoadHeaderLine(s);
                    break;
                }
                while (!sr.EndOfStream)
                {
                    string s = sr.ReadLine();
                    if (s.StartsWith("#")) continue;
                    DigitizerPoint p = new DigitizerPoint(s);
                    Points.Add(p);
                }
                CalibrateToLocation();
                Solution.SelectedFunctionIndex = Solution.Functions.Count - 1;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (sr != null) sr.Close();
                if (fs != null) fs.Close();
            }
        }

        /// <summary>
        /// Calibrates all points (call upon calibration change) 
        /// </summary>
        public void CalibrateToLocation()
        {
            foreach (DigitizerPoint p in Points)
            {
                Calibration.ValueToLocation(p);
            }
        }

        /// <summary>
        /// Calibrates all points (call upon calibration change) 
        /// </summary>
        public void CalibrateToValue()
        {
            foreach (DigitizerPoint p in Points)
            {
                Calibration.LocationToValue(p);
            }
        }

        /// <summary>
        /// Makes a computation
        /// </summary>
        public double RecomputeAll()
        {
            Solution.Compute(Points);
            return Solution.Target;
        }

        /// <summary>
        /// Produces a list of data points for plotting
        /// </summary>
        /// <returns>List of points to draw</returns>
        public List<Point> GetDigitizerPoints()
        {
            List<Point> myPoints = new List<Point>();
            foreach (DigitizerPoint p in Points)
            {
                if (!p.isPlottable) continue;
                myPoints.Add(p.Location);
            }
            return myPoints;
        }

        /// <summary>
        /// Produces a list of fit points for plotting
        /// </summary>
        /// <returns>List of points to draw</returns>
        public List<Point> GetFitPoints()
        {
            List<Point> myPoints = new List<Point>();
            foreach (DigitizerPoint p in Points)
            {
                if (!p.isFitted) continue;
                myPoints.Add( Calibration.ValueToLocation( p.FitValue));
            }
            return myPoints;
        }

        /// <summary>
        /// Produces a list of fit points for plotting
        /// </summary>
        /// <returns>List of points to draw</returns>
        public List<Point> GetPrefitPoints()
        {
            List<Point> myPoints = new List<Point>();
            foreach (DigitizerPoint p in Points)
            {
                if (!p.isFitted) continue;
                myPoints.Add(Calibration.ValueToLocation(p.PrefitValue));
            }
            return myPoints;
        }
    }
}
