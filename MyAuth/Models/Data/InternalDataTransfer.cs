using MyAuth.Utils.Extentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAuth.Models.Data
{
    public class InternalDataTransfer<T>
    {
        public bool Status { get; set; }
        public T Data { get; set; }
        public IntrnalErrorObject Error { get; set; }

        public InternalDataTransfer()
        {
        }

        public InternalDataTransfer(T data)
        {
            Status = true;
            Data = data;
        }

        public InternalDataTransfer(Exception e)
        {
            Status = false;
            Error = new IntrnalErrorObject(e);
        }

        public InternalDataTransfer(Exception e, string description)
        {
            Status = false;
            Error = new IntrnalErrorObject(e, description);
        }

        public InternalDataTransfer(bool status, string description)
        {
            Status = status;
            if (!status)
            {
                Error = new IntrnalErrorObject(description);
            }
        }

        public InternalDataTransfer(IntrnalErrorObject error)
        {
            Status = false;
            Error = error;

        }


        public class IntrnalErrorObject
        {
            public string Error { get; set; }
            public string Description { get; set; }
            public bool IsExceptionTypeError { get; set; }

            public IntrnalErrorObject()
            {
            }

            public IntrnalErrorObject(Exception e)
            {
                Error = e.Message;
                Description = e.StackTrace;
                IsExceptionTypeError = true;
            }

            public IntrnalErrorObject(Exception e, string description)
            {
                if (description == null)
                {
                    Error = e.Message;
                    Description = e.StackTrace;
                    IsExceptionTypeError = true;
                }
                else
                {
                    Error = $"Message: { e.Message }, StackTrace: { e.StackTrace }";
                    Description = description;
                    IsExceptionTypeError = true;
                }

            }

            public IntrnalErrorObject(string description)
            {
                Error = "Error";
                Description = description;
                IsExceptionTypeError = false;
            }

            public TheTypeError<TypeError> GetErrorTypeByDescription<TypeError>() where TypeError : Enum
            {
                if (IsExceptionTypeError)
                {
                    throw new Exception($"{Error} - {Description}");
                }
                return new TheTypeError<TypeError>(Description.GetValueByName<TypeError>());
            }

            public TheTypeError<TypeError> GetErrorTypeByErrorStr<TypeError>() where TypeError : Enum
            {
                if (IsExceptionTypeError)
                {
                    throw new Exception($"{Error} - {Description}");
                }
                return new TheTypeError<TypeError>(Error.GetValueByName<TypeError>());
            }

            public class TheTypeError<TypeError> where TypeError : Enum
            {
                public TheTypeError()
                {
                }

                public TheTypeError(TypeError typeError)
                {
                    Value = typeError;
                }

                public TypeError Value { get; set; }
            }
        }
    }
}
