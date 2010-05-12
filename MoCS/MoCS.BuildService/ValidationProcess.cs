using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoCS.BuildService.Business;
using System.Threading;
using MoCS.Business.Objects;

namespace MoCS.BuildService
{
    public class ValidationProcess
    {
        private DateTime _startTime;
        private Submit _submit;
        private SubmitValidator _validator;
        public Thread Thread { get; private set; }

        public ValidationProcess(Submit submit, DateTime startTime)
        {
            _submit = submit;
            _startTime = startTime;
        }

        public DateTime ProcessingDate { get { return _startTime; } }

        public Submit Submit { get { return _submit; } }

        public void SetThread(Thread thread)
        {
            Thread = thread;
        }

        public void SetProcessor(SubmitValidator validator)
        {
            _validator = Validator;
        }

        public SubmitValidator Validator
        { get { return _validator; } }

        public ValidationResult Result { get; set; }


    }
}
