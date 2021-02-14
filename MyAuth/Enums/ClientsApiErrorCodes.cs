﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAuth.Enums
{
    public enum ClientsApiErrorCodes
    {
        Unauthorized,
        UnauthorizedApplication,
        InternalError,
        NotExistingUser,
        FlaskFaceAuthInternalError,
        BiometricAuthenticationFailure
    }
}
