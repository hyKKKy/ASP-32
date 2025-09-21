using ASP_32.Data.Entities;
using System.Collections;

namespace ASP_32.Models.Home
{
    public class HomeIndexViewModel
    {
        public IEnumerable<ProductGroup> ProductGroups { get; set; } = [];
    }
}
/* Д.З. Реалізувати валідацію моделі товарної групи
 * Виводити повідомлення про її статус
 * Обмежити додавання до БД моделей, що не пройшли валідацію
 */