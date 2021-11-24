using System;
using System.Collections.Generic;
using System.Text;

namespace Muyan.Search
{
    public interface IEntity<TPrimaryKey>
    {
        TPrimaryKey Id { get; set; }
    }

    public interface IEntity:IEntity<string>
    {

    }
}
