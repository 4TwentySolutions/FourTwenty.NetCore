using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FourTwenty.Core.Interfaces.Seo
{
    public interface IRobotsProvider
    {
        Task<string[]> GetRobotsLines();
    }
}
