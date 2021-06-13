using System.Collections.Generic;

namespace BarakaBg.Services.Data
{
    public interface IIngredientsService
    {
        IEnumerable<T> GetAll<T>();
    }
}