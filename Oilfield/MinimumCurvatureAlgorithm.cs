using System;
using System.Collections.Generic;
using System.Text;
using Petronode.OilfieldFileAccess;

namespace Petronode.Oilfield
{
    /// <summary>
    /// Performs minimum curvature computation
    /// </summary>
    public class MinimumCurvatureAlgorithm
    {
        private Oilfield_File m_file = null;

        /// <summary>
        /// Constructor; assigns the Oilfield file
        /// </summary>
        /// <param name="file">data file</param>
        public MinimumCurvatureAlgorithm( Oilfield_File file)
        {
            m_file = file;
        }

        /// <summary>
        /// Computes the classic min curvature algorithm
        /// </summary>
        /// <param name="azimuth_channel_name"></param>
        /// <param name="tilt_channel_name"></param>
        /// <param name="northing_channel_name"></param>
        /// <param name="easting_channel_name"></param>
        /// <param name="tvd_channel_name"></param>
        /// <param name="dls_channel_name"></param>
        public void Compute( string azimuth_channel_name, string tilt_channel_name,
            string northing_channel_name, string easting_channel_name,
            string tvd_channel_name, string dls_channel_name)
        {
            if( m_file == null) return;
            Oilfield_Channel depth = m_file.GetIndex();
            Oilfield_Channel azim = m_file.GetChannel(azimuth_channel_name);
            if( azim == null) throw new Exception( "Channel " + azimuth_channel_name + " is not found in file " + m_file.FileName);
            Oilfield_Channel tilt = m_file.GetChannel(tilt_channel_name);
            if( tilt == null) throw new Exception( "Channel " + tilt_channel_name + " is not found in file " + m_file.FileName);
            Oilfield_Channel northing = m_file.GetOrCreateChannel(northing_channel_name, "M", "Northing coordinate", "0.000");
            if (northing == null) throw new Exception("Channel " + northing_channel_name + " cannot be created");
            Oilfield_Channel easting = m_file.GetOrCreateChannel(easting_channel_name, "M", "Easting coordinate", "0.000");
            if (easting == null) throw new Exception("Channel " + easting_channel_name + " cannot be created");
            Oilfield_Channel tvd = m_file.GetOrCreateChannel(tvd_channel_name, "M", "True vertical depth", "0.000");
            if (tvd == null) throw new Exception("Channel " + tvd_channel_name + " cannot be created");
            Oilfield_Channel dls = m_file.GetOrCreateChannel(dls_channel_name, "deg/30M", "Dogleg severity", "0.000");
            if (dls == null) throw new Exception("Channel " + dls_channel_name + " cannot be created");
            if (depth.Data[0] < depth.Data[depth.Data.Count - 1])
                ComputeFromTop(depth, azim, tilt, northing, easting, tvd, dls);
            else
                ComputeFromBottom(depth, azim, tilt, northing, easting, tvd, dls);
        }

        /// <summary>
        /// Corrects the channel for arbitrary tie-in
        /// </summary>
        /// <param name="channel_name"></param>
        /// <param name="TieIn"></param>
        public void CorrectToTiein(string channel_name, double TieIn)
        {
            Oilfield_Channel chan = m_file.GetChannel(channel_name);
            if (chan == null) throw new Exception("Channel " + channel_name + " is not found in file " + m_file.FileName);
            if (chan.Data.Count <= 0) return;
            double delta = TieIn - chan.Data[0];
            for (int i = 0; i < chan.Data.Count; i++)
            {
                chan.Data[i] += delta;
            }
        }

