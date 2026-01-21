using System;
using System.Net;
using System.Net.Mail;
using System.Configuration;

namespace SistemaGestionCGI.Utilidades
{
    public class EmailService
    {
        public static bool EnviarCorreo(string destinatario, string asunto, string mensajeCuerpo, string tituloCabecera)
        {
            try
            {
                // 1. FORZAR PROTOCOLO TLS 1.2 (Vital para Gmail en .NET antiguo)
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

                // 2. LEER CONFIGURACIÓN DEL WEB.CONFIG
                var host = ConfigurationManager.AppSettings["SmtpHost"];
                var port = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);
                var user = ConfigurationManager.AppSettings["SmtpUser"];
                var pass = ConfigurationManager.AppSettings["SmtpPass"];
                var ssl = bool.Parse(ConfigurationManager.AppSettings["SmtpSsl"]);

                // 3. CONFIGURAR EL MENSAJE
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(user, "SISTEMA INVESTIGACIÓN UTC");
                mail.To.Add(new MailAddress(destinatario));
                mail.Subject = asunto;
                mail.IsBodyHtml = true;

                string html = $@"
                    <div style='font-family: sans-serif; max-width: 600px; margin: auto; border: 1px solid #e2e8f0; border-radius: 12px; overflow: hidden;'>
                        <div style='background-color: #312783; color: white; padding: 30px; text-align: center;'>
                            <h2 style='margin:0;'>{tituloCabecera}</h2>
                        </div>
                        <div style='padding: 30px; color: #1e293b; line-height: 1.6;'>
                            {mensajeCuerpo}
                            <br><br>
                            <p style='font-size: 11px; color: #64748b; border-top: 1px solid #f1f5f9; pt-20;'>
                                Este es un mensaje automático generado por el Sistema CGI - UTC. Por favor, no responda a este correo.
                            </p>
                        </div>
                    </div>";

                mail.Body = html;

                using (SmtpClient smtp = new SmtpClient(host, port))
                {
                    smtp.UseDefaultCredentials = false; 
                    smtp.Credentials = new NetworkCredential(user, pass);
                    smtp.EnableSsl = ssl;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                    smtp.Send(mail);
                }

                return true;
            }
            catch (Exception ex)
            {
                // Para depuración: revisa la "Output" de Visual Studio
                System.Diagnostics.Debug.WriteLine("FALLO CRÍTICO SMTP: " + ex.Message);
                if (ex.InnerException != null)
                    System.Diagnostics.Debug.WriteLine("DETALLE INTERNO: " + ex.InnerException.Message);

                return false;
            }
        }
    }
}