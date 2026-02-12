

using System.Collections.Generic;

namespace HotUpdate.CoreGame
{
    public interface ILogicConfigContainer
    {
        string ContainerName { get; }
        CustomLogicCfg GetCustomLogicCfg(int id);

        public ICollection<int> GetConfigIDs();
    }


}