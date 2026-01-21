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
                // 1. FORZAR TLS 1.2
                // Esto es obligatorio para que .NET 4.8 pueda hablar con los servidores modernos de Google
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                // 2. LEER CONFIGURACIÓN DESDE EL WEB.CONFIG
                // Asegúrate de que los nombres de las llaves coincidan con tu Web.config
                var host = ConfigurationManager.AppSettings["SmtpHost"];
                var portStr = ConfigurationManager.AppSettings["SmtpPort"];
                var user = ConfigurationManager.AppSettings["SmtpUser"];
                var pass = ConfigurationManager.AppSettings["SmtpPass"];
                var sslStr = ConfigurationManager.AppSettings["SmtpSsl"];

                // Validar que la configuración no sea nula
                if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
                {
                    System.Diagnostics.Debug.WriteLine("Error: Configuración SMTP incompleta en Web.config");
                    return false;
                }

                int port = int.Parse(portStr ?? "587");
                bool ssl = bool.Parse(sslStr ?? "true");

                // 3. CONFIGURAR EL CUERPO DEL MENSAJE (DISEÑO INSTITUCIONAL)
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(user, "SISTEMA INVESTIGACIÓN UTC");
                mail.To.Add(new MailAddress(destinatario));
                mail.Subject = asunto;
                mail.IsBodyHtml = true;

                string html = $@"
                    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; border: 1px solid #e2e8f0; border-radius: 10px; overflow: hidden;'>
                        <div style='background-color: #002D72; color: white; padding: 25px; text-align: center;'>
                            <h2 style='margin:0; font-size: 20px;'>{tituloCabecera}</h2>
                        </div>
                        <div style='padding: 30px; color: #334155; line-height: 1.6;'>
                            {mensajeCuerpo}
                            <br><br>
                            <p style='font-size: 11px; color: #94a3b8; border-top: 1px solid #f1f5f9; padding-top: 15px;'>
                                Este es un mensaje automático generado por el Sistema de Gestión CGI - UTC. 
                                Por favor, no responda a este correo electrónico.
                            </p>
                        </div>
                    </div>";

                mail.Body = html;

                // 4. CONFIGURAR EL CLIENTE SMTP
                using (SmtpClient smtp = new SmtpClient(host, port))
                {
                    // IMPORTANTE: El orden de estas propiedades es crítico para evitar Authentication Failed
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(user, pass);
                    smtp.EnableSsl = ssl;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Timeout = 20000; // 20 segundos de espera

                    smtp.Send(mail);
                }

                return true;
            }
            catch (Exception ex)
            {
                // Registra el error exacto en la ventana de Salida (Output) de Visual Studio
                System.Diagnostics.Debug.WriteLine("========================================");
                System.Diagnostics.Debug.WriteLine("ERROR SMTP: " + ex.Message);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("INNER EXCEPTION: " + ex.InnerException.Message);
                }
                System.Diagnostics.Debug.WriteLine("========================================");

                return false;
            }
        }
    }
}