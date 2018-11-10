namespace Lazy.Fody
{
    using System.Collections.Generic;
    using System.Linq;

    using global::Fody;

    public class ModuleWeaver : BaseModuleWeaver
    {
        public override void Execute()
        {
            var x = ModuleDefinition.GetTypes();
        }

        public override IEnumerable<string> GetAssembliesForScanning() => Enumerable.Empty<string>();
    }
}
