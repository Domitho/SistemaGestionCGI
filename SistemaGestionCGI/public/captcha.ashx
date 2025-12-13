<%@ WebHandler Language="C#" Class="captcha" %>

using System;
using System.Web;

using System.Web.SessionState;
using System.Drawing;

public class captcha : IHttpHandler, IRequiresSessionState {
    
    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "image/GIF";
        
        Bitmap imagen_GIF = new System.Drawing.Bitmap(160, 50);
        Graphics grafico = System.Drawing.Graphics.FromImage(imagen_GIF);
        grafico.Clear(Color.NavajoWhite);

        //grafico.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
        Font tipo_fuente = new Font("Comic Sans", 28, FontStyle.Bold);
        
        string randomNum = string.Empty;
        Random autoRand = new Random();

        for (int x = 0; x < 5; x++) {
            randomNum += System.Convert.ToInt32(autoRand.Next(0, 9)).ToString();
        }
        int i_letra = System.Convert.ToInt32(autoRand.Next(65, 90));

        string letra = ((char)i_letra).ToString();
        randomNum += letra;
       // context.Session.Clear();
        
        context.Session["RandomNumero6057"] = randomNum;

        grafico.DrawString(randomNum, tipo_fuente, Brushes.Black, 5, 5);

        imagen_GIF.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Gif);

        tipo_fuente.Dispose();
        grafico.Dispose();
        imagen_GIF.Dispose();
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}