namespace ExactTarget.TriggeredEmail.Core
{
    public class EmailContentHelper
    {
        public static string GetOpenTrackingTag()
        {
            return "<custom name=\"opencounter\" type=\"tracking\">";
        }

        public static string GetCompanyPhysicalMailingAddressTags()
        {
            return "<table cellpadding=\"2\" cellspacing=\"0\" width=\"600\" ID=\"Table5\" Border=0><tr><td><font face=\"verdana\" size=\"1\" color=\"#444444\">This email was sent by: <b>%%Member_Busname%%</b><br>%%Member_Addr%% %%Member_City%%, %%Member_State%%, %%Member_PostalCode%%, %%Member_Country%%<br><br></font></td></tr></table>";
        }

        public static string GetContentAreaTag(string name)
        {
            return string.Format("<custom type=\"content\" name=\"{0}\">", name);
        }
    }
}