        /// <summary>
        /// Computes the displacement relative to the wellhead
        /// </summary>
        /// <param name="north_channel_name"></param>
        /// <param name="TieIn"></param>
        public void ComputeDisplacement(string north_channel_name, string east_channel_name, string disp_channel_name)
        {
            Oilfield_Channel nort = m_file.GetChannel(north_channel_name);
            if (nort == null) throw new Exception("Channel " + north_channel_name + " is not found in file " + m_file.FileName);
            if (nort.Data.Count <= 0) return;
            Oilfield_Channel east = m_file.GetChannel(east_channel_name);
            if (east == null) throw new Exception("Channel " + east_channel_name + " is not found in file " + m_file.FileName);
            if (east.Data.Count <= 0) return;
            Oilfield_Channel chan = m_file.GetChannel(disp_channel_name);
            if (chan == null) throw new Exception("Channel " + disp_channel_name + " is not found in file " + m_file.FileName);
            if (chan.Data.Count <= 0) return;
            for (int i = 0; i < chan.Data.Count; i++)
            {
                double d = nort.Data[i] * nort.Data[i] + east.Data[i] * east.Data[i];
                chan.Data[i] = Math.Sqrt( d);
            }
        }

        /// <summary>
        /// Computes the displacement relative to the wellhead
        /// </summary>
        /// <param name="north_channel_name"></param>
        /// <param name="TieIn"></param>
        /// <param name="correction">TVD correction (up-positive)</param>
        public void ComputeTVD(string tvd_channel_name, string out_channel_name, double correction)
        {
            Oilfield_Channel tvd = m_file.GetChannel(tvd_channel_name);
            if (tvd == null) throw new Exception("Channel " + tvd_channel_name + " is not found in file " + m_file.FileName);
            if (tvd.Data.Count <= 0) return;
            Oilfield_Channel chan = m_file.GetChannel(out_channel_name);
            if (chan == null) throw new Exception("Channel " + out_channel_name + " is not found in file " + m_file.FileName);
            if (chan.Data.Count <= 0) return;
            for (int i = 0; i < chan.Data.Count; i++)
            {
                chan.Data[i] = tvd.Data[i] - correction;
            }
        }

        private void ComputeFromTop(Oilfield_Channel depth, Oilfield_Channel azim, Oilfield_Channel tilt,
            Oilfield_Channel northing, Oilfield_Channel easting, Oilfield_Channel tvd, Oilfield_Channel dls)
        {
            tvd.Data[0] = depth.Data[0];
            northing.Data[0] = 0.0;
            easting.Data[0] = 0.0;
            dls.Data[0] = 0.0;

            for (int i = 1; i < depth.Data.Count; i++)
            {
                double dMD = depth.Data[i] - depth.Data[i - 1];
                double A1 = 0.0;
                double I1 = 0.0;
                double A2 = 0.0;
                double I2 = 0.0;
                if (Double.IsNaN(azim.Data[i - 1]) || Double.IsNaN(tilt.Data[i - 1]))
                {
                    if (!Double.IsNaN(azim.Data[i]) && Double.IsNaN(tilt.Data[i]))
                    {
                        A1 = azim.Data[i] * Math.PI / 180.0;
                        I1 = tilt.Data[i] * Math.PI / 180.0;
                        A2 = azim.Data[i] * Math.PI / 180.0;
                        I2 = tilt.Data[i] * Math.PI / 180.0;
                    }
                }
                else if (Double.IsNaN(azim.Data[i]) || Double.IsNaN(tilt.Data[i]))
                {
                    if (!Double.IsNaN(azim.Data[i - 1]) && Double.IsNaN(tilt.Data[i - 1]))
                    {
                        A1 = azim.Data[i - 1] * Math.PI / 180.0;
                        I1 = tilt.Data[i - 1] * Math.PI / 180.0;
                        A2 = azim.Data[i - 1] * Math.PI / 180.0;
                        I2 = tilt.Data[i - 1] * Math.PI / 180.0;
                    }
                }
                else
                {
                    A1 = azim.Data[i - 1] * Math.PI / 180.0;
                    I1 = tilt.Data[i - 1] * Math.PI / 180.0;
                    A2 = azim.Data[i] * Math.PI / 180.0;
                    I2 = tilt.Data[i] * Math.PI / 180.0;
                }
                double dl = Math.Acos(Math.Cos(I2 - I1) - Math.Sin(I1) * Math.Sin(I2) * (1.0 - Math.Cos(A2 - A1)));
                double rf = 1.0;
                if (Math.Abs(dl) > 0.00001) rf = 2.0 * Math.Tan(dl * 0.5) / dl;
                double dTVD = 0.5 * dMD * (Math.Cos(I1) + Math.Cos(I2)) * rf;
                double dN = 0.5 * dMD * (Math.Sin(I1) * Math.Cos(A1) + Math.Sin(I2) * Math.Cos(A2)) * rf;
                double dE = 0.5 * dMD * (Math.Sin(I1) * Math.Sin(A1) + Math.Sin(I2) * Math.Sin(A2)) * rf;
                tvd.Data[i] = tvd.Data[i - 1] + dTVD;
                northing.Data[i] = northing.Data[i - 1] + dN;
                easting.Data[i] = easting.Data[i - 1] + dE;
                dls.Data[i] = dl * 30.0;
            }
        }

