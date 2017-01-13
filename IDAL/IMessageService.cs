using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL
{
    public interface IMessageService
    {
        bool Insert(Models.CustomMessage msg);
        bool Delete(int id);
        bool Update(Models.CustomMessage msg);
        IList<Models.CustomMessage> SelectAll();
        Models.CustomMessage SelectByID(int id);
    }
}
