using System;
using System.Linq;
using System.Collections.Generic;
using VacationSystem.Models;

namespace VacationSystem.Classes
{
    static public class DatabaseUpdater
    {
        /// <summary>
        /// Загрузить из API данные о должностях
        /// </summary>
        static public void LoadPositions()
        {
            if (DataHandler.GetPositionsCount() == 0)
                FillPositions();
            else
                UpdatePositions();
        }

        /// <summary>
        /// Заполнение таблицы с должностями
        /// </summary>
        /// <returns>Успешность выполнения операции</returns>
        static private bool FillPositions()
        {
            // получить должности из API
            List<Position> positions = ModelConverter.ConvertToPositions(Connector.GetParsedPositionsList());

            // должности были успешно получены
            if (positions == null)
                return false;
            else
                try
                {
                    using (ApplicationContext db = new ApplicationContext())
                    {
                        db.Positions.AddRange(positions);
                        db.SaveChanges();
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
        }

        /// <summary>
        /// Обновление данных о должностях
        /// </summary>
        /// <returns>Успешность выполнения операции</returns>
        static private bool UpdatePositions()
        {
            // получить из API новые данные о должностях
            List<Position> positionsForUpdate = ModelConverter.ConvertToPositions(Connector.GetParsedPositionsList());

            // проверка получения данных из API
            if (positionsForUpdate == null)
                return false;
            else
                // добавить недостающие должности
                return AddNewPositions(positionsForUpdate);
        }

        /// <summary>
        /// Добавить должности из API, которых ещё нет в БД
        /// </summary>
        /// <param name="positionsForUpdate">Список должностей из API</param>
        /// <returns>Успешность выполнения операции</returns>
        static private bool AddNewPositions(List<Position> positionsForUpdate)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    // получить все должности в БД
                    List<Position> positionsInDb = DataHandler.GetPositions(db);

                    // должности из ответа API, которых ещё нет в БД
                    List<Position> newPositions = positionsForUpdate
                        .Where(p => !positionsInDb.Any(pos => pos.Id == p.Id && pos.Name == p.Name))
                        .ToList();

                    // добавить недостающие должности в БД
                    if (newPositions.Count > 0)
                    {
                        db.Positions.AddRange(newPositions);
                        db.SaveChanges();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
