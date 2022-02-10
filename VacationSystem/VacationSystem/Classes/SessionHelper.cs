using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace VacationSystem.Classes
{
    /// <summary>
    /// Класс для работы с сессиями
    /// </summary>
    
    static public class SessionHelper
    {
        /// <summary>
        /// Передать объект в сессию как JSON
        /// </summary>
        /// <param name="session">Сессия, в которую передается объект</param>
        /// <param name="key">Ключ объекта в сессии</param>
        /// <param name="value">Передаваемый объект</param>
        static public void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        /// <summary>
        /// Получение объекта из сессии
        /// </summary>
        /// <typeparam name="T">Тип объекта, получаемого из сессии</typeparam>
        /// <param name="session">Сессия, из которой получается объект</param>
        /// <param name="key">Ключ объекта в сессии</param>
        /// <returns>Объект, хранящийся в сессии по заданному ключу</returns>
        static public T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}