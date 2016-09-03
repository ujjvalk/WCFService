using System;
using System.Text;

namespace WcfService
{
    public class CodeLibrary
    {
        public static string SavePath()
        {
            return System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "File\\";
        }

        public static string ConvertStringToHex(String input, System.Text.Encoding encoding)
        {
            Byte[] stringBytes = encoding.GetBytes(input);
            StringBuilder sbBytes = new StringBuilder(stringBytes.Length * 2);
            foreach (byte b in stringBytes)
            {
                sbBytes.AppendFormat("{0:X2}", b);
            }
            return sbBytes.ToString();
        }

        public string ToHex(string plainText)
        {
            char[] charArray = plainText.ToCharArray();
            StringBuilder mBuilder = new StringBuilder();
            for (int i = 0; i <= charArray.Length - 1; i++)
            {
                char ch = charArray[i];
                mBuilder.Append(Convert.ToInt32(ch).ToString("X4"));
            }

            return mBuilder.ToString();
        }

        public static double Deg2Rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        public static double Rad2Deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }

        public static double Distance(double lat1, double lon1, double lat2, double lon2)
        {
            var theta = lon1 - lon2;
            var dist = Math.Sin(Deg2Rad(lat1)) * Math.Sin(Deg2Rad(lat2)) + Math.Cos(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2)) * Math.Cos(Deg2Rad(theta));
            dist = Math.Acos(dist);
            dist = Rad2Deg(dist);
            dist = dist * 60 * 1.1515;
            return (dist);
        }

        #region Get Universal Date & Time

        private static int DatediffinMin(string zoneId)
        {
            var userReq = TimeZoneInfo.FindSystemTimeZoneById(zoneId);
            var userReqcurrentTime = TimeZoneInfo.ConvertTime(DateTime.Now, userReq);

            var zone = TimeZone.CurrentTimeZone;
            var dbcurrentTime = zone.ToLocalTime(DateTime.Now);

            var span = userReqcurrentTime - dbcurrentTime;
            return Convert.ToInt16(span.TotalMinutes);
        }

        private static int DatediffBetweentwozonesinMin(string eventZone, string userZone)
        {
            var zone1 = TimeZoneInfo.FindSystemTimeZoneById(eventZone);
            var zone1Time = TimeZoneInfo.ConvertTime(DateTime.Now, zone1);

            var zone2 = TimeZoneInfo.FindSystemTimeZoneById(userZone);
            var zone2Time = TimeZoneInfo.ConvertTime(DateTime.Now, zone2);

            var span = zone2Time - zone1Time;
            return Convert.ToInt16(span.TotalMinutes);
        }

        private static DateTime CurrentTime(string zoneId)
        {
            var userReq = TimeZoneInfo.FindSystemTimeZoneById(zoneId);
            return TimeZoneInfo.ConvertTime(DateTime.Now, userReq);
        }

        private static string CurrentOffSet(string zoneId)
        {
            var userReq = TimeZoneInfo.FindSystemTimeZoneById(zoneId);
            if (userReq.GetUtcOffset(DateTime.Now).ToString().Split(':')[0].Contains("-"))
            {
                return userReq.GetUtcOffset(DateTime.Now).ToString().Split(':')[0] + ':' + userReq.GetUtcOffset(DateTime.Now).ToString().Split(':')[1];
            }
            return "+" + userReq.GetUtcOffset(DateTime.Now).ToString().Split(':')[0] + ':' + userReq.GetUtcOffset(DateTime.Now).ToString().Split(':')[1];
        }

        #endregion
    }
}