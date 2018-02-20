using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using Petronode.OilfieldFileAccess.Converters;

namespace Petronode.AUS_Geomag
{
    /// <summary>
    /// Sends the requests to the AGRF site and recovers responses
    /// Properly formed request:
    /// http://www.ga.gov.au/oracle/cgi/geoAGRF.sh?
    /// latd=-25&latm=00&lats=00&lond=136&lonm=00&lons=00&elev=1.1&year=2011&month=09&day=10&Ein=D&Ein=F
    /// </summary>
    public class AGRFGeomagRequest
    {
        /// <summary>
        /// AGRF website
        /// </summary>
        public string AGRFsite = "http://www.ga.gov.au/oracle/cgi/geoAGRF.sh";

        /// <summary>
        /// Latitude
        /// </summary>
        public Latitude lat = new Latitude();

        /// <summary>
        /// Longitude
        /// </summary>
        public Longitude lon = new Longitude();

        /// <summary>
        /// Depth in meters down as for logging
        /// </summary>
        public double depth = 0.0;

        /// <summary>
        /// logging date
        /// </summary>
        public DateTime date = DateTime.Now;

        /// <summary>
        /// Magnetic field declination
        /// </summary>
        public double Declination = Double.NaN;

        /// <summary>
        /// Magnetic field force
        /// </summary>
        public double FieldAmplitude = Double.NaN;

        /// <summary>
        /// Creates a request class
        /// </summary>
        public AGRFGeomagRequest()
        {
        }

        /// <summary>
        /// Retrieves true if the location is in East Australia
        /// </summary>
        public bool IsEasternAustralia
        {
            get
            {
                if (lat.AngleD < -41.0) return false;
                if (lat.AngleD > -17.0) return false;
                if (lon.AngleD < 142.0) return false;
                if (lon.AngleD > 155.0) return false;
                return true;
            }
        }

        /// <summary>
        /// Requests data from the site
        /// </summary>
        public void RequestData()
        {
            string s = FormRequest();
            WebRequest hwr = WebRequest.Create(s);
            WebResponse wr = hwr.GetResponse();
            Stream reader = wr.GetResponseStream();
            byte[] buffer = new byte[80];
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 1000; i++)
            {
                int count = reader.Read(buffer, 0, buffer.Length);
                if (count <= 0) break;
                for (int j = 0; j < count; j++)
                {
                    char c = (buffer[j] == 0) ? ' ' : Convert.ToChar(buffer[j]);
                    sb.Append(c);
                }
            }
            reader.Close();
            wr.Close();
            string total = sb.ToString();
            int pos = total.IndexOf("<br>D  =");
            if (pos > 0)
            {
                string tmp = total.Substring(pos+8, 20).Trim();
                string[] ss = tmp.Split(' ');
                Declination = Convert.ToDouble(ss[0]);
            }
            pos = total.IndexOf("<br>F  =");
            if (pos > 0)
            {
                string tmp = total.Substring(pos + 8, 20).Trim();
                string[] ss = tmp.Split(' ');
                FieldAmplitude = Convert.ToDouble(ss[0]);
            }
        }

        /// <summary>
        /// Forms latitude request
        /// </summary>
        private string FormLatitudeRequest()
        {
            int degrees = lat.degrees;
            int minutes = lat.minutes;
            int seconds = lat.seconds;
            StringBuilder sb = new StringBuilder();
            sb.Append( "?latd=" + ((degrees==0)? "000": degrees.ToString("0")));
            sb.Append( "&latm=" + ((minutes==0)? "00": minutes.ToString("00")));
            sb.Append( "&lats=" + ((seconds==0)? "00": seconds.ToString("00")));
            return sb.ToString();
        }

        /// <summary>
        /// Forms longitude request
        /// </summary>
        private string FormLongitudeRequest()
        {
            int degrees = lon.degrees;
            int minutes = lon.minutes;
            int seconds = lon.seconds;
            StringBuilder sb = new StringBuilder();
            sb.Append( "&lond=" + ((degrees==0)? "000": degrees.ToString("0")));
            sb.Append( "&lonm=" + ((minutes==0)? "00": minutes.ToString("00")));
            sb.Append( "&lons=" + ((seconds==0)? "00": seconds.ToString("00")));
            return sb.ToString();
        }

        /// <summary>
        /// Forms elevation request
        /// </summary>
        private string FormElevationRequest()
        {
            double elev = -depth / 1000.0; // to km, up positive (as for topography)
            return "&elev=" + elev.ToString("0.000");
        }

        /// <summary>
        /// Forms date part of request
        /// </summary>
        private string FormDateRequest()
        {
            StringBuilder sb = new StringBuilder("&year=");
            sb.Append( date.Year.ToString("0"));
            sb.Append( "&month=");
            sb.Append( date.Month.ToString("00"));
            sb.Append( "&day=");
            sb.Append( date.Day.ToString("0"));
            return sb.ToString();
        }

        /// <summary>
        /// Forms the full request string
        /// </summary>
        /// <returns></returns>
        private string FormRequest()
        {
            StringBuilder sb = new StringBuilder(AGRFsite);
            sb.Append(FormLatitudeRequest());
            sb.Append(FormLongitudeRequest());
            sb.Append(FormElevationRequest());
            sb.Append(FormDateRequest());
            sb.Append("&Ein=D&Ein=F");
            return sb.ToString();
        }
    }
}
