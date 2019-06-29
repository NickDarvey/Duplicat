using System.Management.Automation;

namespace Duplicat.PowerShell
{
    [Cmdlet(VerbsCommon.Find, "Duplicates")]
    [OutputType(typeof(string))]
    public class FindDuplicatesCommand : PSCmdlet
    {
        [Parameter(
            Mandatory = true,
            Position = 0,
            HelpMessage = "The path to the directory in which to search for duplicates.",
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string[] Path { get; set; }

        [Parameter(
            Position = 1,
            HelpMessage = "Included, will search subdirectories. Excluded, will search top-level directory only.")]
        public SwitchParameter Recurse { get; set; } = false;

        protected override void ProcessRecord()
        {
            foreach (var path in Path)
            {
                var isSuccess = LocalDuplicateFinder.TryFind(path, Recurse, out var results, out var errors);

                if (isSuccess) foreach (var result in results) WriteObject(result);

                else ThrowTerminatingError(new ErrorRecord(new PSArgumentException(
                    string.Join("; ", errors), nameof(Path)), null, ErrorCategory.InvalidOperation, null));
            }
        }
    }
}
