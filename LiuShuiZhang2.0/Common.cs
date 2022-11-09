using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
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

        public NumericUpDownEx() : base() { }

        public NumericUpDownEx(bool isDisplayValueMode) : base()
        {
            Maximum = 999999999999999;
            Minimum = -999999999999999;
            Increment = 0;
            BorderStyle = BorderStyle.None;
            
            if (isDisplayValueMode)
            {
                BackColor = SystemColors.Info;
                ReadOnly = true;
            }
            else
            {
                BackColor = SystemColors.Window;
            }
        }

        protected override void OnValueChanged(EventArgs e)
        {
            base.OnValueChanged(e);
            valued = Value;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Controls[0].Hide();
        }

        protected override void OnTextBoxResize(object source, EventArgs e)
        {
            Controls[1].Width = Width;
        }
    }
}
