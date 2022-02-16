using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Encodings.Web;

namespace VacationSystem.HtmlHelpers
{
    /// <summary>
    /// Хелпер для отображения сообщений об ошибках
    /// </summary>
    static public class ErrorHelper
    {
        static public HtmlString CheckError(this IHtmlHelper html, string error)
        {
            // ошибки нет - не нужно ничего выводить
            if (error == null)
                return null;
            else
            {
                // формирование HTML-блока ошибки
                TagBuilder errorDiv = new TagBuilder("div");
                errorDiv.AddCssClass("alert");
                errorDiv.AddCssClass("alert-danger");
                errorDiv.InnerHtml.Append(error);

                // запись HTML-кода
                var writer = new System.IO.StringWriter();
                errorDiv.WriteTo(writer, HtmlEncoder.Default);

                return new HtmlString(writer.ToString());
            }
        }
    }
}
