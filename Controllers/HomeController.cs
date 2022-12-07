using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.ClientServices;
using System.Web.Mvc;
using Twilio;
using Twilio.AspNet.Mvc;
using Twilio.Rest.Api.V2010.Account;
using Twilio.TwiML;
using HttpPostAttribute = System.Web.Mvc.HttpPostAttribute;
using Twilio.TwiML.Messaging;
using System.Globalization;
using System.Threading.Tasks;


namespace WhatsAppDevol.Controllers
{
    public class WhatsAppController : TwilioController
    {


        [HttpPostAttribute]
        //WhatsApp/Create
        public TwiMLResult Create(FormCollection formCollection)
        {
            var fromNumber = formCollection["From"];
            var textbutton = formCollection["ButtonText"];
            var body = formCollection["Body"];

            var MonthNum = DateTime.Now.Month;
            var YearNum = DateTime.Now.Year;
            var MonthName = DateTime.Now.ToString("MMMM", new CultureInfo("es-MX"));
            var MonthNameCammel = MonthName.Substring(0, 1).ToUpper() + MonthName.Substring(1);

            var fromNum = fromNumber.Substring(fromNumber.IndexOf("+"));
            var fromlength = fromNum.Length;

            if (fromlength > 13)
            {
                fromNum = fromNum.Substring(0, 3) + fromNum.Substring(4);
            }

            var mediaURL = new Uri("https://repositoriodevoldocs.blob.core.windows.net/nomina/" + YearNum + "/" + MonthNum + "/" + MonthNum + "-" + YearNum + fromNum + ".pdf");

            var mediaURLNomina = new Uri("https://repositoriodevoldocs.blob.core.windows.net/nomina/nomina-prueba.pdf");

            var mediaURLHorario = new Uri("https://repositoriodevoldocs.blob.core.windows.net/horario/horario-prueba.pdf");


            var response = new MessagingResponse();

            body = body.ToUpper();

            if (body == "DESCARGAR PDF NÓMINA" || textbutton == "DESCARGAR PDF NÓMINA")
            {
                var message = new Message();
                message.Media(mediaURLNomina);
                response.Append(message);

                response.Message("¿Algo más en que te pueda ayudar?");

            }
            else if (body == "CONSULTAR MI HORARIO" || textbutton == "CONSULTAR MI HORARIO")
            {
                var message = new Message();
                message.Media(mediaURLHorario);
                response.Append(message);

                response.Message("¿Algo más en que te pueda ayudar?");
            }
            else if (body == "SI" || textbutton == "SI")
            {
                response.Message("Selecciona alguna de las siguientes opciones:");
            }
            else if (body == "NO" || textbutton == "NO")
            {
                response.Message("Muchas gracias, ¡vuelve pronto!");
            }
            else
            {
                response.Message("Hola, gracias por escribir al servicio de chat de Ánfora. ¿Dime en que te puedo ayudar?");
                response.Message("Selecciona alguna de las siguientes opciones:");
            }

            return TwiML(response);
        }

        [HttpPostAttribute]
        //WhatsApp/FallBack
        public TwiMLResult FallBack(FormCollection formCollection)
        {
            var response = new MessagingResponse();

            var message = new Message();
            message.Body("Hola, estamos presentando fallas técnicas en este momento, por favor intentalo de nuevo más tarde.");
            response.Append(message);

            return TwiML(response);
        }

        [HttpPostAttribute]
        //WhatsApp/CallBack
        public TwiMLResult CallBack(FormCollection formCollection)
        {
            var status = formCollection["MessageStatus"];
            var error = "Hola, estamos presentando fallas técnicas en este momento, por favor intentalo de nuevo más tarde.";

            Console.WriteLine(status);
            var response = new MessagingResponse();

            if (status.ToString() == "failed" || status.ToString() == "undelivered")
            {
                response.Message(error);
            }

            return TwiML(response);
        }

        static void MessageMedia(string From, string To, string media)
        {

            string accountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
            string authToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");

            TwilioClient.Init(accountSid, authToken);

            var mediaURL = new[] {
                    new Uri (media)
                }.ToList();

            var message = MessageResource.Create(
                mediaUrl: mediaURL,
                from: new Twilio.Types.PhoneNumber(To),
                to: new Twilio.Types.PhoneNumber(From)
            );
        }

    }
}