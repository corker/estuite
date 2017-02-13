using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using log4net;

namespace Estuite.Example
{
    public class ProgramRunner
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IEnumerable<IRunExamples> _examples;

        public ProgramRunner(IEnumerable<IRunExamples> examples)
        {
            _examples = examples;
        }

        public void Run()
        {
            var tasks = _examples.Select(x => x.Run()).ToArray();
            Task.WaitAll(tasks);
        }
    }
}