using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSafeCsChat_Server
{
    enum ServerCommands
    {
        ClientUpdate = 0,
        ClientAuth,
        ClientRegister,
        RecieveMessage,
        OkCheck,
        AddGroupChat
    }
    class ServerSpecificStrings
    {
        public static string MesageTypeOk = "Message=OK";

        public static string MesageTypeSend = "Message=Send";

        public static string MesageTypeGroupEnter = "Message=groupExit";

        public static string MesageTypeGroupExit = "Message=groupEnter";

        public static string MesageTypeUpdateStatus = "Message=UpdateStatus";

        public static string MesageTypeRegister = "Message=Register";

        public static string MesageTypeAuth = "Message=Auth";

        public static string MesageOk = "Message=OK";

        public static string MesageNotOnline = "Message=NotOnline";

        public static string MesageError = "Message=Error";

        public static string ExtractParamValue(string paramName, string fullQs)
        {
            char[] delim = { '&' };
            string[] parameters = fullQs.Split(delim, StringSplitOptions.RemoveEmptyEntries);
            foreach (string parameter in parameters)
            {
                if (parameter.Contains(paramName) && parameter.Contains("="))
                {
                    char[] delimIn = { '=' };
                    return parameter.Split(delimIn, StringSplitOptions.RemoveEmptyEntries)[1];
                }
            }
            return "";
        }

        public static string GetServerRsaPrivateKey()
        {
            return @"<RSAKeyValue><Modulus>si2pEIKNPApfWeIkFMseRTeJm4UBGpDcqlUQvk9AIQTbFdKD5pNzXBRjogaHv98dnibD9JAZ8QNi5is9NGgt2agfmHWziNvmRLFrQwcICUohqULHmZstTYNsjTFy77hpGzPSkBJ3NTDoe1OKdOsCfB/Q/tdGfvIt+FZtPhnnX9WMRyeb7udOKW8R5AvEOM4qBEatLAb6ieaaU9wlMK+NA/SHTgycYsbtKy5/Zq6RKQgoODdc6UTKDnmbuEEssLvH2JM3W9MzGLOBLTn0XEOhs0iEpHfOg10h8NzTcmJeB1RP+KCdp8xB0Ddr2eep3JjZ41kgDlusIv2igqeRExbhQCqDyMA5GFKKkkPhf+KmQzWCFUDvGkL3Cd4oUP7ewwPP6cIb4DFdTE88g67koLHjeKT2TIyISe5LW1baQW8WKzPBTeOvARjsCC+AoZkA6YrSCK8nctwrDqTNzekzcQtuL0AP0a61S8n0mW6x/oJ/o5hLS8wMQuZ4l6s23Nm/T+ND+zu7JheaNrYHQIHJj/7F47Rx7rWhrdYUhg8So8JnFZ6RbO+Sg8ZY9JukqEKr6fZr1LDEJ0f3xVS2SIwcq3sCvVah5NjaE3ymHIY7n5gKgCy9gK8J2O6/vAEd0PheWMV/VnoGzyK8gCE/0GsSG2BkSGkpd/H8XhkkBio7vAJla8U=</Modulus><Exponent>AQAB</Exponent><P>7AnfK5HzBTbaKoS2v4UEMVeOt4GwwpMuuyeNEC8MktP95c0XbdGqsvmNuZNPlDzns919WW2bHWUhLWDJVPgC8Al/S1Y2Sr62V/Z7XUEaa2+yMpqdgBDViSBJ7u6OBP8PKvcP7ZNgRXPw5PK6vcn6/z4kthgfQW9414b1HiAmKss1ZoEz3Zc95oMwCrXKmWt9CqTTvS5MMzx1HmDGbqwss35mtFx5rbjPctfuhkOEOYccXOg2os6SQhr+95grNV598ndsWc5IH+dOgd1ttHQTuk0JFITRnHwC4Z2E9Z9FWbUOZ5uy/uwfrFeiLxLYq83nvPDP1EkKLQqWUXb0Zfz3Jw==</P><Q>wT8kNYbjVM1tuO8dsb9qev3spEYO1E/cLkAul7XAKB9PSbiBTSEZcuxr3KpLYE7W368Z98CR7P/eS0D1UE8eFs1yk5zbTVeCwUVRZG8ieV+v3ZyA+cdZXAoiHaZGV38tg6wSsMK7gSMrCePTWup7L/hslGSgaNTggvWAfLohMP3b+TQOxRvqVrmM9sFFzPGdSB2VFvyBI+wmEF2pi+iT/j4+/oqDgm/fINoWjva/Tw719j83GXtRXEs53YuFEbmyRsw6eKCmPPzlTMJhnzQJ6E7Z2BgUxXBLK5p3qDTCzLTXfEJKePOIOUL7rKYVqT/jLZ0NA4TceQhPBCqssZO5Mw==</Q><DP>IJVwSFx6dtLOpSXy8FdVb9ZNbkmp9AJLZiycjYphKve2yf1Y6aES9mf6x45tYuJRfgPqZOj7i8BFJzWANYmTJj25Y9X/4quXEmeFOiddlU0zBUdQzLgGMkrRyfkGg+wN1PmMKli45n2N2J0laEO0sX2LpdOIpQjVXrNTrRf1DN1GfuMBHOre8a5UeHYGBM3Q0iJU1H3KqRwEcvAbFHYSKakeVnGZnoCvqoPieohhfWuIX87yk+D+Ub/WSKmp9VEZA9q2i2Mtk7HjpYvCtDF9867kvP9kzDxWTXzoONwGv859sZHN0OEZsjnaurLYVEc1YWDA3+ZBUFxwycgpDtcv6w==</DP><DQ>XSa5po+x74Pi2x1wW+kgQ885DeqzZUZlpWnMy9aRnQWSq7+jnHFTGiG60LDKCs+WJlb6loGeFfc0yWOznJW2tFkuXNdkowCHB/EJDffAT29/j7zMMxsUCRjdXxtyluS4JNuCWvBTgClaqrbGf/G9ld8TP+jNphrPanE7EWSLcVk9ev/MCtms+UcSGzQJGIRzQ3r3DhCDk7RWmNb9TnK7O/f3V62fPPX+QZC/BQUOLObSS3QndEIMcwfUCeYiK+wucpAps1IFMCrbwUeF1T85c2DnA5KAGLhLk+YuvcdJC3I02TH/QhRqWufd91hL4rzoHmmxBzw5JqKJrEGB40uJHw==</DQ><InverseQ>S5sAwSiq/y2l4ClExUD5MDarNa2rAG5Du//Lg2hSrO4toMGy5XakfV8U2+cU/KJd29Py88jKjMO/jTKJ2y8Yn98fK1Gtewb74j+5/hSHb1+xe5N4KNPnNmyfS/KzlUGvn0omzlWv9qpk74nFPnM3mXL0BEMR93UnlDXiv1IK7ItRAH+6xXGwZWHvHkCrL2JuDDZhoiLFEZLGlF2Y7hArMSOjPNM1X9VzfE/cmkE/zLs1KjglxPXa+6T9YNAM4StP/ImQrMd/eWgaTd6v24dLm6e3sXS19zefh1Itx57e6W4W0MJNFCqlw7iXUVeJwRmv/HKusSoXtFN8+Ug7owvPZA==</InverseQ><D>CF+s+mGoUZILOSYvm+qP2aDqjqyJiL4WIMsupkMDOIoJ4dSq7kMvwLwUkqDL8HCD69LWkr/S+Ei4dSjDD5t1zCzijz+42E29XK0Z5WSHNCiPvGyvBMEON/5dM6WHSUzZY7Xjx3GWuKE7y8/5SdVhoIO+RrvHTGAFWQ52w9xjy1U16oIqCehE/B2EMbt71U+bGehYayFI1QJJbneCJ++YKP9s/HYLQ3kl3EmrIyBBIjSH71xquWvKMzPz5YzqlMO+eKcyitY6KIcHIyD+IvfquYpG8ZPP2Myfs7otFcsZsC5+M6kYv7IRxb9VCkCg8wUb+FtP7lzQkjRpVsqZPtRscYthH7WAxpjvdr9haBVEC8EGn5TtbI15wxmtk1crGDAEbjdVaLZf+1Y2Avd1ogdsAAqhdSr86S6svNSMw1jIqinCOeDfFDpx0fGx+m7EE9kZdssxcEXOr0VKYGcF7Ucrb7L18KRMVO1Tj3pazcWrDs9Jchp0rC5AJk2U9RU3nlb/p833WOE9mqiCm/uD5Qs8oXo9zYTzGUndvIQecPCLKgKXXWW0ifa0/hWRiamxFalDyoarvuyZl2oCCTsIKIYgF9GxhCi5EF2Vt2IpqHgOR6NGQSvKAvwrU5eKzZgTuAZB87Ca6n+lMWRcNywVfNBzmCJDHd71IBgAThKvy2QN62E=</D></RSAKeyValue>";
        }
    }
}