        private void ComputeFromBottom(Oilfield_Channel depth, Oilfield_Channel azim, Oilfield_Channel tilt,
            Oilfield_Channel northing, Oilfield_Channel easting, Oilfield_Channel tvd, Oilfield_Channel dls)
        {
            int l = depth.Data.Count-1;
            tvd.Data[l] = depth.Data[l];
            northing.Data[l] = 0.0;
            easting.Data[l] = 0.0;
            dls.Data[l] = 0.0;

            for (int i = l-1; i >= 0; i--)
            {
                double dMD = depth.Data[i] - depth.Data[i + 1];
                double A1 = 0.0;
                double I1 = 0.0;
                double A2 = 0.0;
                double I2 = 0.0;
                if (Double.IsNaN(azim.Data[i + 1]) || Double.IsNaN(tilt.Data[i + 1]))
                {
                    if (!Double.IsNaN(azim.Data[i]) && Double.IsNaN(tilt.Data[i]))
                    {
                        A1 = azim.Data[i] * Math.PI / 180.0;
                        I1 = tilt.Data[i] * Math.PI / 180.0;
                        A2 = azim.Data[i] * Math.PI / 180.0;
                        I2 = tilt.Data[i] * Math.PI / 180.0;
                    }
                }
                else if (Double.IsNaN(azim.Data[i]) || Double.IsNaN(tilt.Data[i]))
                {
                    if (!Double.IsNaN(azim.Data[i + 1]) && Double.IsNaN(tilt.Data[i + 1]))
                    {
                        A1 = azim.Data[i + 1] * Math.PI / 180.0;
                        I1 = tilt.Data[i + 1] * Math.PI / 180.0;
                        A2 = azim.Data[i + 1] * Math.PI / 180.0;
                        I2 = tilt.Data[i + 1] * Math.PI / 180.0;
                    }
                }
                else
                {
                    A1 = azim.Data[i + 1] * Math.PI / 180.0;
                    I1 = tilt.Data[i + 1] * Math.PI / 180.0;
                    A2 = azim.Data[i] * Math.PI / 180.0;
                    I2 = tilt.Data[i] * Math.PI / 180.0;
                }
                double dl = Math.Acos(Math.Cos(I2 - I1) - Math.Sin(I1) * Math.Sin(I2) * (1.0 - Math.Cos(A2 - A1)));
                double rf = 1.0;
                if (Math.Abs(dl) > 0.00001) rf = 2.0 * Math.Tan(dl * 0.5) / dl;
                double dTVD = 0.5 * dMD * (Math.Cos(I1) + Math.Cos(I2)) * rf;
                double dN = 0.5 * dMD * (Math.Sin(I1) * Math.Cos(A1) + Math.Sin(I2) * Math.Cos(A2)) * rf;
                double dE = 0.5 * dMD * (Math.Sin(I1) * Math.Sin(A1) + Math.Sin(I2) * Math.Sin(A2)) * rf;
                tvd.Data[i] = tvd.Data[i + 1] + dTVD;
                northing.Data[i] = northing.Data[i + 1] + dN;
                easting.Data[i] = easting.Data[i + 1] + dE;
                dls.Data[i] = dl * 30.0;
            }
        }
    }
}
