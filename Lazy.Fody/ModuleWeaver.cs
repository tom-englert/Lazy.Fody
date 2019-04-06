namespace Lazy.Fody
{
    using System.Collections.Generic;

    using FodyTools;

    using JetBrains.Annotations;

    public class ModuleWeaver : AbstractModuleWeaver
    {
        public override void Execute()
        {
            // Debugger.Launch();

            ModuleDefinition.Process(this, new SystemReferences(this));
        }

        [ItemNotNull]
        [NotNull]
        public override IEnumerable<string> GetAssembliesForScanning()
        {
            yield return "System.Threading";
        }

        public override bool ShouldCleanReference => true;
    }
}
