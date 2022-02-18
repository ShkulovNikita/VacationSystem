using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Encodings.Web;

namespace VacationSystem.HtmlHelpers
{
    /// <summary>
    /// Хелпер для отображения кнопок обновления/загрузки данных из БД
    /// </summary>
    static public class UpdateButtonsHelper
    {
        /// <summary>
        /// Создание кнопки загрузки/обновления данных из БД
        /// </summary>
        /// <param name="count">Количество записей в таблице БД</param>
        /// <param name="obj">Тип таблицы БД</param>
        /// <param name="blocked">Нужно ли блокировать кнопку</param>
        static public HtmlString ShowUpdateButton(this IHtmlHelper html, int count, string obj, bool blocked)
        {
            TagBuilder button = new TagBuilder("a");
            button.AddCssClass("btn");
            button.AddCssClass("btn-primary");
            button.MergeAttribute("href", "~/Admin/Update/" + obj);

            // в таблице БД ещё нет данных
            if(count == 0)
                button.InnerHtml.Append("Загрузить данные");
            else
                button.InnerHtml.Append("Обновить данные");

            if (blocked)
                button.MergeAttribute("disabled", "disabled");

            var writer = new System.IO.StringWriter();
            button.WriteTo(writer, HtmlEncoder.Default);

            return new HtmlString(writer.ToString());
        }
    }
}
