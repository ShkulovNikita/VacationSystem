using System;
using System.Linq;
using System.Collections.Generic;
using VacationSystem.Models;

namespace VacationSystem.Classes.Database
{
    /// <summary>
    /// Класс для загрузки и обновления данных в БД
    /// о должностях, получаемых из API
    /// </summary>
    static public class PositionsUpdater
    {
        /// <summary>
        /// Загрузить из API данные о должностях
        /// </summary>
        /// <returns>Успешность выполнения операции</returns>
        static public bool LoadPositions()
        {
            if (DataHandler.GetPositionsCount() == 0)
                return FillPositions();
            else
                return UpdatePositions();
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
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    // получить все должности в БД
                    List<Position> positionsInDb = DataHandler.GetPositions(db);
                    if (positionsInDb == null)
                        return false;
                    else
                    {
                        // добавить новые должности в БД
                        bool addingNewPositions = AddNewPositions(db, positionsForUpdate, positionsInDb);

                        // обновить названия должностей
                        bool renamingPositions = RenamePositions(db, positionsForUpdate);

                        return addingNewPositions && renamingPositions;
                    }
                }
            }
        }

        /// <summary>
        /// Добавить должности из API, которых ещё нет в БД
        /// </summary>
        /// <param name="db">Контекст БД</param>
        /// <param name="positionsForUpdate">Список должностей из API</param>
        /// <param name="positionsInDb">Список должностей в БД</param>
        /// <returns>Успешность выполнения операции</returns>
        static private bool AddNewPositions(ApplicationContext db, List<Position> positionsForUpdate, List<Position> positionsInDb)
        {
            try
            {
                // должности из ответа API, которых ещё нет в БД
                List<Position> newPositions = positionsForUpdate
                    .Where(p => !positionsInDb.Any(pos => pos.Id == p.Id))
                    .ToList();

                // добавить недостающие должности в БД
                if (newPositions.Count > 0)
                {
                    db.Positions.AddRange(newPositions);
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
        /// Обновить имена должностей, которые не совпадают в БД
        /// и в ответе от API
        /// </summary>
        /// <param name="db">Контекст БД</param>
        /// <param name="positionsForUpdate">Список должностей из API</param>
        /// <returns>Успешность выполнения операции</returns>
        static private bool RenamePositions(ApplicationContext db, List<Position> positionsForUpdate)
        {
            try
            {
                // проверка соответствия имен в БД и API
                foreach (Position position in positionsForUpdate)
                {
                    Position pos = DataHandler.GetPositionById(position.Id);
                    if (pos.Name != position.Name)
                        pos.Name = position.Name;
                }

                db.SaveChanges();

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
