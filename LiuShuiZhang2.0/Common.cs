using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiuShuiZhang2._0
{
    public static class Common
    {
        public static string constr = ConfigurationManager.ConnectionStrings["LSZConstr"].ConnectionString;

        public static void FixNumbericUpDownMockup(Control f)
        {
            foreach (NumericUpDown n in GetAllControlByType(f, typeof(NumericUpDown)))
            {
                if (n.Tag != null)
                {
                    if (n.Tag.ToString().Split(';')[0] == "display")
                    {
                        n.Enabled = false;
                        n.BackColor = System.Drawing.SystemColors.Info;
                        n.BorderStyle = BorderStyle.None;
                        n.Controls[0].Hide();
                    }
                    else if (n.Tag.ToString().Split(';')[0] == "text")
                    {
                        n.BorderStyle = BorderStyle.None;
                        n.Controls[0].Hide();
                    }
                }
            }
        }

        public static IEnumerable<Control> GetAllControlByType(Control control, Type type)
        {
            var controls = control.Controls.Cast<Control>();

            return controls.SelectMany(ctrl => GetAllControlByType(ctrl, type))
                                      .Concat(controls)
                                      .Where(c => c.GetType() == type);
        }

        public static bool IsNumber(string s)
        {
            return Regex.IsMatch(s, @"^\d+$");
        }

        public static bool NotInludeSpecialChar(string s)
        {
            return Regex.IsMatch(s, "^[a-z0-9]*$");
        }

        public static string EncodePasswordToBase64(string password)
        {
            try
            {
                byte[] encData_byte = new byte[password.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(password);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Encode" + ex.Message);
            }
        }
        //this function Convert to Decord your Password
        public static string DecodeFrom64(string encodedData)
        {
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            System.Text.Decoder utf8Decode = encoder.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(encodedData);
            int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            string result = new String(decoded_char);
            return result;
        }
    }

    public class NumericUpDownEx:NumericUpDown
    {
        private decimal valued;

        public decimal Valued { get => valued; set => valued = value; }

        protected override void OnValueChanged(EventArgs e)
        {
            base.OnValueChanged(e);
            this.valued = this.Value;
        }
    }
}
