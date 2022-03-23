using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Encodings.Web;

namespace VacationSystem.HtmlHelpers
{
    static public class MessageHelper
    {
        /// <summary>
        /// Вывод сообщения об успешно произведенном действии
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        static public HtmlString CheckSuccess(this IHtmlHelper html, string message)
        {
            // ошибки сообщения нет - не нужно ничего выводить
            if (message == null)
                return null;
            else
            {
                // формирование HTML-блока ошибки
                TagBuilder errorDiv = new TagBuilder("div");
                errorDiv.AddCssClass("alert");
                errorDiv.AddCssClass("alert-success");
                errorDiv.InnerHtml.Append(message);

                // запись HTML-кода
                var writer = new System.IO.StringWriter();
                errorDiv.WriteTo(writer, HtmlEncoder.Default);

                return new HtmlString(writer.ToString());
            }
        }

        static public HtmlString CheckMessage(this IHtmlHelper html, string message)
        {
            // сообщения нет - не нужно ничего выводить
            if (message == null)
                return null;
            else
            {
                // формирование HTML-блока сообщения
                TagBuilder errorDiv = new TagBuilder("div");
                errorDiv.AddCssClass("alert");
                errorDiv.AddCssClass("alert-info");
                errorDiv.InnerHtml.Append(message);

                // запись HTML-кода
                var writer = new System.IO.StringWriter();
                errorDiv.WriteTo(writer, HtmlEncoder.Default);

                return new HtmlString(writer.ToString());
            }
        }
    }
}
