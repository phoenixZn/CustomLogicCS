using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace HotUpdate.CoreGame
{
    using Nodes = List<ICustomNodeCfg>;
    
    public partial class CSharpLogicConfigContainer : Dictionary<int, CustomLogicCfg>, ILogicConfigContainer
    {
        protected System.Type DefaultLogicType;
        
        public CSharpLogicConfigContainer(string name, int capacity = 64)
            : base(capacity)
        {
            ContainerName = name;
            DefaultLogicType = typeof(CustomLogic);
        }
        
        protected CustomLogicCfg AddConfig(int id, Nodes nodes, string desc = null)
        {
            var logicCfg = new CustomLogicCfg(id, nodes, DefaultLogicType, desc);
            Add(id, logicCfg);
            return logicCfg;
        }


        //////////////////////////////////////////////////////////////////////////
        /// ILogicConfigContainer
        public CustomLogicCfg GetCustomLogicCfg(int id)
        {
            if (TryGetValue(id, out var cfg))
                return cfg;
            return null;
        }

        public string ContainerName { get; private set; }

        
        public ICollection<int> GetConfigIDs()
        {
            return Keys;
        }
    }
}